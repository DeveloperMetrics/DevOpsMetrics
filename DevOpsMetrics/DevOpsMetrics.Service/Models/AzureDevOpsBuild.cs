using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class AzureDevOpsBuild
    {
        public string status { get; set; }
        public string sourceBranch { get; set; }
        public DateTime queueTime { get; set; }
        public DateTime finishTime { get; set; }

        //Build duration in minutes
        public float buildDuration
        {
            get
            {
                float duration = 0f;
                if (finishTime != null && queueTime != null && finishTime > DateTime.MinValue && queueTime > DateTime.MinValue)
                {
                    TimeSpan ts = finishTime - queueTime;
                    duration = (float)ts.TotalMinutes;
                }
                return duration;
            }
        }
    }
}
