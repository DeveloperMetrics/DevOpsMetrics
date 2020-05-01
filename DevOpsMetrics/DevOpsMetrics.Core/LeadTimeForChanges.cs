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
            double totalMinutes = 0;
            foreach (KeyValuePair<DateTime, TimeSpan> item in items)
            {
                totalMinutes += item.Value.TotalMinutes;
            }
            //Calculate the lead time for changes per day
            float leadTimeForChanges = 0;
            if (items.Count > 0)
            {
                leadTimeForChanges = (float)totalMinutes / (float)items.Count;
            }

            return leadTimeForChanges;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, TimeSpan>> GetLeadTimeForChanges(string pipelineName, int numberOfDays)
        {
            return LeadTimeForChangesList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }
    }
}
