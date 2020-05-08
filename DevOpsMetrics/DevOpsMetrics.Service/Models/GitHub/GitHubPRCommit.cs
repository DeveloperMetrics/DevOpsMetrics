
namespace DevOpsMetrics.Service.Models.Common
{
    public class GitHubPRCommit
    {
        public string sha { get; set; }
        public Committer committer { get; set; }
    }
}