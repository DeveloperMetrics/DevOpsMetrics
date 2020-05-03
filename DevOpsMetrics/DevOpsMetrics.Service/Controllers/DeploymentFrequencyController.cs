using DevOpsMetrics.Service.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentFrequencyController : ControllerBase
    {
        [HttpGet("GetAzDeploymentFrequency")]
        public async Task<float> GetAzDeploymentFrequency(string patToken, string organization, string project, string AzureDevOpsbranch, string buildId, int numberOfDays)
        {
            AzureDevOpsDeploymentFrequencyDA da = new AzureDevOpsDeploymentFrequencyDA();
            float deploymentFrequencyResult = await da.GetDeploymentFrequency(patToken, organization, project, AzureDevOpsbranch, buildId, numberOfDays);
            return deploymentFrequencyResult;
        }

        [HttpGet("GetGHDeploymentFrequency")]
        public async Task<float> GetGHDeploymentFrequency(string owner, string repo, string GHbranch, string workflowId, int numberOfDays)
        {
            GitHubDeploymentFrequencyDA da = new GitHubDeploymentFrequencyDA();
            float deploymentFrequencyResult = await da.GetDeploymentFrequency(owner, repo, GHbranch, workflowId, numberOfDays);
            return deploymentFrequencyResult;
        }
    }
}