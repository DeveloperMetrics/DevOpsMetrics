using DevOpsMetrics.Core;
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

        [HttpGet("GetAzLeadTimeForChanges")]
        public async Task<List<LeadTimeForChangesModel>> GetAzLeadTimeForChanges(string patToken, string organization, string project, string AZbranch, string buildId)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            List<LeadTimeForChangesModel> leadTimeForChanges = await da.GetAzureDevOpsLeadTimesForChanges(patToken, organization, project, AZbranch, buildId);
            return leadTimeForChanges;
        }

        [HttpGet("GetGitHubLeadTimeForChanges")]
        public async Task<List<LeadTimeForChangesModel>> GetGitHubLeadTimeForChanges(string owner, string repo, string GHbranch, string workflowId)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            List<LeadTimeForChangesModel> leadTimeForChanges = await da.GetGitHubLeadTimesForChanges(owner, repo, GHbranch, workflowId);
            return leadTimeForChanges;
        }

    }
}