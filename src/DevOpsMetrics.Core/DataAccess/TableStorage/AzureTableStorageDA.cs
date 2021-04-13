using DevOpsMetrics.Core.DataAccess.APIAccess;
using DevOpsMetrics.Core.Models.Azure;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Core.DataAccess.TableStorage
{
    public class AzureTableStorageDA : IAzureTableStorageDA
    {
        //Note that this can't be async due to performance issues with Azure Storage when you retrieve items
        public JArray GetTableStorageItemsFromStorage(TableStorageConfiguration tableStorageConfig, string tableName, string partitionKey)
        {
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableName);
            List<AzureStorageTableModel> items = tableDA.GetItems(partitionKey);
            JArray list = new JArray();
            foreach (AzureStorageTableModel item in items)
            {
                list.Add(JToken.Parse(item.Data));
            }
            return list;
        }

        public async Task<int> UpdateAzureDevOpsBuildsInStorage(string patToken, TableStorageConfiguration tableStorageConfig,
                string organization, string project, string branch, string buildName, string buildId,
                int numberOfDays, int maxNumberOfItems)
        {
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            JArray items = await api.GetAzureDevOpsBuildsJArray(patToken, organization, project);

            int itemsAdded = 0;
            TableStorageCommonDA tableBuildsDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableAzureDevOpsBuilds);
            TableStorageCommonDA tableChangeFailureRateDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableChangeFailureRate);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsBuild build = JsonConvert.DeserializeObject<AzureDevOpsBuild>(item.ToString());

                //Save the build information for builds
                if (build.status == "completed")
                {
                    string partitionKey = PartitionKeys.CreateBuildWorkflowPartitionKey(organization, project, buildName);
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
                    itemsAdded += await UpdateChangeFailureRate(tableChangeFailureRateDA, newBuild, PartitionKeys.CreateBuildWorkflowPartitionKey(organization, project, buildName));

                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateAzureDevOpsPullRequestsInStorage(string patToken, TableStorageConfiguration tableStorageConfig,
                string organization, string project, string repository,
                int numberOfDays, int maxNumberOfItems)
        {
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            JArray items = await api.GetAzureDevOpsPullRequestsJArray(patToken, organization, project, repository);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRs);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());

                string partitionKey = PartitionKeys.CreateAzureDevOpsPRPartitionKey(organization, project);
                string rowKey = pullRequest.PullRequestId;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }

                itemsAdded += await UpdateAzureDevOpsPullRequestCommitsInStorage(patToken, tableStorageConfig,
                    organization, project, repository, pullRequest.PullRequestId,
                    numberOfDays, maxNumberOfItems);
            }

            return itemsAdded;
        }

        public async Task<int> UpdateAzureDevOpsPullRequestCommitsInStorage(string patToken, TableStorageConfiguration tableStorageConfig,
                string organization, string project, string repository, string pullRequestId,
                int numberOfDays, int maxNumberOfItems)
        {
            AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
            JArray items = await api.GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repository, pullRequestId);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRCommits);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPRCommit pullRequestCommit = JsonConvert.DeserializeObject<AzureDevOpsPRCommit>(item.ToString());

                string partitionKey = PartitionKeys.CreateAzureDevOpsPRCommitPartitionKey(organization, project, pullRequestId);
                string rowKey = pullRequestCommit.commitId;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionRunsInStorage(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig,
                string owner, string repo, string branch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems)
        {
            GitHubAPIAccess api = new GitHubAPIAccess();
            JArray items = await api.GetGitHubActionRunsJArray(clientId, clientSecret, owner, repo, workflowId);

            int itemsAdded = 0;
            TableStorageCommonDA tableBuildDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableGitHubRuns);
            TableStorageCommonDA tableChangeFailureRateDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableChangeFailureRate);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubActionsRun build = JsonConvert.DeserializeObject<GitHubActionsRun>(item.ToString());

                //Save the build information for builds
                if (build.status == "completed")
                {
                    string partitionKey = PartitionKeys.CreateBuildWorkflowPartitionKey(owner, repo, workflowName);
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
                    itemsAdded += await UpdateChangeFailureRate(tableChangeFailureRateDA, newBuild, PartitionKeys.CreateBuildWorkflowPartitionKey(owner, repo, workflowName));
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

        public async Task<int> UpdateGitHubActionPullRequestsInStorage(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig,
                string owner, string repo, string branch,
                int numberOfDays, int maxNumberOfItems)
        {
            GitHubAPIAccess api = new GitHubAPIAccess();
            JArray items = await api.GetGitHubPullRequestsJArray(clientId, clientSecret, owner, repo, branch);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableGitHubPRs);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubPR pr = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());

                if (pr.state == "closed" & pr.merged_at != null)
                {
                    string partitionKey = PartitionKeys.CreateGitHubPRPartitionKey(owner, repo);
                    string rowKey = pr.number;
                    //Debug.WriteLine($"PartitionKey: {partitionKey}, RowKey: {rowKey}");
                    AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                    if (await tableDA.AddItem(newItem) == true)
                    {
                        itemsAdded++;
                    }
                    itemsAdded += await UpdateGitHubActionPullRequestCommitsInStorage(clientId, clientSecret, tableStorageConfig, owner, repo, pr.number);
                }
            }

            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionPullRequestCommitsInStorage(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig,
                string owner, string repo, string pull_number)
        {
            GitHubAPIAccess api = new GitHubAPIAccess();
            JArray items = await api.GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);

            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableGitHubPRCommits);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubCommit commit = JsonConvert.DeserializeObject<GitHubCommit>(item.ToString());

                string partitionKey = PartitionKeys.CreateGitHubPRCommitPartitionKey(owner, repo, pull_number);
                string rowKey = commit.sha;
                AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem) == true)
                {
                    itemsAdded++;
                }
            }

            return itemsAdded;
        }

        public List<AzureDevOpsSettings> GetAzureDevOpsSettingsFromStorage(TableStorageConfiguration tableStorageConfig, string settingsTable, string rowKey)
        {
            List<AzureDevOpsSettings> settings = null;
            string partitionKey = "AzureDevOpsSettings";
            JArray list = GetTableStorageItemsFromStorage(tableStorageConfig, settingsTable, partitionKey);
            if (list != null)
            {
                settings = JsonConvert.DeserializeObject<List<AzureDevOpsSettings>>(list.ToString());
            }
            if (rowKey != null)
            {
                return new List<AzureDevOpsSettings>
                {
                    settings.Where(x => x.RowKey == rowKey).FirstOrDefault()
                };
            }
            else
            {
                return settings;
            }
        }

        public List<GitHubSettings> GetGitHubSettingsFromStorage(TableStorageConfiguration tableStorageConfig, string settingsTable)
        {

            List<GitHubSettings> settings = null;
            string partitionKey = "GitHubSettings";
            JArray list = GetTableStorageItemsFromStorage(tableStorageConfig, settingsTable, partitionKey);
            if (list != null)
            {
                settings = JsonConvert.DeserializeObject<List<GitHubSettings>>(list.ToString());
            }
            return settings;
        }

        public async Task<bool> UpdateAzureDevOpsSettingInStorage(string patToken, TableStorageConfiguration tableStorageConfig, string settingsTable,
             string organization, string project, string repository, string branch, string buildName, string buildId, string resourceGroupName, int itemOrder)
        {
            string partitionKey = "AzureDevOpsSettings";
            string rowKey = PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository);

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
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, settingsTable);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> UpdateGitHubSettingInStorage(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig, string settingsTable,
             string owner, string repo, string branch, string workflowName, string workflowId, string resourceGroupName, int itemOrder)
        {
            string partitionKey = "GitHubSettings";
            string rowKey = PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo);
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
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, settingsTable);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> UpdateDevOpsMonitoringEventInStorage(TableStorageConfiguration tableStorageConfig, MonitoringEvent monitoringEvent)
        {
            string partitionKey = monitoringEvent.PartitionKey;
            string rowKey = monitoringEvent.RowKey;
            string json = monitoringEvent.RequestBody;
            AzureStorageTableModel newItem = new AzureStorageTableModel(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableMTTR);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> LogAzureDevOpsProcessorRunInStorage(TableStorageConfiguration tableStorageConfig, ProjectLog log)
        {
            AzureStorageTableModel newItem = new AzureStorageTableModel(log.PartitionKey, log.RowKey, log.Json);
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableLog);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> LogGitHubProcessorRunInStorage(TableStorageConfiguration tableStorageConfig, ProjectLog log)
        {
            AzureStorageTableModel newItem = new AzureStorageTableModel(log.PartitionKey, log.RowKey, log.Json);
            TableStorageCommonDA tableDA = new TableStorageCommonDA(tableStorageConfig, tableStorageConfig.TableLog);
            return await tableDA.SaveItem(newItem);
        }

    }
}
