using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class GitHubPRCommit
    {
        public string sha { get; set; }
        public Committer committer { get; set; }
    }
}