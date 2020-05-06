using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models
{
    public class DeploymentFrequencyModel
    {
        public float deploymentsPerDay { get; set; }
        public string deploymentsPerDayDescription { get; set; }
    }
}
