using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Mean time to restore (MTTR): How quickly we can restore production in an outage or degradation
    /// </summary>
    public class MeanTimeToRestore
    {
        private readonly List<KeyValuePair<DateTime, TimeSpan>> MeanTimeToRestoreList;

        public MeanTimeToRestore()
        {
            MeanTimeToRestoreList = new();
        }

        public float ProcessMeanTimeToRestore(List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList, int numberOfDays)
        {
            if (meanTimeToRestoreList != null)
            {
                foreach (KeyValuePair<DateTime, TimeSpan> item in meanTimeToRestoreList)
                {
                    AddMeanTimeToRestore(item.Key, item.Value);
                }
            }
            return CalculateMeanTimeToRestore(numberOfDays);
        }

        private bool AddMeanTimeToRestore(DateTime eventDateTime, TimeSpan restoreDuration)
        {
            MeanTimeToRestoreList.Add(new KeyValuePair<DateTime, TimeSpan>(eventDateTime, restoreDuration));
            return true;
        }

        private float CalculateMeanTimeToRestore(int numberOfDays)
        {
            List<KeyValuePair<DateTime, TimeSpan>> items = GetMeanTimeToRestore(numberOfDays);

            //Count up the total MTTR hours
            double totalHours = 0;
            foreach (KeyValuePair<DateTime, TimeSpan> item in items)
            {
                totalHours += item.Value.TotalHours;
            }

            //Calculate mean time for changes per day
            float meanTimeForChanges = 0;
            if (items.Count > 0)
            {
                meanTimeForChanges = (float)totalHours / (float)items.Count;
            }

            meanTimeForChanges = (float)Math.Round((double)meanTimeForChanges, 2);

            return meanTimeForChanges;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, TimeSpan>> GetMeanTimeToRestore(int numberOfDays)
        {
            return MeanTimeToRestoreList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }

        public static string GetMeanTimeToRestoreRating(float meanTimeToRestoreInHours)
        {
            //float hourlyRestoration = 1f;
            float dailyRestoration = 24f;
            float weeklyRestoration = 24f * 7f;

            string rating = "";
            if (meanTimeToRestoreInHours <= 0)
            {
                rating = "None";
            }
            else if (meanTimeToRestoreInHours < dailyRestoration) //less than one day
            {
                rating = "High";
            }
            else if (meanTimeToRestoreInHours < weeklyRestoration) //less than one day and one week
            {
                rating = "Medium";
            }
            else if (meanTimeToRestoreInHours > weeklyRestoration) //more than one week (originally 1 week - 1 month)
            {
                rating = "Low";
            }
            //no rating else statement not required here, as all scenarios are covered above with < and >

            return rating;
        }
    }
}
