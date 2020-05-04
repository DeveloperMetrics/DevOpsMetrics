using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class AzureDevOpsBuild
    {
        public string status { get; set; }
        public string sourceBranch { get; set; }
        public DateTime queueTime { get; set; }
        public DateTime finishTime { get; set; }
    }
}
