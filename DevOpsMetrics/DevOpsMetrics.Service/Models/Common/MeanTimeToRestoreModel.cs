using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class MeanTimeToRestoreModel
    {
        public DevOpsPlatform TargetDevOpsPlatform { get; set; }
        public bool IsProjectView { get; set; }
        public int NumberOfDays { get; set; }
        public int MaxNumberOfItems { get; set; }
        public int TotalItems { get; set; }
        public string ResourceGroup { get; set; }
        public List<MeanTimeToRestoreEvent> MeanTimeToRestoreEvents { get; set; }
        public float MTTRAverageDurationInHours { get; set; }
        public string MTTRAverageDurationDescription { get; set; }  
        public int ItemOrder { get; set; }

        public string BadgeURL { 
            get
            {
                //Example: https://img.shields.io/badge/Time%20to%20restore%20service-Elite-brightgreen
                return Badges.BadgeURL("Time%20to%20restore%20service", MTTRAverageDurationDescription);
            }
        }
    }
}
