using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectUpdateViewModel
    {
        public SelectList ProjectList { get; set; }

        public string ProjectIdSelected { get; set; }

        public SelectList CompletionPercentList { get; set; }

        public int CompletionPercentSelected { get; set; }
    }
}
