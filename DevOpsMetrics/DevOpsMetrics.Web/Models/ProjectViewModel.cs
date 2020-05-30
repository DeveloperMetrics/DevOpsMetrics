using DevOpsMetrics.Service.Models.Common;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectViewModel
    {
        public string ProjectName { get; set; }
        public DeploymentFrequencyModel DeploymentFrequency { get; set; }
        public LeadTimeForChangesModel LeadTimeForChanges { get; set; }
        public MeanTimeToRestoreModel MeanTimeToRestore { get; set; }
        public ChangeFailureRateModel ChangeFailureRate { get; set; }
    }
}
