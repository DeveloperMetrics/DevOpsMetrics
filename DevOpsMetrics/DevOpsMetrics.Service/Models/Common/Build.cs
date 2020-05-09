using System;

namespace DevOpsMetrics.Service.Models.Common
{
    public class Build
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Branch { get; set; }
        public string BuildNumber { get; set; }
        public string Url { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
