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
        public float AZDeploymentFrequency { get; set; }
        public string AZDeploymentFrequencyText
        {
            get
            {
                return "Elite";
            }
        }
        public List<GitHubActionsRun> GHList { get; set; }
        public float GHDeploymentFrequency { get; set; }
        public string GHDeploymentFrequencyText
        {
            get
            {
                return "Poor";
            }
        }
    }
}
