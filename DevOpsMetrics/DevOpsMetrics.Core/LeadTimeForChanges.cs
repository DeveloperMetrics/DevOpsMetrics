using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Lead time for changes: Time from committing a change to deployment to production
    /// </summary>
    public class LeadTimeForChanges
    {
        public bool AddLeadTimeForChanges(string pipelineName, TimeSpan leadTimeDuration)
        {
            //SaveLeadTimeForChanges(pipelineName, leadTimeDuration);

            return true;
        }

        public float CalculateLeadTimeForChanges(string pipelineName, int numberOfDays)
        {
            List<TimeSpan> items = GetLeadTimeForChanges(pipelineName, numberOfDays);

            double totalMinutes = 0;
            foreach (TimeSpan item in items)
            {
                totalMinutes += item.TotalMinutes;
            }
            float leadTimeForChanges = (float)totalMinutes / (float)items.Count;

            return leadTimeForChanges;
        }

        private List<TimeSpan> GetLeadTimeForChanges(string pipelineName, int numberOfDays)
        {
            return new List<TimeSpan> { new TimeSpan(2, 0, 0), new TimeSpan(0, 45, 0), new TimeSpan(1, 0, 0), new TimeSpan(0, 40, 0), new TimeSpan(0, 50, 0) };
        }
    }
}
