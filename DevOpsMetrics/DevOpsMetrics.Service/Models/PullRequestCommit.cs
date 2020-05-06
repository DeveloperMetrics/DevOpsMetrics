using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class PullRequestCommit
    {
        public string commitId { get; set; }
        public Committer committer { get; set; }
    }

    public class Committer
    {
        public string name { get; set; }
        public DateTime date { get; set; }
    }
}