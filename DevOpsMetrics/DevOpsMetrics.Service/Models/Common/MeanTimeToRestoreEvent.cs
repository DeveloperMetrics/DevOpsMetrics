using System;

namespace DevOpsMetrics.Service.Models.Common
{
    public class MeanTimeToRestoreEvent
    {
        public string ResourceGroup { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Url { get; set; }

        public float MTTRDurationInMinutes
        {
            get
            {
                float duration = 0f;
                if (EndTime != null && StartTime != null && EndTime > DateTime.MinValue && StartTime > DateTime.MinValue)
                {
                    TimeSpan ts = EndTime - StartTime;
                    duration = (float)ts.TotalMinutes;
                }
                return duration;
            }
        }

        //public string DeploymentName { get; set; }
        //public int NumberOfDays { get; set; }
        //public bool IsAzureDevOps { get; set; }
        //public List<Build> BuildList { get; set; }

        //private float _deploymentsPerDayMetric;
        //public float DeploymentsPerDayMetric
        //{
        //    get
        //    {
        //        return _deploymentsPerDayMetric;
        //    }
        //    set
        //    {
        //        _deploymentsPerDayMetric = value;
        //        float dailyDeployment = 1f;
        //        float weeklyDeployment = 1f / 7f;
        //        float monthlyDeployment = 1f / 30f;

        //        if (value > dailyDeployment) //NOTE: Does not capture on-demand deployments
        //        {
        //            DeploymentsToDisplayMetric = value;
        //            DeploymentsToDisplayUnit = "per day";
        //        }
        //        else if (value <= dailyDeployment && value >= weeklyDeployment)
        //        {
        //            DeploymentsToDisplayMetric = value * 7;
        //            DeploymentsToDisplayUnit = "times per week";
        //        }
        //        else if (value < weeklyDeployment && value >= monthlyDeployment)
        //        {
        //            DeploymentsToDisplayMetric = value * 30;
        //            DeploymentsToDisplayUnit = "times per month";
        //        }
        //        else if (value < monthlyDeployment)
        //        {
        //            DeploymentsToDisplayMetric = value * 30;
        //            DeploymentsToDisplayUnit = "times per month";
        //        }
        //    }
        //}
        //public float DeploymentsToDisplayMetric { get; set; }
        //public string DeploymentsToDisplayUnit { get; set; }
        //public string DeploymentsPerDayMetricDescription { get; set; }
        //public bool RateLimitHit { get; set; }
        public int ItemOrder { get; set; }
    }
}
