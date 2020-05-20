using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models.Common
{
    public class TableStorageAuth
    {
        //public TableStorageAuth(string accountName, string accountAccessKey, string tableName)
        //{
        //    AccountName = accountName;
        //    AccountAccessKey = accountAccessKey;
        //    TableAzureDevOpsBuilds = tableName;
        //    TableAzureDevOpsPRs = tableName;
        //    TableAzureDevOpsPRCommits = tableName;
        //    TableGitHubRuns = tableName;
        //    TableGitHubPRs = tableName;
        //    TableGitHubPRCommits = tableName;
        //}

        public string AccountName { get; set; }
        public string AccountAccessKey { get; set; }
        public string TableAzureDevOpsBuilds { get; set; }
        public string TableAzureDevOpsPRs { get; set; }
        public string TableAzureDevOpsPRCommits { get; set; }
        public string TableGitHubRuns { get; set; }
        public string TableGitHubPRs { get; set; }
        public string TableGitHubPRCommits { get; set; }
    }
}
