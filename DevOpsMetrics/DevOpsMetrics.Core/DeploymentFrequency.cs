using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Deployment frequency: How often we deploy to production
    /// </summary>
    public class DeploymentFrequency
    {
        public bool AddDeploymentFrequency(string pipelineName, DateTime deploymentTime)
        {
            //SaveDeploymentFrequency(pipelineName, deploymentTime);

            return true;
        }

        public float CalculateDeploymentFrequency(string pipelineName, int numberOfDays)
        {
            List<DateTime> items = GetDeploymentFrequency(pipelineName, numberOfDays);

            float deploymentsPerDay = (float)numberOfDays / (float)items.Count;

            return deploymentsPerDay;
        }

        private List<DateTime> GetDeploymentFrequency(string pipelineName, int numberOfDays)
        {
            return new List<DateTime> { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now };
        }
    }
}
