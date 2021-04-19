using DevOpsMetrics.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Core.Models.GitHub
{
    public class GitHubSettings
    {
        public string RowKey { get; set; }
        public string Owner { get; set; }
        public string Repo { get; set; }
        public string Branch { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowId { get; set; }
        public string ProductionResourceGroup { get; set; }
        public int ItemOrder { get; set; }

    }
}
