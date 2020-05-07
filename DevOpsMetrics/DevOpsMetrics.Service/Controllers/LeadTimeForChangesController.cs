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
            AzureDevOpsLeadTimeForChangesDA da = new AzureDevOpsLeadTimeForChangesDA();
            List<LeadTimeForChangesModel> leadTimeForChanges = await da.GetLeadTimesForChanges(patToken, organization, project, AZbranch, buildId);
            return leadTimeForChanges;
        }

        [HttpGet("GetGHLeadTimeForChanges")]
        public async Task<List<LeadTimeForChangesModel>> GetGHLeadTimeForChanges(string owner, string repo, string GHbranch, string workflowId)
        {
            GitHubLeadTimeForChangesDA da = new GitHubLeadTimeForChangesDA();
            List<LeadTimeForChangesModel> leadTimeForChanges = await da.GetLeadTimesForChanges(owner, repo, GHbranch, workflowId);
            return leadTimeForChanges;
        }

    }
}