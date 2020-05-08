using DevOpsMetrics.Service.Models;
using System.Collections.Generic;

namespace DevOpsMetrics.Web.Models
{
    public class DeploymentPartialViewModel
    {
        public string DeploymentName { get; set; }
        public List<AzureDevOpsBuild> AzureDevOpsList { get; set; }
        public DeploymentFrequencyModel AzureDevOpsDeploymentFrequency { get; set; }
        public List<GitHubActionsRun> GitHubList { get; set; }
        public DeploymentFrequencyModel GitHubDeploymentFrequency { get; set; }

    }
}
