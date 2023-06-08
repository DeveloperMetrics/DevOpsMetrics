using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PullRequestsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly IAzureTableStorageDA AzureTableStorageDA;

        public PullRequestsController(IConfiguration configuration, IAzureTableStorageDA azureTableStorageDA)
        {
            Configuration = configuration;
            AzureTableStorageDA = azureTableStorageDA;
        }

        [HttpGet("UpdateAzureDevOpsPullRequests")]
        public async Task<int> UpdateAzureDevOpsPullRequests(
         string organization, string project, string repository,
         int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the PAT token from the key vault
                string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(organization, project, repository);
                patTokenName = SecretsProcessing.CleanKey(patTokenName);
                string patToken = Configuration[patTokenName];
                //if (string.IsNullOrEmpty(patToken))
                //{
                //    throw new Exception($"patToken '{patTokenName}' not found in key vault");
                //}

                numberOfRecordsSaved = await AzureTableStorageDA.UpdateAzureDevOpsPullRequestsInStorage(patToken, tableStorageConfig,
                         organization, project, repository, numberOfDays, maxNumberOfItems);
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
        public async Task<int> UpdateGitHubActionPullRequests(
                string owner, string repo, string branch,
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the client id and secret from the settings
                string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
                clientIdName = SecretsProcessing.CleanKey(clientIdName);
                string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
                clientSecretName = SecretsProcessing.CleanKey(clientSecretName);
                string clientId = Configuration[clientIdName];
                string clientSecret = Configuration[clientSecretName];
                //if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                //{
                //    throw new Exception($"clientId '{clientId}' or clientSecret '{clientSecret}' not found in key vault");
                //}

                numberOfRecordsSaved = await AzureTableStorageDA.UpdateGitHubActionPullRequestsInStorage(clientId, clientSecret, tableStorageConfig,
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
        public async Task<int> UpdateAzureDevOpsPullRequestCommits(
               string organization, string project, string repository, string pullRequestId,
               int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the PAT token from the key vault
                string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(organization, project, repository);
                patTokenName = SecretsProcessing.CleanKey(patTokenName);
                string patToken = Configuration[patTokenName];
                //if (string.IsNullOrEmpty(patToken))
                //{
                //    throw new Exception($"patToken '{patTokenName}' not found in key vault");
                //}

                numberOfRecordsSaved = await AzureTableStorageDA.UpdateAzureDevOpsPullRequestCommitsInStorage(patToken, tableStorageConfig,
                    organization, project, repository, pullRequestId, numberOfDays, maxNumberOfItems);
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
        public async Task<int> UpdateGitHubActionPullRequestCommits(
                string owner, string repo, string pull_number)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the client id and secret from the settings
                string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
                clientIdName = SecretsProcessing.CleanKey(clientIdName);
                string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
                clientSecretName = SecretsProcessing.CleanKey(clientSecretName);
                string clientId = Configuration[clientIdName];
                string clientSecret = Configuration[clientSecretName];
                //if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                //{
                //    throw new Exception($"clientId '{clientId}' or clientSecret '{clientSecret}' not found in key vault");
                //}

                numberOfRecordsSaved = await AzureTableStorageDA.UpdateGitHubActionPullRequestCommitsInStorage(clientId, clientSecret, tableStorageConfig,
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

    }
}