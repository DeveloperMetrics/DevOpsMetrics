using System;

namespace DevOpsMetrics.Service.Models.AzureDevOps
{
    public class AzureDevOpsBuild
    {
        public string id { get; set; }
        public string status { get; set; }
        public string sourceBranch { get; set; }
        public string buildNumber { get; set; }
        public string url { get; set; }
        public DateTime queueTime { get; set; }
        public DateTime finishTime { get; set; }

        
    }
}
