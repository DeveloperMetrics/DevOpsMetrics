using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class PullRequestModel
    {
        public string PullRequestId { get; set; }
        public string Branch { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration
        {
            get
            {
                return EndDateTime - StartDateTime;
            }
        }
        public int DurationPercent { get; set; } = 100;
        public List<Commit> Commits { get; set; }
        public int BuildCount { get; set; } //TODO: Should this actually be the list of builds?

        //public string ProjectName { get; set; }
        //public List<LeadTimeForChangesModel> AzureDevOpsList { get; set; }
        //public List<LeadTimeForChangesModel> GitHubList { get; set; }

        //public float AverageDuration { get; set; }
        //public float AverageDurationRating { get; set; }

    }
}
