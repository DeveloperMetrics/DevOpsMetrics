using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core.Models.Common
{
    public class ChangeFailureRateModel
    {
        public ChangeFailureRateModel()
        {
            ChangeFailureRateBuildList = new();
        }

        public string DeploymentName
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
        public int MaxNumberOfItems
        {
            get; set;
        }
        public int NumberOfDays
        {
            get; set;
        }
        public int TotalItems
        {
            get; set;
        }
        public List<ChangeFailureRateBuild> ChangeFailureRateBuildList
        {
            get; set;
        }
        public float ChangeFailureRateMetric
        {
            get; set;
        }
        public string ChangeFailureRateMetricDescription
        {
            get; set;
        }

        public string BadgeURL
        {
            get
            {
                //Example: https://img.shields.io/badge/Change%20failure%20rate-High-green
                string title = Uri.EscapeDataString("Change failure rate");
                return Badges.BadgeURL(title, ChangeFailureRateMetricDescription);
            }
        }

        public string BadgeWithMetricURL
        {
            get
            {
                string changeFailureRate = ChangeFailureRateMetric.ToString("0.00%").Replace("-", "");
                //Example: https://img.shields.io/badge/Change%20failure%20rate%20(83.33%25)-High-green
                string title = Uri.EscapeDataString("Change failure rate (" + changeFailureRate + ")");
                return Badges.BadgeURL(title, ChangeFailureRateMetricDescription);
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