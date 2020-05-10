using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
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
        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData, string patToken, string organization, string project, string repositoryId, string branch, string buildId)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel leadTimeForChanges = await da.GetAzureDevOpsLeadTimesForChanges(getSampleData, patToken, organization, project, repositoryId, branch, buildId);

            //TimeSpan largestDuration = TimeSpan.Zero;
            //double totalHours = 0;
            //foreach (LeadTimeForChangesModel item in leadTimeForChanges)
            //{
            //    if (item.Duration > largestDuration)
            //    {
            //        largestDuration = item.Duration;
            //    }
            //    //sum up the total duration
            //    totalHours += item.Duration.TotalHours;
            //}
            ////Loop one more time to scale the durations into a range of 20-100 percent
            //foreach (LeadTimeForChangesModel item in leadTimeForChanges)
            //{
            //    item.DurationPercent = Utility.ScaleNumberToRange((float)item.Duration.TotalHours, 0, (float)largestDuration.TotalHours, 20, 100);
            //}

            //newItem = new LeadTimeForChangesPartialViewModel
            //{
            //    ProjectName = project,
            //    AzureDevOpsList = leadTimeForChanges
            //};
            //newItem.AverageDuration = (float)totalHours / (float)leadTimeForChanges.Count;
            //newItem.AverageDurationRating = GetLeadTimeForChangesRating(newItem.AverageDuration);

            return leadTimeForChanges;
        }

        [HttpGet("GetGitHubLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData, string clientId, string clientSecret ,string owner, string repo, string branch, string workflowId)
        {
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel leadTimeForChanges = await da.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, owner, repo, branch, workflowId);
            return leadTimeForChanges;
        }

    }
}