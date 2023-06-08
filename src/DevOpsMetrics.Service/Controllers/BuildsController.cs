using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly IAzureTableStorageDA AzureTableStorageDA;

        public BuildsController(IConfiguration configuration, IAzureTableStorageDA azureTableStorageDA)
        {
            Configuration = configuration;
            AzureTableStorageDA = azureTableStorageDA;
        }

        // Get builds from the Azure DevOps API, and save new records to the storage table
        [HttpGet("UpdateAzureDevOpsBuilds")]
        public async Task<int> UpdateAzureDevOpsBuilds(
                string organization, string project, string repository, string branch,
                string buildName, string buildId,
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the PAT token from the key vault
                string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(organization, project, repository);
                string patToken = Configuration[patTokenName];
                //if (string.IsNullOrEmpty(patToken))
                //{
                //    throw new Exception($"patToken '{patTokenName}' not found in key vault");
                //}

                numberOfRecordsSaved = await AzureTableStorageDA.UpdateAzureDevOpsBuildsInStorage(patToken, tableStorageConfig, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);
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

        // Get builds from the GitHub API
        [HttpGet("UpdateGitHubActionRuns")]
        public async Task<int> UpdateGitHubActionRuns(
                string owner, string repo, string branch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved;
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the client id and secret from the settings
                string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
                string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
                string clientId = Configuration[clientIdName];
                string clientSecret = Configuration[clientSecretName];
                //if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                //{
                //    throw new Exception($"clientId '{clientId}' or clientSecret '{clientSecret}' not found in key vault");
                //}

                numberOfRecordsSaved = await AzureTableStorageDA.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
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

    }
}