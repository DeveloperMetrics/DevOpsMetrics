using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOpsMetrics.Core
{
    /// <summary>
    /// Service Level Agreement (SLA): An agreement about the uptime for a service provided to a customer 
    /// </summary>
    public class SLA
    {
        private readonly List<KeyValuePair<DateTime, TimeSpan>> SLAList;

        public SLA()
        {
            SLAList = new List<KeyValuePair<DateTime, TimeSpan>>();
        }

        public float ProcessSLA(List<KeyValuePair<DateTime, TimeSpan>> SLAList, int numberOfDays)
        {
            if (SLAList != null)
            {
                foreach (KeyValuePair<DateTime, TimeSpan> item in SLAList)
                {
                    AddSLA(item.Key, item.Value);
                }
            }
            return CalculateSLA(numberOfDays);
        }

        private bool AddSLA(DateTime eventDateTime, TimeSpan restoreDuration)
        {
            SLAList.Add(new KeyValuePair<DateTime, TimeSpan>(eventDateTime, restoreDuration));
            return true;
        }

        private float CalculateSLA(int numberOfDays)
        {
            List<KeyValuePair<DateTime, TimeSpan>> items = GetSLA(numberOfDays);

            //Total number of hours, in the number of days
            int totalNumberOfHours = numberOfDays * 24;

            //Count up the total MTTR hours (this is our outage time)
            double totalHoursOutage = 0;
            foreach (KeyValuePair<DateTime, TimeSpan> item in items)
            {
                totalHoursOutage += item.Value.TotalHours;
            }

            //Calculate the SLA
            float sla = (float)(totalNumberOfHours - totalHoursOutage) / (float)totalNumberOfHours;

            return sla;
        }

        //Filter the list by date
        private List<KeyValuePair<DateTime, TimeSpan>> GetSLA(int numberOfDays)
        {
            return SLAList.Where(x => x.Key > DateTime.Now.AddDays(-numberOfDays)).ToList();
        }

        public string GetSLARating(float SLAPercent)
        {
            float oneNine = 0.9f; //90.0%
            float twoNines = 0.99f; //99.0%
            float threeNines = 0.999f; //99.9%
            float fourNines = 0.9999f; //99.99%
            float fiveNines = 0.99999f; //99.999%
            float sixNines = 0.999999f; //99.9999%

            string rating = "";
            if (SLAPercent < oneNine) //no rating
            {
                rating = "less than 90% SLA";
            }
            else if (SLAPercent >= sixNines)
            {
                rating = "99.9999% SLA";
            }
            else if (SLAPercent >= fiveNines)
            {
                rating = "99.999% SLA";
            }
            else if (SLAPercent >= fourNines)
            {
                rating = "99.99% SLA";
            }
            else if (SLAPercent >= threeNines)
            {
                rating = "99.9% SLA";
            }
            else if (SLAPercent >= twoNines)
            {
                rating = "99.0% SLA";
            }
            else if (SLAPercent >= oneNine) 
            {
                rating = "90.0% SLA";
            }

            return rating;
        }
    }
}
