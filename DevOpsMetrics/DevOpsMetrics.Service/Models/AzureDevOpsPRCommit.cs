namespace DevOpsMetrics.Service.Models
{
    public class AzureDevOpsPRCommit
    {
        public string commitId { get; set; }
        public Committer committer { get; set; }
    }


}