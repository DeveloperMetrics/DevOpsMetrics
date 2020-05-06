using DevOpsMetrics.Service.Models;
using System.Collections.Generic;

namespace DevOpsMetrics.Web.Models
{
    public class IndexDeploymentModel
    {
        public List<PartialViewDeploymentModel> Items { get; set; }
    }
}
