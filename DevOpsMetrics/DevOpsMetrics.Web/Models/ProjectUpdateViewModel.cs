using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectUpdateViewModel
    {
        public SelectList ProjectList { get; set; }

        public string ProjectId { get; set; }
    }
}
