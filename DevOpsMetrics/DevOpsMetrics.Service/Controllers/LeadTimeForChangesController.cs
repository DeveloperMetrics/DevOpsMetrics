using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadTimeForChangesController : ControllerBase
    {

        [HttpGet("GetAzureDevOpsLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData, string patToken, string organization, string project, string repositoryId, string branch, string buildId, int numberOfDays)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel leadTimeForChanges = await da.GetAzureDevOpsLeadTimesForChanges(getSampleData, patToken, organization, project, repositoryId, branch, buildId, numberOfDays);
            return leadTimeForChanges;
        }

        [HttpGet("GetGitHubLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowId, int numberOfDays)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel leadTimeForChanges = await da.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, owner, repo, branch, workflowId, numberOfDays);
            return leadTimeForChanges;
        }

    }
}