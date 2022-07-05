namespace DevOpsMetrics.Core.Models.Common
{
    public class TableStorageConfiguration
    {
        public string StorageAccountConnectionString
        {
            get; set;
        }
        public string TableAzureDevOpsSettings { get; set; } = "DevOpsAzureDevOpsSettings";
        public string TableAzureDevOpsBuilds { get; set; } = "DevOpsAzureDevOpsBuilds";
        public string TableAzureDevOpsPRs { get; set; } = "DevOpsAzureDevOpsPRs";
        public string TableAzureDevOpsPRCommits { get; set; } = "DevOpsAzureDevOpsPRCommits";
        public string TableGitHubSettings { get; set; } = "DevOpsGitHubSettings";
        public string TableGitHubRuns { get; set; } = "DevOpsGitHubRuns";
        public string TableGitHubPRs { get; set; } = "DevOpsGitHubPRs";
        public string TableGitHubPRCommits { get; set; } = "DevOpsGitHubPRCommits";
        public string TableMTTR { get; set; } = "DevOpsMTTR";
        public string TableChangeFailureRate { get; set; } = "DevOpsChangeFailureRate";
        public string TableDORASummaryItem { get; set; } = "DevOpsDORASummaryItem";
        public string TableLog { get; set; } = "DevOpsLog";
    }
}