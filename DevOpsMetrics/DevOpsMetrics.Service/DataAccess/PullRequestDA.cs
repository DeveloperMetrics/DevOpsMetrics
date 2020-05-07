using DevOpsMetrics.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class PullRequestDA
    {
        public static async Task<List<AzureDevOpsPRCommit>> GetAzureDevOpsPullRequestDetails(string patToken, string organization, string project, string pullRequestId)
        {
            string repositoryId = "SamLearnsAzure";
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/git/pull%20request%20commits/get%20pull%20request%20commits?view=azure-devops-rest-5.1
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version=5.1";
            string response = await MessageUtility.SendAzureDevOpsMessage(patToken, url);
            List<AzureDevOpsPRCommit> commits = new List<AzureDevOpsPRCommit>();
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = buildListObject.value;
                commits = JsonConvert.DeserializeObject<List<AzureDevOpsPRCommit>>(value.ToString());
            }
            return commits;
        }

        public static async Task<string> GetGitHubPullRequestByBranchName(string owner, string repo, string branch)
        {
            //https://developer.GitHub.com/v3/pulls/#list-pull-requests
            //GET /repos/:owner/:repo/pulls
            string url = $"/repos/{owner}/{repo}/pulls?state=all&head={branch}";
            string response = await MessageUtility.SendGitHubMessage(url, "https://api.GitHub.com/");

            List<GitHubPR> prs = new List<GitHubPR>();
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = buildListObject;
                prs = JsonConvert.DeserializeObject<List<GitHubPR>>(value.ToString());
            }
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

        public static async Task<List<GitHubPRCommit>> GetGitHubPullRequestDetails(string owner, string repo, string pull_number)
        {
            //https://developer.GitHub.com/v3/pulls/#list-commits-on-a-pull-request
            //GET /repos/:owner/:repo/pulls/:pull_number/commits
            string url = $"/repos/{owner}/{repo}/pulls/{pull_number}/commits";
            string response = await MessageUtility.SendGitHubMessage(url, "https://api.GitHub.com/");

            List<GitHubPRCommit> commits = new List<GitHubPRCommit>();
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = buildListObject;
                commits = JsonConvert.DeserializeObject<List<GitHubPRCommit>>(value.ToString());
            }
            return commits;
        }
    }
}
