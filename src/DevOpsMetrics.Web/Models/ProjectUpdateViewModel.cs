using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectUpdateViewModel
    {
        public string Organization_owner
        {
            get; set;
        }
        public string Project_repo
        {
            get; set;
        }
        public string BuildName_workflowName
        {
            get; set;
        }
        public SelectList ProjectList
        {
            get; set;
        }
        public string ProjectIdSelected
        {
            get; set;
        }
        public SelectList CompletionPercentList
        {
            get; set;
        }
        public int CompletionPercentSelected
        {
            get; set;
        }
        public SelectList NumberOfDaysList
        {
            get; set;
        }
        public int NumberOfDaysSelected
        {
            get; set;
        }
    }
}
