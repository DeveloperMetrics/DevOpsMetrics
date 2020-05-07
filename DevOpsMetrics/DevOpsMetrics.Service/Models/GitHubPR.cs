
namespace DevOpsMetrics.Service.Models
{
    public class GitHubPR
    {
        public string number { get; set; }
        public GitHubHead head { get; set; }
    }
}