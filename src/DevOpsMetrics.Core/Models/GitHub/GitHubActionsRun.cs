using System;

namespace DevOpsMetrics.Core.Models.GitHub
{
    public class GitHubActionsRun
    {
        public string status
        {
            get; set;
        }
        public string head_branch
        {
            get; set;
        }
        public string run_number
        {
            get; set;
        }
        public string html_url
        {
            get; set;
        }
        public DateTime created_at
        {
            get; set;
        }
        public DateTime updated_at
        {
            get; set;
        }

        ////Build duration in minutes
        //public float buildDuration
        //{
        //    get
        //    {
        //        float duration = 0f;
        //        if (updated_at != null && created_at != null && updated_at > DateTime.MinValue && created_at > DateTime.MinValue)
        //        {
        //            TimeSpan ts = updated_at - created_at;
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
        //        if (updated_at != null && created_at != null && updated_at > DateTime.MinValue && created_at > DateTime.MinValue)
        //        {
        //            TimeSpan timespan = updated_at - created_at;
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
        //        if (updated_at != null && updated_at > DateTime.MinValue)
        //        {
        //            TimeSpan timespan = DateTime.Now - updated_at;
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
        //                duration = updated_at.ToString("dd-MMM-yyyy");
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
