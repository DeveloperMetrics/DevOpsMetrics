using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectViewModel
    {
        public string RowKey
        {
            get; set;
        }
        public string ProjectName
        {
            get; set;
        }
        public SelectList NumberOfDays
        {
            get; set;
        }
        public int NumberOfDaysSelected
        {
            get; set;
        }
        public DevOpsPlatform TargetDevOpsPlatform
        {
            get; set;
        }
        public DeploymentFrequencyModel DeploymentFrequency
        {
            get; set;
        }
        public LeadTimeForChangesModel LeadTimeForChanges
        {
            get; set;
        }
        public MeanTimeToRestoreModel MeanTimeToRestore
        {
            get; set;
        }
        public ChangeFailureRateModel ChangeFailureRate
        {
            get; set;
        }
    }
}
