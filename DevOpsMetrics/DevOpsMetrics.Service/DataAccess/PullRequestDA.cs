using DevOpsMetrics.Service.DataAccess.APIAccess;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.GitHub;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class PullRequestDA
    {

        public async Task<List<AzureDevOpsPRCommit>> GetAzureDevOpsPullRequestCommits(string patToken, string organization, string project, string repositoryId, string pullRequestId)
        {
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            Newtonsoft.Json.Linq.JArray list = await api.GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repositoryId, pullRequestId);
            List<AzureDevOpsPRCommit> commits = JsonConvert.DeserializeObject<List<AzureDevOpsPRCommit>>(list.ToString());
            return commits;
        }

        public async Task<string> GetGitHubPullRequestIdByBranchName(string clientId, string clientSecret, string owner, string repo, string branch)
        {
            List<GitHubPR> prs = new List<GitHubPR>();
            GitHubAPIAccess api = new GitHubAPIAccess();
            Newtonsoft.Json.Linq.JArray list = await api.GetGitHubPullRequestsJArray(clientId, clientSecret, owner, repo, branch);
            if (list != null)
            {
                prs = JsonConvert.DeserializeObject<List<GitHubPR>>(list.ToString());
            }

            //Find the PR id
            string prId = "";
            foreach (GitHubPR item in prs)
            {
                if (item.head.@ref == branch)
                {
                    prId = item.number;
                }
            }
            return prId;
        }

        public async Task<List<GitHubPRCommit>> GetGitHubPullRequestCommits(string clientId, string clientSecret, string owner, string repo, string pull_number)
        {
            GitHubAPIAccess api = new GitHubAPIAccess();
            Newtonsoft.Json.Linq.JArray list = await api.GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);
            List<GitHubPRCommit> commits = JsonConvert.DeserializeObject<List<GitHubPRCommit>>(list.ToString());
            return commits;
        }
    }
}
