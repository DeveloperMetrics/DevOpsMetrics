using System.Collections.Generic;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectLogViewModel
    {
        public string ProjectId { get; set; }
        public List<ProjectLog> Logs { get; set; }
        //public List<AzureDevOpsSettings> AzureDevOpsSettings { get; set; }
        //public List<GitHubSettings> GitHubSettings { get; set; }
        public SelectList Projects { get; set; }

    }
}
