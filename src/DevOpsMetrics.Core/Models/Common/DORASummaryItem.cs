using System;

namespace DevOpsMetrics.Core.Models.Common
{
    public class DORASummaryItem
    {
        public DORASummaryItem()
        {
        }

        public DORASummaryItem(string owner, string project, string repo)
        {
            Owner = owner;
            Project = project;
            Repo = repo;
            //Set the properties to defaults (none)
            DeploymentFrequency = 0;
            DeploymentFrequencyBadgeURL = "https://img.shields.io/badge/Deployment%20frequency-None-lightgrey";
            DeploymentFrequencyBadgeWithMetricURL = "https://img.shields.io/badge/Deployment%20frequency%20-None-lightgrey";
            LeadTimeForChanges = 0;
            LeadTimeForChangesBadgeURL = "https://img.shields.io/badge/Lead%20time%20for%20changes-None-lightgrey";
            LeadTimeForChangesBadgeWithMetricURL = "https://img.shields.io/badge/Lead%20time%20for%20changes%20-None-lightgrey";
            MeanTimeToRestore = 0;
            MeanTimeToRestoreBadgeURL = "https://img.shields.io/badge/Time%20to%20restore%20service-None-lightgrey";
            MeanTimeToRestoreBadgeWithMetricURL = "https://img.shields.io/badge/Time%20to%20restore%20service%20-None-lightgrey";
            ChangeFailureRate = 0;
            ChangeFailureRateBadgeURL = "https://img.shields.io/badge/Change%20failure%20rate-None-lightgrey";
            ChangeFailureRateBadgeWithMetricURL = "https://img.shields.io/badge/Change%20failure%20rate%20-None-lightgrey";
            LastUpdatedMessage = "No data available";
        }

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
