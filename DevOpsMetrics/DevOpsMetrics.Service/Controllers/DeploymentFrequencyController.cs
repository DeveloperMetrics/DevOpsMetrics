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
        //[HttpGet("GetAzureDevOpsDeployments")]
        //public async Task<List<AzureDevOpsBuild>> GetAzureDevOpsDeployments(string patToken, string organization, string project, string branch, string buildId)
        //{
        //    BuildsDA da = new BuildsDA();
        //    List<AzureDevOpsBuild> deployments = await da.GetAzureDevOpsBuilds(patToken, organization, project, branch, buildId);
        //    return deployments;
        //}

        [HttpGet("GetAzureDevOpsDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays)
        {
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays);
            return model;
        }

        [HttpGet("GetGitHubDeployments")]
        public async Task<List<GitHubActionsRun>> GetGitHubDeployments(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string GHbranch, string workflowId)
        {
            BuildsDA da = new BuildsDA();
            List<GitHubActionsRun> deployments = await da.GetGitHubActionRuns(getSampleData, clientId, clientSecret, owner, repo, GHbranch, workflowId);
            return deployments;
        }

        [HttpGet("GetGitHubDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string GHbranch, string workflowId, int numberOfDays)
        {
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, GHbranch, workflowId, numberOfDays);
            return model;
        }
    }
}