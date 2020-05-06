using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class AzureDevOpsDeploymentFrequencyDA
    {
        public async Task<List<AzureDevOpsBuild>> GetDeployments(string patToken, string organization, string project, string branch, string buildId)
        {
            List<AzureDevOpsBuild> builds = new List<AzureDevOpsBuild>();
            string buildListResponse = await SendAzureDevOpsMessage(patToken, $"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1&queryOrder=BuildQueryOrder,finishTimeDescending");
            if (string.IsNullOrEmpty(buildListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(buildListResponse);
                Newtonsoft.Json.Linq.JArray value = buildListObject.value;
                builds = JsonConvert.DeserializeObject<List<AzureDevOpsBuild>>(value.ToString());
            }
            //construct the Url to the build
            foreach (AzureDevOpsBuild item in builds)
            {
                item.url = $"https://dev.azure.com/{organization}/{project}/_build/results?buildId={item.id}&view=results";
            }
            //sort the list
            builds = builds.OrderBy(o => o.queueTime).ToList();
            return builds;
        }

        public async Task<DeploymentFrequencyModel> GetDeploymentFrequency(string patToken, string organization, string project, string branch, string buildId, int numberOfDays)
        {
            float deploymentsPerDay = 0;
            DeploymentFrequency deploymentFrequency = new DeploymentFrequency();

            ////Gets a list of builds
            //GET https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1         
            string buildListResponse = await SendAzureDevOpsMessage(patToken, $"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1&queryOrder=BuildQueryOrder,finishTimeDescending");
            //Console.WriteLine(buildListResponse);
            if (string.IsNullOrEmpty(buildListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(buildListResponse);
                Newtonsoft.Json.Linq.JArray value = buildListObject.value;
                IEnumerable<AzureDevOpsBuild> builds = JsonConvert.DeserializeObject<List<AzureDevOpsBuild>>(value.ToString());

                List<KeyValuePair<DateTime, DateTime>> dateList = new List<KeyValuePair<DateTime, DateTime>>();
                foreach (AzureDevOpsBuild item in builds)
                {
                    if (item.status == "completed" && item.sourceBranch == branch && item.queueTime > DateTime.Now.AddDays(-numberOfDays))
                    {
                        KeyValuePair<DateTime, DateTime> newItem = new KeyValuePair<DateTime, DateTime>(item.queueTime, item.queueTime);
                        dateList.Add(newItem);
                    }
                }

                deploymentsPerDay = deploymentFrequency.ProcessDeploymentFrequency(dateList, "", numberOfDays);
                //////Gets build detail
                //////GET https://dev.azure.com/{organization}/{project}/_apis/build/builds/{buildId}?api-version=5.1
                //string buildDetailResponse = await Base.SendAzureDevOpsMessage($"https://dev.azure.com/{organization}/{project}/_apis/build/builds/{buildId}?api-version=5.1");
                //Console.WriteLine(buildDetailResponse);

                //dynamic buildObject = JsonConvert.DeserializeObject(buildDetailResponse);
                ////string status = buildObject.status;
                ////string finishTime = buildObject.finishTime;
                ////Console.WriteLine(status);
                ////Console.WriteLine(finishTime);
            }
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                deploymentsPerDay = deploymentsPerDay,
                deploymentsPerDayDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay)
            };
            return model;
        }

        public async Task<string> SendAzureDevOpsMessage(string patToken, string url)
        {
            string responseBody = "";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", patToken))));
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
            }
            return responseBody;
        }
    }
}
