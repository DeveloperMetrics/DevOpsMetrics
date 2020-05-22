
namespace DevOpsMetrics.Service.Models.AzureDevOps
{
    public class AzureDevOpsPR
    {
        public string PullRequestId { get; set; }
        public string targetRefName { get; set; }
        public string status { get; set; }
    }
}