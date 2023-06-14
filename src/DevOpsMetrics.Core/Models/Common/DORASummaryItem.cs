using System;

namespace DevOpsMetrics.Core.Models.Common
{
    public class DORASummaryItem
    {
        public string Owner
        {
            get; set;
        }
        public string Project
        {
            get; set;
        }
        public string Repo
        {
            get; set;
        }
        public int NumberOfDays
        {
            get; set;
        }
        public float DeploymentFrequency
        {
            get; set;
        }
        public string DeploymentFrequencyBadgeURL
        {
            get; set;
        }
        public string DeploymentFrequencyBadgeWithMetricURL
        {
            get; set;
        }
        public float LeadTimeForChanges
        {
            get; set;
        }
        public string LeadTimeForChangesBadgeURL
        {
            get; set;
        }
        public string LeadTimeForChangesBadgeWithMetricURL
        {
            get; set;
        }
        public float MeanTimeToRestore
        {
            get; set;
        }
        public string MeanTimeToRestoreBadgeURL
        {
            get; set;
        }
        public string MeanTimeToRestoreBadgeWithMetricURL
        {
            get; set;
        }
        public float ChangeFailureRate
        {
            get; set;
        }
        public string ChangeFailureRateBadgeURL
        {
            get; set;
        }
        public string ChangeFailureRateBadgeWithMetricURL
        {
            get; set;
        }
        public string LastUpdatedMessage
        {
            get; set;
        }
        public DateTime LastUpdated
        {
            get; set;
        }
    }
}
