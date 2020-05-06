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
        //How will this work?
        //We will look at pipelines, noting when a pipelines with a branch exist, looking at that lifecycle, and comparing it to commits with that branch. 
        //This could be complicated by branch deletions - common with Pull Request merges


        //[HttpGet("GetAzLeadTimeForChanges")]
        //public async Task<List<AzureDevOpsBuild>> GetAzLeadTimeForChanges(string patToken, string organization, string project, string AzureDevOpsbranch, string buildId)
        //{
        //    AzureDevOpsLeadTimeForChangesDA da = new AzureDevOpsLeadTimeForChangesDA();
        //    List<AzureDevOpsBuild> leadTimeForChanges = await da.GetDeployments(patToken, organization, project, AzureDevOpsbranch, buildId);
        //    return deployments;
        //}

        //[HttpGet("GetGHLeadTimeForChanges")]
        //public async Task<List<GitHubActionsRun>> GetGHLeadTimeForChanges(string owner, string repo, string GHbranch, string workflowId)
        //{
        //    GitHubLeadTimeForChangesDA da = new GitHubLeadTimeForChangesDA();
        //    List<GitHubActionsRun> leadTimeForChanges = await da.GetDeployments(owner, repo, GHbranch, workflowId);
        //    return deployments;
        //}

    }
}