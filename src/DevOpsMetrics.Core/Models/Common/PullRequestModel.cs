using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core.Models.Common
{
    public class PullRequestModel
    {
        public string PullRequestId
        {
            get; set;
        }
        public string Branch
        {
            get; set;
        }
        public string Url
        {
            get; set;
        }
        public DateTime StartDateTime
        {
            get; set;
        }
        public DateTime EndDateTime
        {
            get; set;
        }
        public string Status
        {
            get; set;
        }
        public TimeSpan Duration
        {
            get
            {
                TimeSpan duration = EndDateTime - StartDateTime;
                if (duration.TotalMinutes < 60)
                {
                    //If it's less than 60 minutes, round up to one hour, since we only measure this metric in hours.
                    duration = new TimeSpan(1, 0, 0);
                }
                return duration;
            }
        }
        public int DurationPercent
        {
            get; set;
        }
        public List<Commit> Commits
        {
            get; set;
        }
        public int BuildCount
        {
            get; set;
        } //Note: this could be a list of builds, but currently we have nothing to do with it, so a count will do for now

    }
}
