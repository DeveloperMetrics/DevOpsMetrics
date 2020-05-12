using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Lead time for changes: Time from committing a change to deployment to production
    /// </summary>
    public class LeadTimeForChanges
    {
        private List<KeyValuePair<DateTime, TimeSpan>> LeadTimeForChangesList;

        public LeadTimeForChanges()
        {
            LeadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>();
        }

        public float ProcessLeadTimeForChanges(List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList, string pipelineName, int numberOfDays)
        {
            if (leadTimeForChangesList != null)
            {
                foreach (KeyValuePair<DateTime, TimeSpan> item in leadTimeForChangesList)
                {
                    AddLeadTimeForChanges(pipelineName, item.Key, item.Value);
                }
            }
            return CalculateLeadTimeForChanges(pipelineName, numberOfDays);
        }

        private bool AddLeadTimeForChanges(string pipelineName, DateTime eventDateTime, TimeSpan leadTimeDuration)
        {
            LeadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(eventDateTime, leadTimeDuration));
            return true;
        }

        private float CalculateLeadTimeForChanges(string pipelineName, int numberOfDays)
        {
            List<KeyValuePair<DateTime, TimeSpan>> items = GetLeadTimeForChanges(pipelineName, numberOfDays);

            //Add up the total minutes
            double totalHours = 0;
            foreach (KeyValuePair<DateTime, TimeSpan> item in items)
            {
                totalHours += item.Value.TotalHours;
            }
            //Calculate the lead time for changes per day
            float leadTimeForChanges = 0;
            if (items.Count > 0)
            {
                leadTimeForChanges = (float)items.Count / (float)totalHours;
            }

            leadTimeForChanges = (float)Math.Round((double)leadTimeForChanges, 4);

            return leadTimeForChanges;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, TimeSpan>> GetLeadTimeForChanges(string pipelineName, int numberOfDays)
        {
            return LeadTimeForChangesList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }

        public string GetLeadTimeForChangesRating(float leadTimeForChanges)
        {
            float dailyDeployment = 1f;
            float weeklyDeployment = 1f / 7f;
            float monthlyDeployment = 1f / 30f;

            string result = "";
            if (leadTimeForChanges > dailyDeployment) //less than one day
            {
                result = "Elite";
            }
            else if (leadTimeForChanges <= dailyDeployment && leadTimeForChanges >= weeklyDeployment) //between one day and one week
            {
                result = "High";
            }
            else if (leadTimeForChanges < weeklyDeployment && leadTimeForChanges >= monthlyDeployment) //between one week and one month
            {
                result = "Medium";
            }
            else if (leadTimeForChanges < monthlyDeployment) //more than one month
            {
                result = "Low";
            }
            return result;
        }
    }
}
