using System;

namespace DevOpsMetrics.Service.Models
{
    public class Commit
    {
        public string commitId { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
    }
}