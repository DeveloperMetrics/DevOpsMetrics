using System;

namespace DevOpsMetrics.Core.Models.Common
{
    public class MeanTimeToRestoreEvent
    {
        public string Name
        {
            get; set;
        }
        public string ResourceGroup
        {
            get; set;
        }
        public string Resource
        {
            get; set;
        }
        public DateTime StartTime
        {
            get; set;
        }
        public DateTime EndTime
        {
            get; set;
        }
        public string Status
        {
            get; set;
        }
        public string Url
        {
            get; set;
        }

        public float MTTRDurationInHours
        {
            get
            {
                float duration = 0f;
                if (EndTime > DateTime.MinValue && StartTime > DateTime.MinValue)
                {
                    TimeSpan ts = EndTime - StartTime;
                    duration = (float)ts.TotalHours;
                }
                return duration;
            }
        }
        public int MTTRDurationPercent
        {
            get; set;
        }

        public int ItemOrder
        {
            get; set;
        }
    }
}
