using System;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.Models.AzureDevOps
{
    public class AzureDevOpsBuild
    {
        public string id
        {
            get; set;
        }
        public string status
        {
            get; set;
        }
        public string sourceBranch
        {
            get; set;
        }
        public string parameters
        {
            get; set;
        }
        public string branch
        {
            get
            {
                string prBranch;
                if (parameters != null)
                {
                    JToken item = JToken.Parse(parameters);
                    if (item["system.pullRequest.sourceBranch"] != null)
                    {
                        prBranch = item["system.pullRequest.sourceBranch"].ToString();
                    }
                    else
                    {
                        prBranch = "";
                    }
                }
                else
                {
                    prBranch = "";
                }
                return prBranch;
            }
        }
        public string buildNumber
        {
            get; set;
        }
        public string url
        {
            get; set;
        }
        public DateTime queueTime
        {
            get; set;
        }
        public DateTime finishTime
        {
            get; set;
        }

        ////Build duration in minutes
        //public float buildDuration
        //{
        //    get
        //    {
        //        float duration = 0f;
        //        if (finishTime != null && queueTime != null && finishTime > DateTime.MinValue && queueTime > DateTime.MinValue)
        //        {
        //            TimeSpan ts = finishTime - queueTime;
        //            duration = (float)ts.TotalMinutes;
        //        }
        //        return duration;
        //    }
        //}

        //public string buildDurationInMinutesAndSeconds
        //{
        //    get
        //    {
        //        string duration = "0:00";
        //        if (finishTime != null && queueTime != null && finishTime > DateTime.MinValue && queueTime > DateTime.MinValue)
        //        {
        //            TimeSpan timespan = finishTime - queueTime;
        //            duration = $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
        //        }
        //        return duration;
        //    }
        //}
        //public string timeSinceBuildCompleted
        //{
        //    get
        //    {
        //        string duration = "0:00";
        //        if (finishTime != null && finishTime > DateTime.MinValue)
        //        {
        //            TimeSpan timespan = DateTime.Now - finishTime;
        //            if (timespan.TotalMinutes < 60)
        //            {
        //                duration = ((int)timespan.TotalMinutes).ToString() + " mins ago";
        //            }
        //            else if (timespan.TotalHours < 24)
        //            {
        //                duration = ((int)timespan.TotalHours).ToString() + " hours ago";
        //            }
        //            else if (timespan.TotalDays < 7)
        //            {
        //                duration = ((int)timespan.TotalDays).ToString() + " days ago";
        //            }
        //            else
        //            {
        //                duration = finishTime.ToString("dd-MMM-yyyy");
        //            }
        //        }
        //        return duration;
        //    }
        //}

        public int buildDurationPercent
        {
            get; set;
        }
    }
}
