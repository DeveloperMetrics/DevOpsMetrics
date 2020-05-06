using DevOpsMetrics.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Web.Models
{
    public class IndexDeploymentModel
    {
        public List<AzureDevOpsBuild> AZList { get; set; }
        public DeploymentFrequencyModel AZDeploymentFrequency { get; set; }
        public List<GitHubActionsRun> GHList { get; set; }
        public DeploymentFrequencyModel GHDeploymentFrequency { get; set; }

    }
}
