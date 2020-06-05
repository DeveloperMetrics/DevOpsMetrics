using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableStorageController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public TableStorageController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Get builds from the Azure DevOps API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="patToken"></param>
        /// <param name="organization"></param>
        /// <param name="project"></param>
        /// <param name="branch"></param>
        /// <param name="buildName"></param>
        /// <param name="buildId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("GetAzureDevOpsDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, TableStorageAuth tableStorageAuth, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, tableStorageAuth, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.DeploymentName = buildName;
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;
        }

        /// <summary>
        /// Get builds from the Azure DevOps API, and save new records to the storage table
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="patToken"></param>
        /// <param name="organization"></param>
        /// <param name="project"></param>
        /// <param name="branch"></param>
        /// <param name="buildName"></param>
        /// <param name="buildId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("UpdateAzureDevOpsBuilds")]
        public async Task<int> UpdateAzureDevOpsBuilds(string patToken,
                string organization, string project, string branch, string buildName, string buildId,
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateAzureDevOpsBuilds(patToken, tableStorageAuth, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

        /// <summary>
        /// Get builds from the GitHub API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <param name="branch"></param>
        /// <param name="workflowName"></param>
        /// <param name="workflowId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("UpdateGitHubActionRuns")]
        public async Task<int> UpdateGitHubActionRuns(string clientId, string clientSecret,
                string owner, string repo, string branch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateGitHubActionRuns(clientId, clientSecret, tableStorageAuth,
                        owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

        [HttpGet("UpdateAzureDevOpsPullRequests")]
        public async Task<int> UpdateAzureDevOpsPullRequests(string patToken,
               string organization, string project, string repositoryId,
               int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateAzureDevOpsPullRequests(patToken, tableStorageAuth,
                        organization, project, repositoryId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

        [HttpGet("UpdateGitHubActionPullRequests")]
        public async Task<int> UpdateGitHubActionPullRequests(string clientId, string clientSecret,
                string owner, string repo, string branch, 
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateGitHubActionPullRequests(clientId, clientSecret, tableStorageAuth,
                        owner, repo, branch, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

        [HttpGet("UpdateAzureDevOpsPullRequestCommits")]
        public async Task<int> UpdateAzureDevOpsPullRequestCommits(string patToken,
               string organization, string project, string repositoryId, string pullRequestId,
               int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateAzureDevOpsPullRequestCommits(patToken, tableStorageAuth,
                    organization, project, repositoryId, pullRequestId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

        [HttpGet("UpdateGitHubActionPullRequestCommits")]
        public async Task<int> UpdateGitHubActionPullRequestCommits(string clientId, string clientSecret,
                string owner, string repo, string pull_number)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateGitHubActionPullRequestCommits(clientId, clientSecret, tableStorageAuth,
                        owner, repo, pull_number);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

        [HttpGet("GetAzureDevOpsSettings")]
        public List<AzureDevOpsSettings> GetAzureDevOpsSettings()
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<AzureDevOpsSettings> settings = da.GetAzureDevOpsSettings(tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings);

            return settings;
        }

        [HttpGet("GetGitHubSettings")]
        public List<GitHubSettings> GetGitHubSettings()
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<GitHubSettings> settings = da.GetGitHubSettings(tableStorageAuth, tableStorageAuth.TableGitHubSettings);
            return settings;
        }

        [HttpGet("UpdateAzureDevOpsSetting")]
        public async Task<bool> UpdateAzureDevOpsSetting(string patToken,
                string organization, string project, string repository, string branch, string buildName, string buildId, string resourceGroup, int itemOrder)
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            AzureTableStorageDA da = new AzureTableStorageDA();
            return await da.UpdateAzureDevOpsSetting(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings,
                    organization, project, repository, branch, buildName, buildId, resourceGroup, itemOrder);
        }

        [HttpGet("UpdateGitHubSetting")]
        public async Task<bool> UpdateGitHubSetting(string clientId, string clientSecret,
                string owner, string repo, string branch, string workflowName, string workflowId, string resourceGroup, int itemOrder)
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            AzureTableStorageDA da = new AzureTableStorageDA();
            return await da.UpdateGitHubSetting(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroup, itemOrder);
        }

        [HttpPost("UpdateDevOpsMonitoringEvent")]
        public async Task<bool> UpdateDevOpsMonitoringEvent([FromBody] MonitoringEvent monitoringEvent)
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            AzureTableStorageDA da = new AzureTableStorageDA();
            return await da.UpdateDevOpsMonitoringEvent(tableStorageAuth, monitoringEvent);
        }

    }
}