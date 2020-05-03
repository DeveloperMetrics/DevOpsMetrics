using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class GitHubActionsRun
    {
        public string status;
        public string head_branch;
        public DateTime created_at;
        public DateTime updated_at;
    }
}
