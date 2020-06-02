using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectUpdateItem
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
