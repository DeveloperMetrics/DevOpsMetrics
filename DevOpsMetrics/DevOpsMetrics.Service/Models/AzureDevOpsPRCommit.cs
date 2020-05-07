using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class AzureDevOpsPRCommit
    {
        public string commitId { get; set; }
        public Committer committer { get; set; }
    }


}