using System;

namespace DevOpsMetrics.Core.Models.Common
{
    public class Commit
    {
        public string commitId
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public DateTime date
        {
            get; set;
        }
    }
}