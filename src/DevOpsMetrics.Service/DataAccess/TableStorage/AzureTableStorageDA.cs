using DevOpsMetrics.Service.DataAccess.APIAccess;
using DevOpsMetrics.Service.Models.Azure;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess.TableStorage
{
    public class AzureTableStorageDA : IAzureTableStorageDA
    {
        public string CreateAzureDevOpsSettingsPartitionKey(string organization, string project, string repository, string buildName)
        {
            return organization + "_" + project + "_" + repository;
        }

        public string CreateBuildWorkflowPartitionKey(string organization_owner, string project_repo, string buildName_workflowName)
        {
            return organization_owner + "_" + project_repo + "_" + buildName_workflowName;
        }

        public string CreateAzureDevOpsPRPartitionKey(string organization, string project)
        {
            return organization + "_" + project;
        }

        public string CreateAzureDevOpsPRCommitPartitionKey(string organization, string project, string pullRequestId)
        {
            return organization + "_" + project + "_" + pullRequestId;
        }

        public string CreateGitHubSettingsPartitionKey(string owner, string repo, string workflowName)
        {
            return owner + "_" + repo;
        }

        public string CreateGitHubPRPartitionKey(string owner, string repo)
        {
            return owner + "_" + repo;
        }

        public string CreateGitHubPRCommitPartitionKey(string owner, string repo, string pullRequestId)
        {
            return owner + "_" + repo + "_" + pullRequestId;
        }

        //Note that this can't be async due to performance issues with Azure Storage when you retrieve items
        public JArray GetTableStorageItems(TableStorageAuth tableStorageAuth, string tableName, string partitionKey)
        {
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, tableName);
            List<AzureStorageTableModel> items = tableDA.GetItems(partitionKey);
            JArray list = new JArray();
            foreach (AzureStorageTableModel item in items)
            {
                list.Add(JToken.Parse(item.Data));
            }
            return list;
        }

        public async Task<int> UpdateAzureDevOpsBuilds(string patToken, TableStorageAuth tableStorageAuth,
                string organization, string project, string branch, string buildName, string buildId,
                int numberOfDays, int maxNumberOfItems)
        {
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            JArray items = await api.GetAzureDevOpsBuildsJArray(patToken, organization, project);

            int itemsAdded = 0;
            TableStorageCommonDA tableBuildsDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableAzureDevOpsBuilds);
            TableStorageCommonDA tableChangeFailureRateDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableChangeFailureRate);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsBuild build = JsonConvert.DeserializeObject<AzureDevOpsBuild>(item.ToString());

                //Save the build information for builds
                if (build.status == "completed")
                {
                    string partitionKey = CreateBuildWorkflowPartitionKey(organization, project, buildName);
                    string rowKey = build.buildNumber;
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableBuildsDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }

                    //Save the build information for change failure rate
                    ChangeFailureRateBuild newBuild = new ChangeFailureRateBuild
                    {
                        Id = build.id,
                        Branch = build.sourceBranch,
                        BuildNumber = build.buildNumber,
                        StartTime = build.queueTime,
                        EndTime = build.finishTime,
                        BuildDurationPercent = build.buildDurationPercent,
                        Status = build.status,
                        Url = build.url
                    };
                    itemsAdded += await UpdateChangeFailureRate(tableChangeFailureRateDA, newBuild, CreateBuildWorkflowPartitionKey(organization, project, buildName));

                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateAzureDevOpsPullRequests(string patToken, TableStorageAuth tableStorageAuth,
                string organization, string project, string repositoryId,
                int numberOfDays, int maxNumberOfItems)
        {
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            JArray items = await api.GetAzureDevOpsPullRequestsJArray(patToken, organization, project, repositoryId);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableAzureDevOpsPRs);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());

                string partitionKey = CreateAzureDevOpsPRPartitionKey(organization, project);
                string rowKey = pullRequest.PullRequestId;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }

                itemsAdded += await UpdateAzureDevOpsPullRequestCommits(patToken, tableStorageAuth,
                    organization, project, repositoryId, pullRequest.PullRequestId,
                    numberOfDays, maxNumberOfItems);
            }

            return itemsAdded;
        }

        public async Task<int> UpdateAzureDevOpsPullRequestCommits(string patToken, TableStorageAuth tableStorageAuth,
                string organization, string project, string repositoryId, string pullRequestId,
                int numberOfDays, int maxNumberOfItems)
        {
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            JArray items = await api.GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repositoryId, pullRequestId);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableAzureDevOpsPRCommits);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPRCommit pullRequestCommit = JsonConvert.DeserializeObject<AzureDevOpsPRCommit>(item.ToString());

                string partitionKey = CreateAzureDevOpsPRCommitPartitionKey(organization, project, pullRequestId);
                string rowKey = pullRequestCommit.commitId;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionRuns(string clientId, string clientSecret, TableStorageAuth tableStorageAuth,
                string owner, string repo, string branch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems)
        {
            GitHubAPIAccess api = new GitHubAPIAccess();
            JArray items = await api.GetGitHubActionRunsJArray(clientId, clientSecret, owner, repo, workflowId);

            int itemsAdded = 0;
            TableStorageCommonDA tableBuildDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableGitHubRuns);
            TableStorageCommonDA tableChangeFailureRateDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableChangeFailureRate);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubActionsRun build = JsonConvert.DeserializeObject<GitHubActionsRun>(item.ToString());

                //Save the build information for builds
                if (build.status == "completed")
                {
                    string partitionKey = CreateBuildWorkflowPartitionKey(owner, repo, workflowName);
                    string rowKey = build.run_number;
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableBuildDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }

                    //Save the build information for change failure rate
                    ChangeFailureRateBuild newBuild = new ChangeFailureRateBuild
                    {
                        Id = build.run_number,
                        Branch = build.head_branch,
                        BuildNumber = build.run_number,
                        StartTime = build.created_at,
                        EndTime = build.updated_at,
                        BuildDurationPercent = build.buildDurationPercent,
                        Status = build.status,
                        Url = build.html_url
                    };
                    itemsAdded += await UpdateChangeFailureRate(tableChangeFailureRateDA, newBuild, CreateBuildWorkflowPartitionKey(owner, repo, workflowName));
                }

            }
            return itemsAdded;
        }

        public async Task<int> UpdateChangeFailureRate(TableStorageCommonDA tableChangeFailureRateDA, ChangeFailureRateBuild newBuild, string partitionKey, bool forceUpdate = false)
        {
            int itemsAdded = 0;
            string rowKey = newBuild.Id;
            string json = JsonConvert.SerializeObject(newBuild);
            AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, json);
            if (await tableChangeFailureRateDA.AddItem(newItem, forceUpdate) == true)
            {
                itemsAdded++;
            }
            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionPullRequests(string clientId, string clientSecret, TableStorageAuth tableStorageAuth,
                string owner, string repo, string branch,
                int numberOfDays, int maxNumberOfItems)
        {
            GitHubAPIAccess api = new GitHubAPIAccess();
            JArray items = await api.GetGitHubPullRequestsJArray(clientId, clientSecret, owner, repo, branch);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableGitHubPRs);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubPR pr = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());

                if (pr.state == "closed" & pr.merged_at != null)
                {
                    string partitionKey = CreateGitHubPRPartitionKey(owner, repo);
                    string rowKey = pr.number;
                    Debug.WriteLine($"PartitionKey: {partitionKey}, RowKey: {rowKey}");
                    //if (rowKey == "242" || rowKey == "239")
                    //{
                    //    Debug.WriteLine("here");
                    //}
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }

                    itemsAdded += await UpdateGitHubActionPullRequestCommits(clientId, clientSecret, tableStorageAuth,
                        owner, repo, pr.number);
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionPullRequestCommits(string clientId, string clientSecret, TableStorageAuth tableStorageAuth,
                string owner, string repo, string pull_number)
        {
            GitHubAPIAccess api = new GitHubAPIAccess();
            JArray items = await api.GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableGitHubPRCommits);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubCommit commit = JsonConvert.DeserializeObject<GitHubCommit>(item.ToString());

                string partitionKey = CreateGitHubPRCommitPartitionKey(owner, repo, pull_number);
                string rowKey = commit.sha;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }
            }

            return itemsAdded;
        }

        public List<AzureDevOpsSettings> GetAzureDevOpsSettings(TableStorageAuth tableStorageAuth, string settingsTable)
        {
            List<AzureDevOpsSettings> settings = null;
            string partitionKey = "AzureDevOpsSettings";
            JArray list = GetTableStorageItems(tableStorageAuth, settingsTable, partitionKey);
            if (list != null)
            {
                settings = JsonConvert.DeserializeObject<List<AzureDevOpsSettings>>(list.ToString());
            }
            return settings;
        }

        public List<GitHubSettings> GetGitHubSettings(TableStorageAuth tableStorageAuth, string settingsTable)
        {

            List<GitHubSettings> settings = null;
            string partitionKey = "GitHubSettings";
            JArray list = GetTableStorageItems(tableStorageAuth, settingsTable, partitionKey);
            if (list != null)
            {
                settings = JsonConvert.DeserializeObject<List<GitHubSettings>>(list.ToString());
            }
            return settings;
        }

        public async Task<bool> UpdateAzureDevOpsSetting(string patToken, TableStorageAuth tableStorageAuth, string settingsTable,
             string organization, string project, string repository, string branch, string buildName, string buildId, string resourceGroupName, int itemOrder)
        {
            string partitionKey = "AzureDevOpsSettings";
            string rowKey = CreateAzureDevOpsSettingsPartitionKey(organization, project, repository, buildName);
            AzureDevOpsSettings settings = new AzureDevOpsSettings
            {
                RowKey = rowKey,
                PatToken = patToken,
                Organization = organization,
                Project = project,
                Repository = repository,
                Branch = branch,
                BuildName = buildName,
                BuildId = buildId,
                ProductionResourceGroup = resourceGroupName,
                ItemOrder = itemOrder
            };

            string json = JsonConvert.SerializeObject(settings);
            AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, settingsTable);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> UpdateGitHubSetting(string clientId, string clientSecret, TableStorageAuth tableStorageAuth, string settingsTable,
             string owner, string repo, string branch, string workflowName, string workflowId, string resourceGroupName, int itemOrder)
        {
            string partitionKey = "GitHubSettings";
            string rowKey = CreateGitHubSettingsPartitionKey(owner, repo, workflowName);
            GitHubSettings settings = new GitHubSettings
            {
                RowKey = rowKey,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Owner = owner,
                Repo = repo,
                Branch = branch,
                WorkflowName = workflowName,
                WorkflowId = workflowId,
                ProductionResourceGroup = resourceGroupName,
                ItemOrder = itemOrder
            };

            string json = JsonConvert.SerializeObject(settings);
            AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, settingsTable);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> UpdateDevOpsMonitoringEvent(TableStorageAuth tableStorageAuth, MonitoringEvent monitoringEvent)
        {
            string partitionKey = monitoringEvent.PartitionKey;
            string rowKey = monitoringEvent.RowKey;
            string json = monitoringEvent.RequestBody;
            AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableMTTR);
            return await tableDA.SaveItem(newItem);
        }

    }
}
