using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Change failure rate: after a production deployment, was it successful? Or did we need to deploy a fix/rollback?
    /// </summary>
    public class ChangeFailureRate
    {
        private List<KeyValuePair<DateTime, bool>> ChangeFailureRateList;

        public ChangeFailureRate()
        {
            ChangeFailureRateList = new List<KeyValuePair<DateTime, bool>>();
        }

        public float ProcessLeadTimeForChanges(List<KeyValuePair<DateTime, bool>> changeFailureRateList, string pipelineName, int numberOfDays)
        {
            if (changeFailureRateList != null)
            {
                foreach (KeyValuePair<DateTime, bool> item in changeFailureRateList)
                {
                    AddChangeFailureRate(pipelineName, item.Key, item.Value);
                }
            }
            return CalculateChangeFailureRate(pipelineName, numberOfDays);
        }

        private bool AddChangeFailureRate(string pipelineName, DateTime eventDateTime, bool deploymentIsSuccessful)
        {
            ChangeFailureRateList.Add(new KeyValuePair<DateTime, bool>(eventDateTime, deploymentIsSuccessful));
            return true;
        }

        private float CalculateChangeFailureRate(string pipelineName, int numberOfDays)
        {
            List<KeyValuePair<DateTime, bool>> items = GetChangeFailureRate(pipelineName, numberOfDays);

            //Count up all successful changes
            int successfulCount = 0;
            foreach (KeyValuePair<DateTime, bool> item in items)
            {
                if (item.Value == true)
                {
                    successfulCount++;
                }
            }

            //Calculate the change failure rate per day
            float changeFailureRate = 0;
            if (items.Count > 0)
            {
                changeFailureRate = (float)successfulCount / (float)items.Count;
            }

            changeFailureRate = (float)Math.Round((double)changeFailureRate, 4);

            return changeFailureRate;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, bool>> GetChangeFailureRate(string pipelineName, int numberOfDays)
        {
            return ChangeFailureRateList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }
    }
}
