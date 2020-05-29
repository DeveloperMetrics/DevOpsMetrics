using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models.GitHub
{
    public class GitHubCommit
    {
        public string sha { get; set; }
        public GitHubCommitter committer { get; set; }
    }
}
