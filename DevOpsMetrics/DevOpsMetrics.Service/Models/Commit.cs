using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class Commit
    {
        public string commitId { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
    }
}