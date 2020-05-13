using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentFrequencyController : ControllerBase
    {
        [HttpGet("GetAzureDevOpsDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems = 20)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;
        }

        [HttpGet("GetGitHubDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems = 20)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
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