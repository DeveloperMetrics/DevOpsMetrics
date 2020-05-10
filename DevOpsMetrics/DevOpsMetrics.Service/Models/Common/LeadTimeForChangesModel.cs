using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class LeadTimeForChangesModel
    {
        public string ProjectName { get; set; }
        public bool IsAzureDevOps { get; set; }
        public List<PullRequestModel> PullRequests { get; set; }

        public float AverageDuration { get; set; }
        public string AverageDurationRating { get; set; }

    }
}
