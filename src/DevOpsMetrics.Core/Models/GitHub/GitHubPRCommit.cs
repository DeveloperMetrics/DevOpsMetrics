namespace DevOpsMetrics.Core.Models.GitHub
{
    public class GitHubPRCommit
    {
        public string sha
        {
            get; set;
        }
        public GitHubCommit commit
        {
            get; set;
        }
    }
}