using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class LeadTimeForChangesModel
    {
        public string ProjectName { get; set; }
        public bool IsAzureDevOps { get; set; }
        public List<PullRequestModel> PullRequests { get; set; }

        public float AverageLeadTimeForChanges { get; set; }
        public string AverageLeadTimeForChangesRating { get; set; }

    }
}
