
namespace DevOpsMetrics.Service.Models
{
    public class GitHubPRCommit
    {
        public string sha { get; set; }
        public Committer committer { get; set; }
    }
}