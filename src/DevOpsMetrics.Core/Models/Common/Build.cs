using System;

namespace DevOpsMetrics.Core.Models.Common
{
    public class Build
    {
        public string Id
        {
            get; set;
        }
        public string Status
        {
            get; set;
        }
        public string Branch
        {
            get; set;
        }
        public string BuildNumber
        {
            get; set;
        }
        public string Url
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

        //Build duration in minutes
        public float BuildDuration
        {
            get
            {
                float duration = 0f;
                if (EndTime > DateTime.MinValue && StartTime > DateTime.MinValue)
                {
                    TimeSpan ts = EndTime - StartTime;
                    duration = (float)ts.TotalMinutes;
                }
                return duration;
            }
        }

        public string BuildDurationInMinutesAndSeconds
        {
            get
            {
                string duration = "0:00";
                if (EndTime > DateTime.MinValue && StartTime > DateTime.MinValue)
                {
                    TimeSpan timespan = EndTime - StartTime;
                    duration = $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
                }
                return duration;
            }
        }

        public string TimeSinceBuildCompleted
        {
            get
            {
                string duration = "0:00";
                if (EndTime > DateTime.MinValue)
                {
                    TimeSpan timespan = DateTime.UtcNow - EndTime;
                    if (timespan.TotalMinutes < 60)
                    {
                        duration = ((int)timespan.TotalMinutes).ToString() + " mins ago";
                    }
                    else if (timespan.TotalHours < 24)
                    {
                        duration = ((int)timespan.TotalHours).ToString() + " hours ago";
                    }
                    else if (timespan.TotalDays < 7)
                    {
                        duration = ((int)timespan.TotalDays).ToString() + " days ago";
                    }
                    else
                    {
                        duration = EndTime.ToString("dd-MMM-yyyy");
                    }
                }
                return duration;
            }
        }

        public int BuildDurationPercent
        {
            get; set;
        }
    }
}
