using System.Collections.Generic;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.GitHub;

namespace DevOpsMetrics.Web.Models
{
    public class IndexViewModel
    {
        public List<AzureDevOpsSettings> AzureDevOpsSettings
        {
            get; set;
        }
        public List<GitHubSettings> GitHubSettings
        {
            get; set;
        }
        public string ProjectId
        {
            get; set;
        }
        public string Log
        {
            get; set;
        }
    }
}
