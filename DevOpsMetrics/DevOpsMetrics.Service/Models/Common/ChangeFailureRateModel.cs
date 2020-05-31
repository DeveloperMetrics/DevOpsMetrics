using System.Collections.Generic;

namespace DevOpsMetrics.Service.Models.Common
{
    public class ChangeFailureRateModel
    {
        public string DeploymentName { get; set; }
        public DevOpsPlatform TargetDevOpsPlatform { get; set; }
        public List<ChangeFailureRateBuild> ChangeFailureRateBuildList { get; set; }
        public float ChangeFailureRateMetric { get; set; }
        public string ChangeFailureRateMetricDescription { get; set; }
    }
}