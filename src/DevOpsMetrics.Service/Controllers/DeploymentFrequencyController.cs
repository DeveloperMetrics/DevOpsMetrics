using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentFrequencyController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DeploymentFrequencyController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Get builds from the Azure DevOps API
        [HttpGet("GetAzureDevOpsDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData,
            string organization, string project, string repository, string branch, string buildName,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the PAT token from the key vault
                string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(organization, project, repository);
                string patToken = Configuration[patTokenName];
                if (string.IsNullOrEmpty(patToken) == true)
                {
                    throw new Exception($"patToken '{patTokenName}' not found in key vault");
                }

                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, tableStorageConfig, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);
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

        // Get builds from the GitHub API
        [HttpGet("GetGitHubDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData,
            string owner, string repo, string branch, string workflowName, string workflowId,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

                //Get the client id and secret from the settings
                string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
                string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
                string clientId = Configuration[clientIdName];
                string clientSecret = Configuration[clientSecretName];
                if (string.IsNullOrEmpty(clientId) == true | string.IsNullOrEmpty(clientSecret) == true)
                {
                    throw new Exception($"clientId '{clientId}' or clientSecret '{clientSecret}' not found in key vault");
                }

                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, tableStorageConfig, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.DeploymentName = workflowName;
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