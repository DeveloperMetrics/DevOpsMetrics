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
        private readonly List<KeyValuePair<DateTime, TimeSpan>> LeadTimeForChangesList;

        public LeadTimeForChanges()
        {
            LeadTimeForChangesList = new();
        }

        public float ProcessLeadTimeForChanges(List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList, int numberOfDays)
        {
            if (leadTimeForChangesList != null)
            {
                foreach (KeyValuePair<DateTime, TimeSpan> item in leadTimeForChangesList)
                {
                    AddLeadTimeForChanges(item.Key, item.Value);
                }
            }
            return CalculateLeadTimeForChanges(numberOfDays);
        }

        private bool AddLeadTimeForChanges(DateTime eventDateTime, TimeSpan leadTimeDuration)
        {
            LeadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(eventDateTime, leadTimeDuration));
            return true;
        }

        private float CalculateLeadTimeForChanges(int numberOfDays)
        {
            List<KeyValuePair<DateTime, TimeSpan>> items = GetLeadTimeForChanges(numberOfDays);

            //Add up the total hours
            double totalHours = 0;
            foreach (KeyValuePair<DateTime, TimeSpan> item in items)
            {
                totalHours += item.Value.TotalHours;
            }
            //Calculate the lead time for changes per day
            float leadTimeForChanges = 0;
            if (items.Count > 0)
            {
                leadTimeForChanges = (float)totalHours / (float)items.Count;
            }

            leadTimeForChanges = (float)Math.Round((double)leadTimeForChanges, 4);

            return leadTimeForChanges;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, TimeSpan>> GetLeadTimeForChanges(int numberOfDays)
        {
            return LeadTimeForChangesList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }

        public static string GetLeadTimeForChangesRating(float leadTimeForChangesInHours)
        {
            //float dailyDeployment = 24f;
            float weeklyDeployment = 24f * 7f;
            float monthlyDeployment = 24f * 30f;
            //float sixMonthDeployment = 24f * 30f * 6f;

            string rating = "";
            if (leadTimeForChangesInHours <= 0f) //no rating
            {
                rating = "None";
            }
            else if (leadTimeForChangesInHours <= weeklyDeployment) //between one day and one week/ or once a week or faster
            {
                rating = "High";
            }
            else if (leadTimeForChangesInHours > weeklyDeployment && leadTimeForChangesInHours <= monthlyDeployment) //between one week and one month
            {
                rating = "Medium";
            }
            else if (leadTimeForChangesInHours > monthlyDeployment) //more than once every month
            {
                rating = "Low";
            }
            //no rating else statement not required here, as all scenarios are covered above with < and >

            return rating;
        }
    }
}
