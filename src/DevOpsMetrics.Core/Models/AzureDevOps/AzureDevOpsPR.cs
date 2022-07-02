
namespace DevOpsMetrics.Core.Models.AzureDevOps
{
    public class AzureDevOpsPR
    {
        public string PullRequestId
        {
            get; set;
        }
        public string targetRefName
        {
            get; set;
        }
        public string sourceRefName
        {
            get; set;
        }
        public string status
        {
            get; set;
        }
    }
}