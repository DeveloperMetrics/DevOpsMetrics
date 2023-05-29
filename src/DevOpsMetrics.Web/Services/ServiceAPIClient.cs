using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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

        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string organization, string project, string repository, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/DeploymentFrequency/GetAzureDevOpsDeploymentFrequency?getSampleData={getSampleData}&organization={organization}&project={project}&repository={repository}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            try
            {
                return await GetResponse<DeploymentFrequencyModel>(Client, url);
            }
            catch (Exception ex)
            {
                return new DeploymentFrequencyModel
                {
                    DeploymentName = buildName,
                    TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                    DeploymentsPerDayMetricDescription = "None",
                    Exception = ex,
                    ExceptionUrl = url
                };
            }
        }

        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/DeploymentFrequency/GetGitHubDeploymentFrequency?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            try
            {
                return await GetResponse<DeploymentFrequencyModel>(Client, url);
            }
            catch (Exception ex)
            {
                return new DeploymentFrequencyModel
                {

                    DeploymentName = workflowName,
                    TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                    DeploymentsPerDayMetricDescription = "None",
                    Exception = ex,
                    ExceptionUrl = url
                };
            }
        }

        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData, string organization, string project, string repository, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/LeadTimeForChanges/GetAzureDevOpsLeadTimeForChanges?getSampleData={getSampleData}&organization={organization}&project={project}&repository={repository}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            try
            {
                return await GetResponse<LeadTimeForChangesModel>(Client, url);
            }
            catch (Exception ex)
            {
                return new LeadTimeForChangesModel
                {
                    ProjectName = project,
                    TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                    LeadTimeForChangesMetricDescription = "None",
                    Exception = ex,
                    ExceptionUrl = url
                };
            }
        }

        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            string url = $"/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&useCache={useCache}";
            try
            {
                return await GetResponse<LeadTimeForChangesModel>(Client, url);
            }
            catch (Exception ex)
            {
                return new LeadTimeForChangesModel
                {
                    ProjectName = repo,
                    TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                    LeadTimeForChangesMetricDescription = "None",
                    Exception = ex,
                    ExceptionUrl = url
                };
            }
        }

        public async Task<MeanTimeToRestoreModel> GetAzureMeanTimeToRestore(bool getSampleData, DevOpsPlatform targetDevOpsPlatform, string resourceGroup, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/MeanTimeToRestore/GetAzureMeanTimeToRestore?getSampleData={getSampleData}&targetDevOpsPlatform={targetDevOpsPlatform}&resourceGroup={resourceGroup}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            try
            {
                return await GetResponse<MeanTimeToRestoreModel>(Client, url);
            }
            catch (Exception ex)
            {
                return new MeanTimeToRestoreModel
                {
                    ResourceGroup = resourceGroup,
                    TargetDevOpsPlatform = targetDevOpsPlatform,
                    MTTRAverageDurationDescription = "None",
                    Exception = ex,
                    ExceptionUrl = url
                };
            }
        }

        public async Task<ChangeFailureRateModel> GetChangeFailureRate(bool getSampleData, DevOpsPlatform targetDevOpsPlatform, string organization_owner, string project_repo, string branch, string buildName_workflowName, int numberOfDays, int maxNumberOfItems)
        {
            string url = $"/api/ChangeFailureRate/GetChangeFailureRate?getSampleData={getSampleData}&targetDevOpsPlatform={targetDevOpsPlatform}&organization_owner={organization_owner}&project_repo={project_repo}&branch={branch}&buildName_workflowName={buildName_workflowName}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            try
            {
                return await GetResponse<ChangeFailureRateModel>(Client, url);
            }
            catch (Exception ex)
            {
                return new ChangeFailureRateModel
                {
                    DeploymentName = buildName_workflowName,
                    TargetDevOpsPlatform = targetDevOpsPlatform,
                    ChangeFailureRateMetricDescription = "None",
                    Exception = ex,
                    ExceptionUrl = url
                };
            }
        }

        public async Task<bool> UpdateChangeFailureRate(string organization_owner, string project_repo, string buildName_workflowName, int percentComplete, int numberOfDays)
        {
            string url = $"/api/ChangeFailureRate/UpdateChangeFailureRate?organization_owner={organization_owner}&project_repo={project_repo}&buildName_workflowName={buildName_workflowName}&percentComplete={percentComplete}&numberOfDays={numberOfDays}";
            return await GetResponse<bool>(Client, url);
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

        public async Task<bool> UpdateAzureDevOpsSetting(string patToken,
                string organization, string project, string repository,
                string branch, string buildName, string buildId, string resourceGroup,
                int itemOrder, bool showSetting)
        {
            string url = $"/api/Settings/UpdateAzureDevOpsSetting?patToken={patToken}&organization={organization}&project={project}&repository={repository}&branch={branch}&buildName={buildName}&buildId={buildId}&resourceGroup={resourceGroup}&itemOrder={itemOrder}&showSetting={showSetting}";

            return await GetResponse<bool>(Client, url);
        }

        public async Task<bool> UpdateGitHubSetting(string clientId, string clientSecret,
            string owner, string repo,
            string branch, string workflowName, string workflowId, string resourceGroup,
            int itemOrder, bool showSetting)
        {
            string url = $"/api/Settings/UpdateGitHubSetting?clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&resourceGroup={resourceGroup}&itemOrder={itemOrder}&showSetting={showSetting}";

            return await GetResponse<bool>(Client, url);
        }

        public async Task<List<ProjectLog>> GetAzureDevOpsProjectLogs(string organization, string project, string repository)
        {
            string url = $"/api/Settings/GetAzureDevOpsProjectLog?organization={organization}&project={project}&repository={repository}";
            return await GetResponse<List<ProjectLog>>(Client, url);
        }

        public async Task<List<ProjectLog>> GetGitHubProjectLogs(string owner, string repo)
        {
            string url = $"/api/Settings/GetGitHubProjectLog?owner={owner}&repo={repo}";
            return await GetResponse<List<ProjectLog>>(Client, url);
        }

        public async Task<ProcessingResult> UpdateDORASummaryItem(
            string owner, string project, string repository,
            string branch, string workflowName, string workflowId,
            string resourceGroup, int numberOfDays, int maxNumberOfItems,
            bool isGitHub = true)
        {
            string url = $"/api/DORASummary/UpdateDORASummaryItem?owner={owner}&project={project}&repo={repository}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&resourceGroup={resourceGroup}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}&log=&useCache=true&isGitHub={isGitHub}";
            return await GetResponse<ProcessingResult>(Client, url);
        }

        private static async Task<T> GetResponse<T>(HttpClient client, string url)
        {
            T obj = default;
            if (client != null && url != null)
            {
                Debug.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
                Console.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseBody))
                        {
                            obj = JsonConvert.DeserializeObject<T>(responseBody);
                        }
                    }
                    else
                    {
                        //Throw an exception
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            return obj;
        }
    }
}
