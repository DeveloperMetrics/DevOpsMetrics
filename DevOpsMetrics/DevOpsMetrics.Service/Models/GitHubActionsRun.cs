using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class GitHubActionsRun
    {
        public string status { get; set; }
        public string head_branch { get; set; }
        public string run_number { get; set; }
        public string url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        //Build duration in minutes
        public float buildDuration
        {
            get
            {
                float duration = 0f;
                if (updated_at != null && created_at != null && updated_at > DateTime.MinValue && created_at > DateTime.MinValue)
                {
                    TimeSpan ts = updated_at - created_at;
                    duration = (float)ts.TotalMinutes;
                }
                return duration;
            }
        }
        public int buildDurationPercent { get; set; }
    }
}
