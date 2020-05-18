using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.GitHub;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class PullRequestDA
    {
        public async Task<Newtonsoft.Json.Linq.JArray> GetAzureDevOpsPullRequestCommitsJArray(string patToken, string organization, string project, string repositoryId, string pullRequestId)
        {
            Newtonsoft.Json.Linq.JArray list = null;
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/git/pull%20request%20commits/get%20pull%20request%20commits?view=azure-devops-rest-5.1
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version=5.1";
            string response = await MessageUtility.SendAzureDevOpsMessage(url, patToken);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                list = buildListObject.value;
            }
            return list;
        }

        public async Task<List<AzureDevOpsPRCommit>> GetAzureDevOpsPullRequestCommits(string patToken, string organization, string project, string repositoryId, string pullRequestId)
        {
            Newtonsoft.Json.Linq.JArray list = await GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repositoryId, pullRequestId);
            List<AzureDevOpsPRCommit> commits = JsonConvert.DeserializeObject<List<AzureDevOpsPRCommit>>(list.ToString());

            return commits;
        }

        public async Task<string> GetGitHubPullRequestIdByBranchName(string clientId, string clientSecret, string owner, string repo, string branch)
        {
            //https://developer.GitHub.com/v3/pulls/#list-pull-requests
            //GET /repos/:owner/:repo/pulls
            string url = $"https://api.github.com/repos/{owner}/{repo}/pulls?state=all&head={branch}&per_page=100";
            string response = await MessageUtility.SendGitHubMessage(url, clientId, clientSecret);

            List<GitHubPR> prs = new List<GitHubPR>();
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = buildListObject;
                prs = JsonConvert.DeserializeObject<List<GitHubPR>>(value.ToString());
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

        public async Task<Newtonsoft.Json.Linq.JArray> GetGitHubPullRequestCommitsJArray(string clientId, string clientSecret, string owner, string repo, string pull_number)
        {
            Newtonsoft.Json.Linq.JArray list = null;
            //https://developer.GitHub.com/v3/pulls/#list-commits-on-a-pull-request
            //GET /repos/:owner/:repo/pulls/:pull_number/commits
            string url = $"https://api.github.com/repos/{owner}/{repo}/pulls/{pull_number}/commits?per_page=100";
            string response = await MessageUtility.SendGitHubMessage(url, clientId, clientSecret);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                list = buildListObject;
            }
            return list;
        }

        public async Task<List<GitHubPRCommit>> GetGitHubPullRequestCommits(string clientId, string clientSecret, string owner, string repo, string pull_number)
        {
            Newtonsoft.Json.Linq.JArray list = await GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);
            List<GitHubPRCommit> commits = JsonConvert.DeserializeObject<List<GitHubPRCommit>>(list.ToString());
            return commits;
        }
    }
}
