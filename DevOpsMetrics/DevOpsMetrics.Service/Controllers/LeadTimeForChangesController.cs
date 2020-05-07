using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadTimeForChangesController : ControllerBase
    {

        [HttpGet("GetAzureDevOpsLeadTimeForChanges")]
        public async Task<List<LeadTimeForChangesModel>> GetAzureDevOpsLeadTimeForChanges(string patToken, string organization, string project, string branch, string buildId)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            List<LeadTimeForChangesModel> leadTimeForChanges = await da.GetAzureDevOpsLeadTimesForChanges(patToken, organization, project, branch, buildId);
            return leadTimeForChanges;
        }

        [HttpGet("GetGitHubLeadTimeForChanges")]
        public async Task<List<LeadTimeForChangesModel>> GetGitHubLeadTimeForChanges(string owner, string repo, string branch, string workflowId)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            List<LeadTimeForChangesModel> leadTimeForChanges = await da.GetGitHubLeadTimesForChanges(owner, repo, branch, workflowId);
            return leadTimeForChanges;
        }

    }
}