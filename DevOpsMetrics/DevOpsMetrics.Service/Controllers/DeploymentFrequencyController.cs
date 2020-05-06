using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentFrequencyController : ControllerBase
    {
        [HttpGet("GetAzDeployments")]
        public async Task<List<AzureDevOpsBuild>> GetAzDeployments(string patToken, string organization, string project, string AzureDevOpsbranch, string buildId)
        {
            AzureDevOpsDeploymentFrequencyDA da = new AzureDevOpsDeploymentFrequencyDA();
            List<AzureDevOpsBuild> deployments = await da.GetDeployments(patToken, organization, project, AzureDevOpsbranch, buildId);
            return deployments;
        }

        [HttpGet("GetAzDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzDeploymentFrequency(string patToken, string organization, string project, string AzureDevOpsbranch, string buildId, int numberOfDays)
        {
            AzureDevOpsDeploymentFrequencyDA da = new AzureDevOpsDeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetDeploymentFrequency(patToken, organization, project, AzureDevOpsbranch, buildId, numberOfDays);
            return model;
        }

        [HttpGet("GetGHDeployments")]
        public async Task<List<GitHubActionsRun>> GetGHDeployments(string owner, string repo, string GHbranch, string workflowId)
        {
            GitHubDeploymentFrequencyDA da = new GitHubDeploymentFrequencyDA();
            List<GitHubActionsRun> deployments = await da.GetDeployments(owner, repo, GHbranch, workflowId);
            return deployments;
        }

        [HttpGet("GetGHDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetGHDeploymentFrequency(string owner, string repo, string GHbranch, string workflowId, int numberOfDays)
        {
            GitHubDeploymentFrequencyDA da = new GitHubDeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetDeploymentFrequency(owner, repo, GHbranch, workflowId, numberOfDays);
            return model;
        }
    }
}