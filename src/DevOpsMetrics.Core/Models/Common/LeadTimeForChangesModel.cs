using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core.Models.Common
{
    public class LeadTimeForChangesModel
    {
        public LeadTimeForChangesModel()
        {
            PullRequests = new();
        }
        public string ProjectName
        {
            get; set;
        }
        public DevOpsPlatform TargetDevOpsPlatform
        {
            get; set;
        }
        public bool IsProjectView
        {
            get; set;
        }
        public int NumberOfDays
        {
            get; set;
        }
        public int MaxNumberOfItems
        {
            get; set;
        }
        public int TotalItems
        {
            get; set;
        }
        public List<PullRequestModel> PullRequests
        {
            get; set;
        }
        public float AverageBuildHours
        {
            get; set;
        }
        public float AveragePullRequestHours
        {
            get; set;
        }
        private float _leadTimeForChangesMetric;
        public float LeadTimeForChangesMetric
        {
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

        public float LeadTimeForChangesMetricDisplayMetric
        {
            get; set;
        }
        public string LeadTimeForChangesMetricDisplayUnit
        {
            get; set;
        }
        public string LeadTimeForChangesMetricDescription
        {
            get; set;
        }
        public bool RateLimitHit
        {
            get; set;
        }
        public int ItemOrder
        {
            get; set;
        }

        public string BadgeURL
        {
            get
            {
                //Example: https://img.shields.io/badge/Lead%20time%20for%20changes-High-green
                string title = Uri.EscapeDataString("Lead time for changes");
                return Badges.BadgeURL(title, LeadTimeForChangesMetricDescription);
            }
        }

        public string BadgeWithMetricURL
        {
            get
            {
                //Example: https://img.shields.io/badge/Change%20failure%20rate%20(83.33%25)-High-green
                string title = Uri.EscapeDataString("Lead time for changes (" + LeadTimeForChangesMetricDisplayMetric.ToString("0.0") + " " + LeadTimeForChangesMetricDisplayUnit + ")");
                return Badges.BadgeURL(title, LeadTimeForChangesMetricDescription);
            }
        }

        public Exception Exception
        {
            get; set;
        }
        public string ExceptionUrl
        {
            get; set;
        }
    }
}
