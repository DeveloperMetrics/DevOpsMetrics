using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DevOpsMetrics.Function
{
    public class ServiceApiClient
    {
        private readonly IConfiguration Configuration;
        private readonly HttpClient Client;

        public ServiceApiClient(IConfiguration configuration)
        {
            Configuration = configuration;
            Client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"])
            };
        }

        public async Task<List<AzureDevOpsSettings>> GetAzureDevOpsSettings()
        {
            string url = $"/api/Settings/GetAzureDevOpsSettings";
            return await GetResponse<List<AzureDevOpsSettings>>(Client, url);
        }

        public async Task<List<GitHubSettings>> GetGitHubSettings()
        {
            string url = $"/api/Settings/GetGitHubSettings";
            return await GetResponse<List<GitHubSettings>>(Client, url);
        }

        public async Task<ProcessingResult> UpdateDORASummaryItem(
            string owner, string project, string repository,
            string branch, string workflowName, string workflowId,
            string resourceGroup, int numberOfDays, int maxNumberOfItems,
            bool isGitHub = true)
        {
            string url = $"/api/DORASummary/UpdateDORASummaryItem?owner={owner}&project={project}&repository={repository}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&resourceGroup={resourceGroup}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&log=&useCache=true&isGitHub={isGitHub}";
            return await GetResponse<ProcessingResult>(Client, url);
        }

        public async Task<bool> UpdateDevOpsMonitoringEvent(MonitoringEvent monitoringEvent)
        {
            string url = $"/api/Settings/UpdateDevOpsMonitoringEvent";
            return await PostResponse(Client, url, monitoringEvent);
        }

        private async Task<T> GetResponse<T>(HttpClient client, string url)
        {
            T obj = default;
            if (client != null && url != null)
            {
                Debug.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    //TODO: Add a flag in the service to turn this on and off.
                    if (response.IsSuccessStatusCode == false)
                    {
                        //Log the URL to help with debugging
                    }
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody) == false)
                    {
                        obj = JsonConvert.DeserializeObject<T>(responseBody);
                    }
                }
            }
            return obj;
        }

        private async Task<bool> PostResponse(HttpClient client, string url, MonitoringEvent monitoringEvent)
        {

            if (client != null && url != null)
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(monitoringEvent), Encoding.UTF8, "application/json");

                Debug.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
                using (HttpResponseMessage response = await client.PostAsync(url, content))
                {
                    response.EnsureSuccessStatusCode();
                    //string responseBody = await response.Content.ReadAsStringAsync();
                    //if (string.IsNullOrEmpty(responseBody) == false)
                    //{
                    //    obj = JsonConvert.DeserializeObject<T>(responseBody);
                    //}
                }
            }
            return true;
        }
    }
}
