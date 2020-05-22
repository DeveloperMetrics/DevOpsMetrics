using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class LeadTimeForChangesModel
    {
        public string ProjectName { get; set; }
        public int NumberOfDays { get; set; }
        public bool IsAzureDevOps { get; set; }
        public List<PullRequestModel> PullRequests { get; set; }
        public float AverageBuildHours { get; set; }
        public float AveragePullRequestHours { get; set; }
        private float _leadTimeForChangesMetric;
        public float LeadTimeForChangesMetric {
            get
            {
                return _leadTimeForChangesMetric;
            }
            set
            {
                _leadTimeForChangesMetric = value;
                LeadTimeForChangesMetricDisplayMetric = value;
                LeadTimeForChangesMetricDisplayUnit = "hours";
            }
        }

        public float LeadTimeForChangesMetricDisplayMetric { get; set; }
        public string LeadTimeForChangesMetricDisplayUnit { get; set; }
        public string LeadTimeForChangesMetricDescription { get; set; }
        public bool RateLimitHit { get; set; }
        public int ItemOrder { get; set; }
    }
}
