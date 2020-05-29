using DevOpsMetrics.Service.Models.Common;

namespace DevOpsMetrics.Web.Models
{
    public class ProjectViewModel
    {
        public string projectName { get; set; }
        public DeploymentFrequencyModel deploymentFrequencyModel { get; set; }
        public LeadTimeForChangesModel leadTimeForChangesModel { get; set; }
        public MeanTimeToRestoreModel meanTimeToRestoreModel { get; set; }
    }
}
