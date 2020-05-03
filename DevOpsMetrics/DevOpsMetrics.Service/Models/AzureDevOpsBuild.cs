using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class AzureDevOpsBuild
    {
        public string status;
        public string sourceBranch;
        public DateTime queueTime;
        public DateTime finishTime;
    }
}
