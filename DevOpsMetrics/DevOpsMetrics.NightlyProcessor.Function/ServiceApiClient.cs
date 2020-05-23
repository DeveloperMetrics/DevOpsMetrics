using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsMetrics.NightlyProcessor.Function
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
            string url = $"/api/TableStorage/GetAzureDevOpsSettings";
            return await GetResponse<List<AzureDevOpsSettings>>(Client, url);
        }

        public async Task<List<GitHubSettings>> GetGitHubSettings()
        {
            string url = $"/api/TableStorage/GetGitHubSettings";
            return await GetResponse<List<GitHubSettings>>(Client, url);
        }

        public async Task<int> UpdateAzureDevOpsBuilds(string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/TableStorage/UpdateAzureDevOpsBuilds?patToken={patToken}&organization={organization}&project={project}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<int>(Client, url);
        }

        public async Task<int> UpdateGitHubActionRuns(string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/TableStorage/UpdateGitHubActionRuns?clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<int>(Client, url);
        }

        public async Task<int> UpdateAzureDevOpsPullRequests(string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/TableStorage/UpdateAzureDevOpsBuilds?patToken={patToken}&organization={organization}&project={project}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<int>(Client, url);
        }

        public async Task<int> UpdateGitHubActionPullRequests(string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/TableStorage/UpdateGitHubActionRuns?clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<int>(Client, url);
        }

        public async Task<int> UpdateAzureDevOpsPullRequestCommits(string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/TableStorage/UpdateAzureDevOpsBuilds?patToken={patToken}&organization={organization}&project={project}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<int>(Client, url);
        }

        public async Task<int> UpdateGitHubActionPullRequestCommits(string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/TableStorage/UpdateGitHubActionRuns?clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<int>(Client, url);
        }

        private async Task<T> GetResponse<T>(HttpClient client, string url)
        {
            T obj = default;
            if (client != null && url != null)
            {
                Debug.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
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
    }
}
