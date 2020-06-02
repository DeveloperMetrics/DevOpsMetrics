using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Deployment frequency: How often we deploy to production
    /// </summary>
    public class DeploymentFrequency
    {
        private List<KeyValuePair<DateTime, DateTime>> DeploymentFrequencyList;

        public DeploymentFrequency()
        {
            DeploymentFrequencyList = new List<KeyValuePair<DateTime, DateTime>>();
        }

        /// <summary>
        /// Add and calculate a deployment frequency list to return the frequency of deployments
        /// </summary>
        /// <param name="deploymentFrequencyList"></param>
        /// <param name="pipelineName"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        public float ProcessDeploymentFrequency(List<KeyValuePair<DateTime, DateTime>> deploymentFrequencyList, string pipelineName, int numberOfDays)
        {
            if (deploymentFrequencyList != null)
            {
                DeploymentFrequencyList = new List<KeyValuePair<DateTime, DateTime>>();
                foreach (KeyValuePair<DateTime, DateTime> item in deploymentFrequencyList)
                {
                    AddDeploymentFrequency(pipelineName, item.Key, item.Value);
                }
            }
            return CalculateDeploymentFrequency(pipelineName, numberOfDays);
        }

        private bool AddDeploymentFrequency(string pipelineName, DateTime eventDateTime, DateTime deploymentDateTime)
        {
            DeploymentFrequencyList.Add(new KeyValuePair<DateTime, DateTime>(eventDateTime, deploymentDateTime));
            return true;
        }

        private float CalculateDeploymentFrequency(string pipelineName, int numberOfDays)
        {
            List<KeyValuePair<DateTime, DateTime>> items = GetDeploymentFrequency(pipelineName, numberOfDays);

            //Calculate the deployments per day
            float deploymentsPerDay = 0;

            if (items.Count > 0 && numberOfDays > 0)
            {
                deploymentsPerDay = (float)items.Count / (float)numberOfDays;
            }

            deploymentsPerDay = (float)Math.Round((double)deploymentsPerDay, 4);

            return deploymentsPerDay;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, DateTime>> GetDeploymentFrequency(string pipelineName, int numberOfDays)
        {
            return DeploymentFrequencyList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }

        public string GetDeploymentFrequencyRating(float deploymentsPerDay)
        {
            float dailyDeployment = 1f;
            float weeklyDeployment = 1f / 7f;
            float monthlyDeployment = 1f / 30f;

            string rating = "";
            if (deploymentsPerDay <= 0f) 
            {
                rating = "None";
            }
            else if (deploymentsPerDay > dailyDeployment) //NOTE: Does not capture on-demand deployments
            {
                rating = "Elite";
            }
            else if (deploymentsPerDay <= dailyDeployment && deploymentsPerDay >= weeklyDeployment)
            {
                rating = "High";
            }
            else if (deploymentsPerDay < weeklyDeployment && deploymentsPerDay >= monthlyDeployment)
            {
                rating = "Medium";
            }
            else if (deploymentsPerDay < monthlyDeployment)
            {
                rating = "Low";
            }
            return rating;
        }
    }
}
