//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;
//using DevOpsMetrics.Core.Models.AzureDevOps;
//using DevOpsMetrics.Core.Models.Common;
//using DevOpsMetrics.Core.Models.GitHub;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;

//namespace DevOpsMetrics.Function
//{
//    public class ServiceApiClient
//    {
//        private readonly IConfiguration Configuration;
//        private readonly HttpClient Client;

//        public ServiceApiClient(IConfiguration configuration)
//        {
//            Configuration = configuration;
//            Client = new HttpClient
//            {
//                BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"])
//            };
//        }

//        public async Task<List<AzureDevOpsSettings>> GetAzureDevOpsSettings()
//        {
//            string url = $"/api/Settings/GetAzureDevOpsSettings";
//            return await GetResponse<List<AzureDevOpsSettings>>(Client, url);
//        }

//        public async Task<List<GitHubSettings>> GetGitHubSettings()
//        {
//            string url = $"/api/Settings/GetGitHubSettings";
//            return await GetResponse<List<GitHubSettings>>(Client, url);
//        }

//        public async Task<(int, string)> UpdateAzureDevOpsBuilds(string organization, string project, string repository, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
//        {
//            string url = $"/api/Builds/UpdateAzureDevOpsBuilds?organization={organization}&project={project}&repository={repository}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
//            return (await GetResponse<int>(Client, url), url);
//        }

//        public async Task<(int, string)> UpdateGitHubActionRuns(string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
//        {
//            string url = $"/api/Builds/UpdateGitHubActionRuns?owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
//            return (await GetResponse<int>(Client, url), url);
//        }

//        public async Task<(int, string)> UpdateAzureDevOpsPullRequests(string organization, string project, string repository, int numberOfDays, int maxNumberOfItems)
//        {
//            string url = $"/api/PullRequests/UpdateAzureDevOpsPullRequests?organization={organization}&project={project}&repository={repository}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
//            return (await GetResponse<int>(Client, url), url);
//        }

//        public async Task<(int, string)> UpdateGitHubActionPullRequests(string owner, string repo, string branch, int numberOfDays, int maxNumberOfItems)
//        {
//            string url = $"/api/PullRequests/UpdateGitHubActionPullRequests?owner={owner}&repo={repo}&branch={branch}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
//            return (await GetResponse<int>(Client, url), url);
//        }

//        public async Task<bool> UpdateDevOpsMonitoringEvent(string requestBody)
//        {
//            MonitoringEvent monitoringEvent = new MonitoringEvent(requestBody);
//            string url = $"/api/Settings/UpdateDevOpsMonitoringEvent";
//            return await PostResponse(Client, url, monitoringEvent);
//        }

//        public async Task<bool> UpdateAzureDevOpsProjectLog(string organization, string project, string repository, int buildsUpdated, int prsUpdated, string buildUrl, string prUrl, string exceptionMessage, string exceptionStackTrace)
//        {
//            string url = $"/api/Settings/UpdateAzureDevOpsProjectLog?organization={organization}&project={project}&repository={repository}&buildsUpdated={buildsUpdated}&prsUpdated={prsUpdated}&buildsUpdated={buildsUpdated}&prsUpdated={prsUpdated}&buildUrl={HttpUtility.UrlEncode(buildUrl)}&prUrl={HttpUtility.UrlEncode(prUrl)}&exceptionMessage={exceptionMessage}&exceptionStackTrace={exceptionStackTrace}";
//            return await GetResponse<bool>(Client, url);
//        }

//        public async Task<bool> UpdateGitHubProjectLog(string owner, string repo, int buildsUpdated, int prsUpdated, string buildUrl, string prUrl, string exceptionMessage, string exceptionStackTrace)
//        {
//            string url = $"/api/Settings/UpdateGitHubProjectLog?owner={owner}&repo={repo}&buildsUpdated={buildsUpdated}&prsUpdated={prsUpdated}&buildUrl={HttpUtility.UrlEncode(buildUrl)}&prUrl={HttpUtility.UrlEncode(prUrl)}&exceptionMessage={exceptionMessage}&exceptionStackTrace={exceptionStackTrace}";
//            return await GetResponse<bool>(Client, url);
//        }

//        private async Task<T> GetResponse<T>(HttpClient client, string url)
//        {
//            T obj = default;
//            if (client != null && url != null)
//            {
//                Debug.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
//                using (HttpResponseMessage response = await client.GetAsync(url))
//                {
//                    //TODO: Add a flag in the service to turn this on and off.
//                    if (response.IsSuccessStatusCode == false)
//                    {
//                        //Log the URL to help with debugging
//                    }
//                    response.EnsureSuccessStatusCode();
//                    string responseBody = await response.Content.ReadAsStringAsync();
//                    if (string.IsNullOrEmpty(responseBody) == false)
//                    {
//                        obj = JsonConvert.DeserializeObject<T>(responseBody);
//                    }
//                }
//            }
//            return obj;
//        }

//        private async Task<bool> PostResponse(HttpClient client, string url, MonitoringEvent monitoringEvent)
//        {

//            if (client != null && url != null)
//            {
//                StringContent content = new StringContent(JsonConvert.SerializeObject(monitoringEvent), Encoding.UTF8, "application/json");

//                Debug.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
//                using (HttpResponseMessage response = await client.PostAsync(url, content))
//                {
//                    response.EnsureSuccessStatusCode();
//                    //string responseBody = await response.Content.ReadAsStringAsync();
//                    //if (string.IsNullOrEmpty(responseBody) == false)
//                    //{
//                    //    obj = JsonConvert.DeserializeObject<T>(responseBody);
//                    //}
//                }
//            }
//            return true;
//        }
//    }
//}
