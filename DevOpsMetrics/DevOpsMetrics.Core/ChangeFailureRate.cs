using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Change failure rate: after a production deployment, was it successful? Or did we need to deploy a fix/rollback?
    /// </summary>
    public class ChangeFailureRate
    {
        public bool AddChangeFailureRate(string pipelineName, bool deploymentIsSuccessful)
        {
            //SaveChangeFailureRate(pipelineName, deploymentIsSuccessful);

            return true;
        }

        public float CalculateChangeFailureRate(string pipelineName, int numberOfDays)
        {
            //Get the number of days
            List<bool> items = GetChangeFailureRate(pipelineName, numberOfDays);
            int successfulCount = 0;
            foreach (bool item in items)
            {
                if (item)
                {
                    successfulCount++;
                }
            }

            return (float)successfulCount / (float)items.Count;
        }

        private List<bool> GetChangeFailureRate(string pipelineName, int numberOfDays)
        {
            return new List<bool> {true, false, true, false, true};
        }
    }
}
