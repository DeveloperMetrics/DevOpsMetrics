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

        public float ProcessChangeFailureRate(List<KeyValuePair<DateTime, bool>> changeFailureRateList, string pipelineName, int numberOfDays)
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

            float changeFailureRate = 0;
            if (items == null || items.Count == 0)
            {
                 changeFailureRate = -1;
            }
            else
            {
                //Count up all successful changes
                int failureCount = 0;
                foreach (KeyValuePair<DateTime, bool> item in items)
                {
                    if (item.Value == false)
                    {
                        failureCount++;
                    }
                }

                //Calculate the change failure rate per day
                if (items.Count > 0)
                {
                    changeFailureRate = (float)failureCount / (float)items.Count;
                }
                changeFailureRate = (float)Math.Round((double)changeFailureRate, 4);
            }

            return changeFailureRate;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, bool>> GetChangeFailureRate(string pipelineName, int numberOfDays)
        {
            return ChangeFailureRateList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }

        public string GetChangeFailureRateRating(float changeFailureRate)
        {
            string rating = "";
            if (changeFailureRate < 0f)
            {
                rating = "None";
            }
            else if (changeFailureRate <= 0.15f) //0-15%
            {
                rating = "Elite";
            }
            else if (changeFailureRate <= 0.30f) //0-15% (not a typo, changing to <=30%)
            {
                rating = "High";
            }
            else if (changeFailureRate < 0.46f) //0-15% (not a typo, changing to <=46%)
            {
                rating = "Medium";
            }
            else if (changeFailureRate >= 0.46f)// 46-60% (but effectively 46+)
            {
                rating = "Low";
            }
            return rating;
        }
    }
}
