using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess.APIAccess
{
    public class AzureDevOpsAPIAccess
    {
        //Call the Azure DevOps Rest API to get a JSON array of builds
        public async Task<Newtonsoft.Json.Linq.JArray> GetAzureDevOpsBuildsJArray(string patToken, string organization, string project)
        {
            Newtonsoft.Json.Linq.JArray list = null;
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1&queryOrder=BuildQueryOrder,finishTimeDescending";
            string response = await GetAzureDevOpsMessage(url, patToken);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(response);
                list = jsonObj.value;
            }
            return list;
        }

        //Call the Azure DevOps Rest API to get a JSON array of pull requests
        public async Task<Newtonsoft.Json.Linq.JArray> GetAzureDevOpsPullRequestsJArray(string patToken, string organization, string project, string repositoryId)
        {
            Newtonsoft.Json.Linq.JArray list = null;
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/git/pull%20requests/get%20pull%20requests?view=azure-devops-rest-5.1
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullrequests?searchCriteria.status=completed&api-version=5.1";
            string response = await GetAzureDevOpsMessage(url, patToken);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                list = buildListObject.value;
            }
            return list;
        }

        //Call the Azure DevOps Rest API to get a JSON array of pull request commits
        public async Task<Newtonsoft.Json.Linq.JArray> GetAzureDevOpsPullRequestCommitsJArray(string patToken, string organization, string project, string repositoryId, string pullRequestId)
        {
            Newtonsoft.Json.Linq.JArray list = null;
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/git/pull%20request%20commits/get%20pull%20request%20commits?view=azure-devops-rest-5.1
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version=5.1";
            string response = await GetAzureDevOpsMessage(url, patToken);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                list = buildListObject.value;
            }
            return list;
        }

        private async static Task<string> GetAzureDevOpsMessage(string url, string patToken)
        {
            string responseBody = "";
            if (url.IndexOf("dev.azure.com") == -1)
            {
                throw new Exception("dev.azure.com missing from URL");
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //If we use a pat token, we can access private repos
                if (string.IsNullOrEmpty(patToken) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", patToken))));
                }
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    //Throw a response exception
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine(responseBody);
                    }
                }
            }
            return responseBody;
        }

    }
}
