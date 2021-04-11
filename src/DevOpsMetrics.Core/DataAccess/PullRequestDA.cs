using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.APIAccess;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Newtonsoft.Json;

namespace DevOpsMetrics.Core.DataAccess
{
    public class PullRequestDA
    {
        public async Task<AzureDevOpsPR> GetAzureDevOpsPullRequest(string patToken, TableStorageConfiguration tableStorageConfig, string organization, string project, string repositoryId, string branch, bool useCache)
        {
            List<AzureDevOpsPR> prs = new List<AzureDevOpsPR>();
            Newtonsoft.Json.Linq.JArray list;
            if (useCache == true)
            {
                //Get the pull requests from Azure storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                list = daTableStorage.GetTableStorageItems(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRs, daTableStorage.CreateGitHubPRPartitionKey(organization, project));
            }
            else
            {
                //Get the pull requests from the Azure DevOps API
                AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
                list = await api.GetAzureDevOpsPullRequestsJArray(patToken, organization, project, repositoryId);
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

        public async Task<List<AzureDevOpsPRCommit>> GetAzureDevOpsPullRequestCommits(string patToken, TableStorageConfiguration tableStorageConfig, string organization, string project, string repositoryId, string pullRequestId, bool useCache)
        {
            Newtonsoft.Json.Linq.JArray list;
            if (useCache == true)
            {
                //Get the commits from Azure storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                list = daTableStorage.GetTableStorageItems(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRCommits, daTableStorage.CreateAzureDevOpsPRCommitPartitionKey(organization, project, pullRequestId));
            }
            else
            {
                //Get the commits from the Azure DevOps API
                AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
                list = await api.GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repositoryId, pullRequestId);
            }

            List<AzureDevOpsPRCommit> commits = JsonConvert.DeserializeObject<List<AzureDevOpsPRCommit>>(list.ToString());
            return commits;
        }


        public async Task<GitHubPR> GetGitHubPullRequest(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig, string owner, string repo, string branch, bool useCache)
        {
            List<GitHubPR> prs = new List<GitHubPR>();
            Newtonsoft.Json.Linq.JArray list;
            if (useCache == true)
            {
                //Get the pull requests from Azure storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                list = daTableStorage.GetTableStorageItems(tableStorageConfig, tableStorageConfig.TableGitHubPRs, daTableStorage.CreateGitHubPRPartitionKey(owner, repo));
            }
            else
            {
                //Get the pull requests from the GitHub API
                GitHubAPIAccess api = new GitHubAPIAccess();
                list = await api.GetGitHubPullRequestsJArray(clientId, clientSecret, owner, repo, branch);
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

        public async Task<List<GitHubPRCommit>> GetGitHubPullRequestCommits(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig, string owner, string repo, string pull_number, bool useCache)
        {
            Newtonsoft.Json.Linq.JArray list;
            if (useCache == true)
            {
                //Get the commits from Azure storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                list = daTableStorage.GetTableStorageItems(tableStorageConfig, tableStorageConfig.TableGitHubPRCommits, daTableStorage.CreateGitHubPRCommitPartitionKey(owner, repo, pull_number));
            }
            else
            {
                //Get the commits from the GitHub API
                GitHubAPIAccess api = new GitHubAPIAccess();
                list = await api.GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);
            }
            List<GitHubPRCommit> commits = JsonConvert.DeserializeObject<List<GitHubPRCommit>>(list.ToString());
            return commits;
        }
    }
}
