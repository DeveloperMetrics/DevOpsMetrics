﻿using DevOpsMetrics.Service.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models.AzureDevOps
{
    public class AzureDevOpsSettings
    {
        public string RowKey { get; set; }
        public string PatToken { get; set; }
        public string Organization { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string Branch { get; set; }
        public string BuildName { get; set; }
        public string BuildId { get; set; }
        public string ProductionResourceGroup { get; set; }
        public int ItemOrder { get; set; }
    }
}
