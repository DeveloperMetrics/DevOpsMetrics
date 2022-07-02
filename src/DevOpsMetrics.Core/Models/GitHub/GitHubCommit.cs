namespace DevOpsMetrics.Core.Models.GitHub
{
    public class GitHubCommit
    {
        public string sha
        {
            get; set;
        }
        public GitHubCommitter committer
        {
            get; set;
        }
    }
}
