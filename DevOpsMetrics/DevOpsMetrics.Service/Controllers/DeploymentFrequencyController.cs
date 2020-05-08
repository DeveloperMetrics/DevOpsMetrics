using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentFrequencyController : ControllerBase
    {
        [HttpGet("GetAzureDevOpsDeployments")]
        public async Task<List<AzureDevOpsBuild>> GetAzureDevOpsDeployments(string patToken, string organization, string project, string AzureDevOpsbranch, string buildId)
        {
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            List<AzureDevOpsBuild> deployments = await da.GetAzureDevOpsDeployments(patToken, organization, project, AzureDevOpsbranch, buildId);
            return deployments;
        }

        [HttpGet("GetAzureDevOpsDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(string patToken, string organization, string project, string AzureDevOpsbranch, string buildId, int numberOfDays)
        {
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetAzureDevOpsDeploymentFrequency(patToken, organization, project, AzureDevOpsbranch, buildId, numberOfDays);
            return model;
        }

        [HttpGet("GetGitHubDeployments")]
        public async Task<List<GitHubActionsRun>> GetGitHubDeployments(string clientId, string clientSecret, string owner, string repo, string GHbranch, string workflowId)
        {
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            List<GitHubActionsRun> deployments = await da.GetGitHubDeployments(clientId, clientSecret, owner, repo, GHbranch, workflowId);
            return deployments;
        }

        [HttpGet("GetGitHubDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(string clientId, string clientSecret, string owner, string repo, string GHbranch, string workflowId, int numberOfDays)
        {
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetGitHubDeploymentFrequency(clientId, clientSecret, owner, repo, GHbranch, workflowId, numberOfDays);
            return model;
        }
    }
}