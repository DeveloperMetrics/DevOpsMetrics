using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core.Models.Common
{
    public class DeploymentFrequencyModel
    {
        public DeploymentFrequencyModel()
        {
            BuildList = new();
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
        public List<Build> BuildList
        {
            get; set;
        }
        private float _deploymentsPerDayMetric;
        public float DeploymentsPerDayMetric
        {
            get
            {
                return _deploymentsPerDayMetric;
            }
            set
            {
                _deploymentsPerDayMetric = value;
                float dailyDeployment = 1f;
                float weeklyDeployment = 1f / 7f;
                float monthlyDeployment = 1f / 30f;
                float yearlyDeployment = 1f / 365f;

                if (value > dailyDeployment) //NOTE: Does not capture on-demand deployments
                {
                    DeploymentsToDisplayMetric = value;
                    DeploymentsToDisplayUnit = "per day";
                }
                else if (value <= dailyDeployment && value >= weeklyDeployment)
                {
                    DeploymentsToDisplayMetric = value * 7;
                    DeploymentsToDisplayUnit = "times per week";
                }
                else if (value < weeklyDeployment && value >= monthlyDeployment)
                {
                    DeploymentsToDisplayMetric = value * 30;
                    DeploymentsToDisplayUnit = "times per month";
                }
                else if (value < monthlyDeployment && value > yearlyDeployment)
                {
                    DeploymentsToDisplayMetric = value * 30;
                    DeploymentsToDisplayUnit = "times per month";
                }
                else if (value <= yearlyDeployment)
                {
                    DeploymentsToDisplayMetric = value * 365;
                    DeploymentsToDisplayUnit = "times per year";
                }
            }
        }
        public float DeploymentsToDisplayMetric
        {
            get; set;
        }
        public string DeploymentsToDisplayUnit
        {
            get; set;
        }
        public string DeploymentsPerDayMetricDescription
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
                //Example: https://img.shields.io/badge/Deployment%20frequency-High-green
                string title = Uri.EscapeDataString("Deployment frequency");
                return Badges.BadgeURL(title, DeploymentsPerDayMetricDescription);
            }
        }

        public string BadgeWithMetricURL
        {
            get
            {
                //Example: https://img.shields.io/badge/Change%20failure%20rate%20(83.33%25)-High-green
                string title = Uri.EscapeDataString("Deployment frequency (" + DeploymentsToDisplayMetric.ToString("0.00") + " " + DeploymentsToDisplayUnit + ")");
                return Badges.BadgeURL(title, DeploymentsPerDayMetricDescription);
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
