using System;

namespace DevOpsMetrics.Service.Models.Common
{
    public class ChangeFailureRateBuild : Build
    {
        public bool DeploymentWasSuccessful { get; set; }
    }
}
