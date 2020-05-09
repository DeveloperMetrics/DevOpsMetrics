
using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class DeploymentFrequencyModel
    {
        public string DeploymentName { get; set; }
        public bool IsAzureDevOps { get; set; }
        public List<Build> BuildList { get; set; }
        public float DeploymentsPerDayMetric { get; set; }
        public string DeploymentsPerDayMetricDescription { get; set; }
    }
}
