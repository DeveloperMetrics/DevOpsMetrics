using System.Collections.Generic;

namespace DevOpsMetrics.Core.Models.Common
{
    public class SLAModel
    {
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
        public string ResourceGroup
        {
            get; set;
        }
        public List<MeanTimeToRestoreEvent> MeanTimeToRestoreEvents
        {
            get; set;
        }
        public float SLADurationInHours
        {
            get; set;
        }
        public string SLADurationDescription
        {
            get; set;
        }
        public int ItemOrder
        {
            get; set;
        }

        //public string BadgeURL { 
        //    get
        //    {
        //        //Example: https://img.shields.io/badge/Time%20to%20restore%20service-High-green
        //        string title = Uri.EscapeDataString("SLA");
        //        return Badges.BadgeURL(title, MTTRAverageDurationDescription);
        //    }
        //}

        //public string BadgeWithMetricURL
        //{
        //    get
        //    {
        //        //Example: https://img.shields.io/badge/Change%20failure%20rate%20(83.33%25)-High-green
        //        string title = Uri.EscapeDataString("Time to restore service (" + MTTRAverageDurationInHours.ToString("0.00") + " hours)");
        //        return Badges.BadgeURL(title, MTTRAverageDurationDescription);
        //    }
        //}
    }
}
