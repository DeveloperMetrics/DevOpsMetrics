using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadTimeForChangesController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public LeadTimeForChangesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Get lead time for changes from Azure DevOps API
        [HttpGet("GetAzureDevOpsLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData,
            string organization, string project, string repository, string branch, string buildName,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            LeadTimeForChangesModel model = new();
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

                LeadTimeForChangesDA da = new();
                model = await LeadTimeForChangesDA.GetAzureDevOpsLeadTimesForChanges(getSampleData, patToken, tableStorageConfig,
                        organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.ProjectName = project;
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;
        }

        // Get lead time for changes from GitHub API
        [HttpGet("GetGitHubLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData,
            string owner, string repo, string branch, string workflowName, string workflowId,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            LeadTimeForChangesModel model = new();
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

                LeadTimeForChangesDA da = new();
                model = await LeadTimeForChangesDA.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, tableStorageConfig,
                        owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.ProjectName = repo;
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;

        }
    }
}