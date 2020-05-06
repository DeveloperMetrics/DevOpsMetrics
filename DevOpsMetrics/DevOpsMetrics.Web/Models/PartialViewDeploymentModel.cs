using DevOpsMetrics.Service.Models;
using System.Collections.Generic;

namespace DevOpsMetrics.Web.Models
{
    public class PartialViewDeploymentModel
    {
        public string DeploymentName { get; set; }
        public List<AzureDevOpsBuild> AZList { get; set; }
        public DeploymentFrequencyModel AZDeploymentFrequency { get; set; }
        public List<GitHubActionsRun> GHList { get; set; }
        public DeploymentFrequencyModel GHDeploymentFrequency { get; set; }

    }
}
