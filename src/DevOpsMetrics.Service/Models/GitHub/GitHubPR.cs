
namespace DevOpsMetrics.Service.Models.GitHub
{
    public class GitHubPR
    {
        public string number { get; set; }
        public GitHubHead head { get; set; }
        public string state { get; set; }
        public string merged_at{ get; set; }
    }
}