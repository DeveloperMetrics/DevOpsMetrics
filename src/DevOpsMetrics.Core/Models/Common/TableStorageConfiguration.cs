using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Core.Models.Common
{
    public class TableStorageConfiguration
    {
        //public string StorageAccountName { get; set; }
        //public string StorageAccountAccessKey { get; set; }
        public string StorageAccountConnectionString{ get; set; }
        public string TableAzureDevOpsSettings { get; set; }
        public string TableAzureDevOpsBuilds { get; set; }
        public string TableAzureDevOpsPRs { get; set; }
        public string TableAzureDevOpsPRCommits { get; set; }
        public string TableGitHubSettings { get; set; }
        public string TableGitHubRuns { get; set; }
        public string TableGitHubPRs { get; set; }
        public string TableGitHubPRCommits { get; set; }
        public string TableMTTR { get; set; }
        public string TableChangeFailureRate { get; set; }
        public string TableLog { get; set; }

    }
}