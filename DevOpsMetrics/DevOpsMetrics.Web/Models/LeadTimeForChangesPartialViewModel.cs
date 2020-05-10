using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.Common;
using System.Collections.Generic;

namespace DevOpsMetrics.Web.Models
{
    public class LeadTimeForChangesPartialViewModel
    {
        public string ProjectName { get; set; }
        public List<PullRequestModel> AzureDevOpsList { get; set; }
        public List<PullRequestModel> GitHubList { get; set; }

        public float AverageDuration { get; set; }
        public float AverageDurationRating { get; set; }
    }
}
