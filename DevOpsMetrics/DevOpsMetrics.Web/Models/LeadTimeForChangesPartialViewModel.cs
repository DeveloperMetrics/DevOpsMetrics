using DevOpsMetrics.Service.Models;
using System.Collections.Generic;

namespace DevOpsMetrics.Web.Models
{
    public class LeadTimeForChangesPartialViewModel
    {
        public string ProjectName { get; set; }
        public List<LeadTimeForChangesModel> AzureDevOpsList { get; set; }
        public List<LeadTimeForChangesModel> GitHubList { get; set; }

        public float AverageDuration { get; set; }
        public float AverageDurationRating { get; set; }
    }
}
