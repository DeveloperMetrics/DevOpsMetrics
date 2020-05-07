using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class GitHubPR
    {
        public string number { get; set; }
        public GitHubHead head { get; set; }
    }
}