using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.APIAccess;
using DevOpsMetrics.Core.Models.Azure;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.DataAccess.TableStorage
{
    public class AzureTableStorageDA : IAzureTableStorageDA
    {
        /// <summary>
        /// Generic function to read data from Azure table storage
        /// Note that this can't be async due to performance issues with Azure Storage when you retrieve items
        /// </summary>
        /// <param name="tableStorageConfig">Names of all possible tables to query</param>
        /// <param name="tableName">Table name to query</param>
        /// <param name="partitionKey">Partition key to filter by</param>
        /// <param name="includePartitionAndRowKeys">Include Partition and row key metadata (for debugging)</param>
        /// <returns></returns>
        public async Task<JArray> GetTableStorageItemsFromStorage(TableStorageConfiguration tableStorageConfig, string tableName, string partitionKey, bool includePartitionAndRowKeys = false)
        {
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableName);
            List<AzureStorageTableModel> items = await tableDA.GetItems(partitionKey);
            if (items == null)
            {
                items = new();
            }
            JArray list = new();
            foreach (AzureStorageTableModel item in items)
            {
                if (includePartitionAndRowKeys)
                {
                    string data = item.Data?.ToString();
                    list.Add(
                        new JObject(
                            new JProperty("PartitionKey", item.PartitionKey),
                                new JProperty("RowKey", item.RowKey),
                                new JProperty("Data", data))
                    );
                }
                else
                {
                    list.Add(JToken.Parse(item.Data));
                }
            }
            return list;
        }

        public async Task<List<AzureDevOpsSettings>> GetAzureDevOpsSettingsFromStorage(TableStorageConfiguration tableStorageConfig, string settingsTable, string rowKey)
        {
            List<AzureDevOpsSettings> settings = null;
            string partitionKey = "AzureDevOpsSettings";
            JArray list = await GetTableStorageItemsFromStorage(tableStorageConfig, settingsTable, partitionKey);
            if (list != null)
            {
                settings = JsonConvert.DeserializeObject<List<AzureDevOpsSettings>>(list.ToString());
            }
            if (rowKey != null)
            {
                return new List<AzureDevOpsSettings>
                {
                    settings.Where(x => x.RowKey.ToLower() == rowKey.ToLower()).FirstOrDefault()
                };
            }
            else
            {
                return settings;
            }
        }

        public async Task<List<GitHubSettings>> GetGitHubSettingsFromStorage(TableStorageConfiguration tableStorageConfig, string settingsTable, string rowKey)
        {
            List<GitHubSettings> settings = null;
            string partitionKey = "GitHubSettings";
            JArray list = await GetTableStorageItemsFromStorage(tableStorageConfig, settingsTable, partitionKey);
            if (list != null)
            {
                settings = JsonConvert.DeserializeObject<List<GitHubSettings>>(list.ToString());
            }
            if (rowKey != null)
            {
                return new List<GitHubSettings>
                {
                    settings.Where(x => x.RowKey.ToLower() == rowKey.ToLower()).FirstOrDefault()
                };
            }
            else
            {
                return settings;
            }
        }

        public async Task<List<ProjectLog>> GetProjectLogsFromStorage(TableStorageConfiguration tableStorageConfig, string partitionKey)
        {
            List<ProjectLog> logs = null;
            JArray list = await GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableLog, partitionKey, true);
            if (list != null)
            {
                logs = JsonConvert.DeserializeObject<List<ProjectLog>>(list.ToString());
            }

            return logs;
        }

        public async Task<int> UpdateAzureDevOpsBuildsInStorage(string patToken, TableStorageConfiguration tableStorageConfig,
                string organization, string project, string branch, string buildName, string buildId,
                int numberOfDays, int maxNumberOfItems)
        {
            JArray items = await AzureDevOpsAPIAccess.GetAzureDevOpsBuildsJArray(patToken, organization, project);
            if (items == null)
            {
                items = new();
            }
            int itemsAdded = 0;
            TableStorageCommonDA tableBuildsDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableAzureDevOpsBuilds);
            TableStorageCommonDA tableChangeFailureRateDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableChangeFailureRate);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsBuild build = JsonConvert.DeserializeObject<AzureDevOpsBuild>(item.ToString());

                //Save the build information for builds
                if (build.status == "completed")
                {
                    string partitionKey = PartitionKeys.CreateBuildWorkflowPartitionKey(organization, project, buildName);
                    string rowKey = build.buildNumber;
                    AzureStorageTableModel newItem = new(partitionKey, rowKey, item.ToString());
                    if (await tableBuildsDA.AddItem(newItem))
                    {
                        itemsAdded++;
                    }

                    //Save the build information for change failure rate
                    ChangeFailureRateBuild newBuild = new()
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
            JArray items = await AzureDevOpsAPIAccess.GetAzureDevOpsPullRequestsJArray(patToken, organization, project, repository);
            if (items == null)
            {
                items = new();
            }
            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableAzureDevOpsPRs);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());

                string partitionKey = PartitionKeys.CreateAzureDevOpsPRPartitionKey(organization, project);
                string rowKey = pullRequest.PullRequestId;
                AzureStorageTableModel newItem = new(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem))
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
            JArray items = await AzureDevOpsAPIAccess.GetAzureDevOpsPullRequestCommitsJArray(patToken, organization, project, repository, pullRequestId);
            if (items == null)
            {
                items = new();
            }
            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableAzureDevOpsPRCommits);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsPRCommit pullRequestCommit = JsonConvert.DeserializeObject<AzureDevOpsPRCommit>(item.ToString());

                string partitionKey = PartitionKeys.CreateAzureDevOpsPRCommitPartitionKey(organization, project, pullRequestId);
                string rowKey = pullRequestCommit.commitId;
                AzureStorageTableModel newItem = new(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem))
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
            JArray items = await GitHubAPIAccess.GetGitHubActionRunsJArray(clientId, clientSecret, owner, repo, workflowId);
            if (items == null)
            {
                items = new();
            }
            Debug.WriteLine($"{items.Count} builds found for {owner}/{repo}/{workflowName}");

            int itemsAdded = 0;
            TableStorageCommonDA tableBuildDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableGitHubRuns);
            TableStorageCommonDA tableChangeFailureRateDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableChangeFailureRate);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubActionsRun build = JsonConvert.DeserializeObject<GitHubActionsRun>(item.ToString());

                //Save the build information for builds
                if (build.status == "completed")
                {
                    string partitionKey = PartitionKeys.CreateBuildWorkflowPartitionKey(owner, repo, workflowName);
                    string rowKey = build.run_number;
                    AzureStorageTableModel newItem = new(partitionKey, rowKey, item.ToString());
                    if (await tableBuildDA.AddItem(newItem))
                    {
                        itemsAdded++;
                    }
                    //Debug.WriteLine($"Processing build {build.run_number} with items adding={itemsAdded}");

                    //Save the build information for change failure rate
                    ChangeFailureRateBuild newBuild = new()
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
                    //Debug.WriteLine($"UpdateChangeFailureRate for build {build.run_number} with items adding={itemsAdded}");

                }
            }
            Debug.WriteLine($"{items.Count} builds updated for {owner}/{repo}/{workflowName}");
            return itemsAdded;
        }

        public async Task<int> UpdateChangeFailureRate(TableStorageCommonDA tableChangeFailureRateDA, ChangeFailureRateBuild newBuild, string partitionKey, bool forceUpdate = false)
        {
            int itemsAdded = 0;
            string rowKey = newBuild.Id;
            string json = JsonConvert.SerializeObject(newBuild);
            AzureStorageTableModel newItem = new(partitionKey, rowKey, json);
            if (await tableChangeFailureRateDA.AddItem(newItem, forceUpdate))
            {
                itemsAdded++;
            }
            return itemsAdded;
        }

        public async Task<int> UpdateGitHubActionPullRequestsInStorage(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig,
                string owner, string repo, string branch,
                int numberOfDays, int maxNumberOfItems)
        {
            JArray items = await GitHubAPIAccess.GetGitHubPullRequestsJArray(clientId, clientSecret, owner, repo, branch);
            if (items == null)
            {
                items = new();
            }
            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableGitHubPRs);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubPR pr = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());

                if (pr.state == "closed" && pr.merged_at != null)
                {
                    string partitionKey = PartitionKeys.CreateGitHubPRPartitionKey(owner, repo);
                    string rowKey = pr.number;
                    string json = item.ToString();
                    //Debug.WriteLine($"PartitionKey: {partitionKey}, RowKey: {rowKey}, Length: {json.Length}");

                    if (item.ToString().Length > (1024 * 32)) //1024 x 32 is the column limit
                    {
                        //Need to strip out the body for large PR's (particularly some Dependabot PR's have very long bodies)
                        JObject o = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(json);
                        o.Property("body").Remove();
                        json = o.ToString();
                    }
                    AzureStorageTableModel newItem = new(partitionKey, rowKey, json);
                    if (await tableDA.AddItem(newItem))
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
            JArray items = await GitHubAPIAccess.GetGitHubPullRequestCommitsJArray(clientId, clientSecret, owner, repo, pull_number);
            if (items == null)
            {
                items = new();
            }
            int itemsAdded = 0;
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableGitHubPRCommits);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubCommit commit = JsonConvert.DeserializeObject<GitHubCommit>(item.ToString());

                string partitionKey = PartitionKeys.CreateGitHubPRCommitPartitionKey(owner, repo, pull_number);
                string rowKey = commit.sha;
                AzureStorageTableModel newItem = new(partitionKey, rowKey, item.ToString());
                if (await tableDA.AddItem(newItem))
                {
                    itemsAdded++;
                }
            }

            return itemsAdded;
        }

        public async Task<bool> UpdateAzureDevOpsSettingInStorage(TableStorageConfiguration tableStorageConfig, string settingsTable,
             string organization, string project, string repository, string branch, string buildName, string buildId, string resourceGroupName,
             int itemOrder, bool showSetting)
        {
            string partitionKey = "AzureDevOpsSettings";
            string rowKey = PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository);

            AzureDevOpsSettings settings = new()
            {
                RowKey = rowKey,
                Organization = organization,
                Project = project,
                Repository = repository,
                Branch = branch,
                BuildName = buildName,
                BuildId = buildId,
                ProductionResourceGroup = resourceGroupName,
                ItemOrder = itemOrder,
                ShowSetting = showSetting
            };

            string json = JsonConvert.SerializeObject(settings);
            AzureStorageTableModel newItem = new(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, settingsTable);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> UpdateGitHubSettingInStorage(TableStorageConfiguration tableStorageConfig, string settingsTable,
             string owner, string repo, string branch, string workflowName, string workflowId, string resourceGroupName,
             int itemOrder, bool showSetting)
        {
            string partitionKey = "GitHubSettings";
            string rowKey = PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo);
            GitHubSettings settings = new()
            {
                RowKey = rowKey,
                Owner = owner,
                Repo = repo,
                Branch = branch,
                WorkflowName = workflowName,
                WorkflowId = workflowId,
                ProductionResourceGroup = resourceGroupName,
                ItemOrder = itemOrder,
                ShowSetting = showSetting
            };

            string json = JsonConvert.SerializeObject(settings);
            AzureStorageTableModel newItem = new(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, settingsTable);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> UpdateDevOpsMonitoringEventInStorage(TableStorageConfiguration tableStorageConfig, MonitoringEvent monitoringEvent)
        {
            string partitionKey = monitoringEvent.PartitionKey;
            string rowKey = monitoringEvent.RowKey;
            string json = monitoringEvent.RequestBody;
            AzureStorageTableModel newItem = new(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableMTTR);
            return await tableDA.SaveItem(newItem);
        }

        public async Task<bool> UpdateProjectLogInStorage(TableStorageConfiguration tableStorageConfig, ProjectLog log)
        {
            AzureStorageTableModel newItem = new(log.PartitionKey, log.RowKey, log.Json);
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableLog);
            return await tableDA.SaveItem(newItem);
        }

        public static async Task<bool> UpdateDORASummaryItem(TableStorageConfiguration tableStorageConfig,
            string owner, string project, string repo, DORASummaryItem DORASummaryItem)
        {
            string partitionKey = owner;
            string rowKey;
            if (string.IsNullOrEmpty(project))
            {
                rowKey = repo;
            }
            else
            {
                rowKey = project;
            }
            string json = JsonConvert.SerializeObject(DORASummaryItem);
            AzureStorageTableModel newItem = new(partitionKey, rowKey, json);
            TableStorageCommonDA tableDA = new(tableStorageConfig.StorageAccountConnectionString, tableStorageConfig.TableDORASummaryItem);
            return await tableDA.SaveItem(newItem);
        }

    }
}
