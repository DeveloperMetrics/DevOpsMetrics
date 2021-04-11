using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
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
            string organization, string project, string branch, string buildName,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, tableStorageConfig, organization, project, branch, buildName, numberOfDays, maxNumberOfItems, useCache);
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
        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret,
            string owner, string repo, string branch, string workflowName, string workflowId,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
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