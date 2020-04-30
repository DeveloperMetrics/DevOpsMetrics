using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Mean time to restore (MTTR): How quickly we can restore production in an outage or degradation
    /// </summary>
    public class MeanTimeToRestore
    {
        public bool AddMeanTimeToRestore(string pipelineName, TimeSpan restoreDuration)
        {
            //SaveMeanTimeToRestore(pipelineName, restoreDuration);

            return true;
        }

        public float CalculateMeanTimeToRestore(string pipelineName, int numberOfDays)
        {
            List<TimeSpan> items = GetMeanTimeToRestore(pipelineName, numberOfDays);
            double totalMinutes = 0;
            foreach (TimeSpan item in items)
            {
                totalMinutes += item.TotalMinutes;
            }
            float meanTimeForChanges = (float)totalMinutes / (float)items.Count;

            return meanTimeForChanges;
        }

        private List<TimeSpan> GetMeanTimeToRestore(string pipelineName, int numberOfDays)
        {
            return new List<TimeSpan> { new TimeSpan(2, 0, 0), new TimeSpan(0, 45, 0), new TimeSpan(1, 0, 0), new TimeSpan(0, 40, 0), new TimeSpan(0, 50, 0) };
        }
    }
}
