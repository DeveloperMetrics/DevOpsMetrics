using DevOpsMetrics.Service.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models.GitHub
{
    public class GitHubSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Owner { get; set; }
        public string Repo { get; set; }
        public string Branch { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowId { get; set; }

    }
}
