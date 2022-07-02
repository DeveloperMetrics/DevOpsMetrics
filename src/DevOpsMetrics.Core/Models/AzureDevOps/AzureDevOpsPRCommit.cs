using DevOpsMetrics.Core.Models.Common;

namespace DevOpsMetrics.Core.Models.AzureDevOps
{
    public class AzureDevOpsPRCommit
    {
        public string commitId
        {
            get; set;
        }
        public Committer committer
        {
            get; set;
        }
    }

}