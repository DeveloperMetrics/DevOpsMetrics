using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class PullRequestModel
    {
        public string PullRequestId { get; set; }
        public string Branch { get; set; }
        public string Url { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration
        {
            get
            {
                return EndDateTime - StartDateTime;
            }
        }
        public int DurationPercent { get; set; } 
        public List<Commit> Commits { get; set; }
        public int BuildCount { get; set; } //TODO: Should this actually be the list of builds?

    }
}
