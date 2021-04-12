using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Web.Services
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

        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/DeploymentFrequency/GetAzureDevOpsDeploymentFrequency?getSampleData={getSampleData}&organization={organization}&project={project}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            return await GetResponse<DeploymentFrequencyModel>(Client, url);
        }

        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/DeploymentFrequency/GetGitHubDeploymentFrequency?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            return await GetResponse<DeploymentFrequencyModel>(Client, url);
        }

        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData, string organization, string project, string repository, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/LeadTimeForChanges/GetAzureDevOpsLeadTimeForChanges?getSampleData={getSampleData}&organization={organization}&project={project}&repository={repository}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            return await GetResponse<LeadTimeForChangesModel>(Client, url);
        }

        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            return await GetResponse<LeadTimeForChangesModel>(Client, url);
        }

        public async Task<MeanTimeToRestoreModel> GetAzureMeanTimeToRestore(bool getSampleData, DevOpsPlatform targetDevOpsPlatform, string resourceGroup, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/MeanTimeToRestore/GetAzureMeanTimeToRestore?getSampleData={getSampleData}&targetDevOpsPlatform={targetDevOpsPlatform}&resourceGroup={resourceGroup}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<MeanTimeToRestoreModel>(Client, url);
        }

        public async Task<ChangeFailureRateModel> GetChangeFailureRate(bool getSampleData, DevOpsPlatform targetDevOpsPlatform, string organization_owner, string project_repo, string branch, string buildName_workflowName,  int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/ChangeFailureRate/GetChangeFailureRate?getSampleData={getSampleData}&targetDevOpsPlatform={targetDevOpsPlatform}&organization_owner={organization_owner}&project_repo={project_repo}&branch={branch}&buildName_workflowName={buildName_workflowName}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            return await GetResponse<ChangeFailureRateModel>(Client, url);
        }

        public async Task<bool> UpdateChangeFailureRate(string organization_owner, string project_repo, string buildName_workflowName, int percentComplete, int numberOfDays)
        {
            string url = $"/api/ChangeFailureRate/UpdateChangeFailureRate?organization_owner={organization_owner}&project_repo={project_repo}&buildName_workflowName={buildName_workflowName}&percentComplete={percentComplete}&numberOfDays={numberOfDays}";
            return await GetResponse<bool>(Client, url);
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

        private static async Task<T> GetResponse<T>(HttpClient client, string url)
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
