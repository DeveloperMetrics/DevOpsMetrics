namespace DevOpsMetrics.Service.Models.AzureDevOps
{
    public class AzureDevOpsPRCommit
    {
        public string commitId { get; set; }
        public Committer committer { get; set; }
    }


}