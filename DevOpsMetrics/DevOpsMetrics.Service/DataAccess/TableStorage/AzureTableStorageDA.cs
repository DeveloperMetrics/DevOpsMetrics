using DevOpsMetrics.Service.DataAccess.APIAccess;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess.TableStorage
{
    public class AzureTableStorageDA
    {
        public async Task<int> UpdateAzureDevOpsBuilds(string patToken, string accountName, string accessKey, string tableName, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
        {
            BuildsDA da = new BuildsDA();
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            Newtonsoft.Json.Linq.JArray items = await api.GetAzureDevOpsBuildsJArray(patToken, organization, project, branch, buildId);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsBuild build = JsonConvert.DeserializeObject<AzureDevOpsBuild>(item.ToString());

                if (build.status == "completed")
                {
                    string partitionKey = organization + "_" + project + "_" + buildName;
                    string rowKey = build.buildNumber;
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateAzureDevOpsPullRequests(string patToken, string accountName, string accessKey, string tableName, string organization, string project, string repositoryId, int numberOfDays, int maxNumberOfItems)
        {
            PullRequestDA da = new PullRequestDA();
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            Newtonsoft.Json.Linq.JArray items = await api.GetAzureDevOpsPullRequestsJArray(patToken, organization, project, repositoryId);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());

                string partitionKey = organization + "_" + project;
                string rowKey = pullRequest.PullRequestId;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateAzureDevOpsPullRequestCommits(string patToken, string accountName, string accessKey, string tableName, string organization, string project, string repositoryId, string pullRequestId, int numberOfDays, int maxNumberOfItems)
        {
            PullRequestDA da = new PullRequestDA();
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            Newtonsoft.Json.Linq.JArray items = await api.GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repositoryId, pullRequestId);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPRCommit pullRequestCommit = JsonConvert.DeserializeObject<AzureDevOpsPRCommit>(item.ToString());

                string partitionKey = organization + "_" + project;
                string rowKey = pullRequestCommit.commitId;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionRuns(string clientId, string clientSecret, string accountName, string accessKey, string tableName, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            BuildsDA da = new BuildsDA();
            GitHubAPIAccess api = new GitHubAPIAccess();
            Newtonsoft.Json.Linq.JArray items = await api.GetGitHubActionRunsJArray(clientId, clientSecret, owner, repo, branch, workflowId);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubActionsRun build = JsonConvert.DeserializeObject<GitHubActionsRun>(item.ToString());

                if (build.status == "completed")
                {
                    string partitionKey = owner + "_" + repo + "_" + workflowName;
                    string rowKey = build.run_number;
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionPullRequests(string clientId, string clientSecret, string accountName, string accessKey, string tableName, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            PullRequestDA da = new PullRequestDA();
            GitHubAPIAccess api = new GitHubAPIAccess();
            Newtonsoft.Json.Linq.JArray items = await api.GetGitHubPullRequestsJArray(clientId, clientSecret, owner, repo, branch);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubPR pr = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());

                if (pr.state == "closed")
                {
                    string partitionKey = owner + "_" + repo;
                    string rowKey = pr.number;
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionPullRequestCommits(string clientId, string clientSecret, string accountName, string accessKey, string tableName, string owner, string repo, string branch, string workflowName, string workflowId, string pull_number, int numberOfDays, int maxNumberOfItems)
        {
            PullRequestDA da = new PullRequestDA();
            GitHubAPIAccess api = new GitHubAPIAccess();
            Newtonsoft.Json.Linq.JArray items = await api.GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubActionsRun build = JsonConvert.DeserializeObject<GitHubActionsRun>(item.ToString());

                if (build.status == "completed")
                {
                    string partitionKey = owner + "_" + repo;
                    string rowKey = pull_number;
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }
                }
            }

            return itemsAdded;
        }

    }
}
