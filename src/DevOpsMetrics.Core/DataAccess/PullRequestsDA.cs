using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.APIAccess;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.DataAccess
{
    public class PullRequestsDA
    {
        public static async Task<AzureDevOpsPR> GetAzureDevOpsPullRequest(string patToken, TableStorageConfiguration tableStorageConfig,
            string organization, string project, string repository, string branch, bool useCache)
        {
            List<AzureDevOpsPR> prs = new();
            JArray list;
            if (useCache)
            {
                //Get the pull requests from Azure storage
                AzureTableStorageDA daTableStorage = new();
                list = await daTableStorage.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRs, PartitionKeys.CreateGitHubPRPartitionKey(organization, project));
            }
            else
            {
                //Get the pull requests from the Azure DevOps API
                list = await AzureDevOpsAPIAccess.GetAzureDevOpsPullRequestsJArray(patToken, organization, project, repository);
            }
            if (list != null)
            {
                prs = JsonConvert.DeserializeObject<List<AzureDevOpsPR>>(list.ToString());
            }

            //Find the PR id
            AzureDevOpsPR pr = null;
            foreach (AzureDevOpsPR item in prs)
            {
                if (item.sourceRefName == branch)
                {
                    pr = item;
                    break;
                }
            }
            return pr;
        }

        public static async Task<List<AzureDevOpsPRCommit>> GetAzureDevOpsPullRequestCommits(string patToken, TableStorageConfiguration tableStorageConfig, string organization, string project, string repository, string pullRequestId, bool useCache)
        {
            JArray list;
            if (useCache)
            {
                //Get the commits from Azure storage
                AzureTableStorageDA daTableStorage = new();
                list = await daTableStorage.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRCommits, PartitionKeys.CreateAzureDevOpsPRCommitPartitionKey(organization, project, pullRequestId));
            }
            else
            {
                //Get the commits from the Azure DevOps API
                list = await AzureDevOpsAPIAccess.GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repository, pullRequestId);
            }

            List<AzureDevOpsPRCommit> commits = JsonConvert.DeserializeObject<List<AzureDevOpsPRCommit>>(list.ToString());
            return commits;
        }

        public static async Task<GitHubPR> GetGitHubPullRequest(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig, string owner, string repo, string branch, bool useCache)
        {
            List<GitHubPR> prs = new();
            JArray list;
            if (useCache)
            {
                //Get the pull requests from Azure storage
                AzureTableStorageDA daTableStorage = new();
                list = await daTableStorage.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubPRs, PartitionKeys.CreateGitHubPRPartitionKey(owner, repo));
            }
            else
            {
                //Get the pull requests from the GitHub API
                list = await GitHubAPIAccess.GetGitHubPullRequestsJArray(clientId, clientSecret, owner, repo, branch);
            }
            if (list != null)
            {
                prs = JsonConvert.DeserializeObject<List<GitHubPR>>(list.ToString());
            }

            //Find the PR id
            GitHubPR pr = null;
            foreach (GitHubPR item in prs)
            {
                if (item.head.@ref == branch)
                {
                    pr = item;
                    break;
                }
            }
            return pr;
        }

        public static async Task<List<GitHubPRCommit>> GetGitHubPullRequestCommits(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig, string owner, string repo, string pull_number, bool useCache)
        {
            JArray list;
            if (useCache)
            {
                //Get the commits from Azure storage
                AzureTableStorageDA daTableStorage = new();
                list = await daTableStorage.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubPRCommits, PartitionKeys.CreateGitHubPRCommitPartitionKey(owner, repo, pull_number));
            }
            else
            {
                //Get the commits from the GitHub API
                list = await GitHubAPIAccess.GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);
            }
            List<GitHubPRCommit> commits = JsonConvert.DeserializeObject<List<GitHubPRCommit>>(list.ToString());
            return commits;
        }
    }
}
