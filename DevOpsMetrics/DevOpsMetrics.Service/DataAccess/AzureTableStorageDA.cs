using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class AzureTableStorageDA
    {

        public async Task<int> UpdateAzureDevOpsBuilds(string patToken, string accountName, string accessKey, string tableName, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
        {
            BuildsDA da = new BuildsDA();
            Newtonsoft.Json.Linq.JArray items = await da.GetAzureDevOpsBuildsJArray(patToken, organization, project, branch, buildId);

            int itemsAdded = 0;
            TableStorageDA tableDA = new TableStorageDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                AzureDevOpsBuild build = JsonConvert.DeserializeObject<AzureDevOpsBuild>(item.ToString());

                if (build.status == "completed")
                {
                    string partitionKey = buildName;
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

        public async Task<int> UpdateGitHubActionRuns(string clientId, string clientSecret, string accountName, string accessKey, string tableName, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            BuildsDA da = new BuildsDA();
            Newtonsoft.Json.Linq.JArray items = await da.GetGitHubActionRunsJArray(clientId, clientSecret, owner, repo, branch, workflowId);

            int itemsAdded = 0;
            TableStorageDA tableDA = new TableStorageDA(accountName, accessKey, tableName);
            //Check each build to see if it's in storage, adding the items not in storage
            foreach (JToken item in items)
            {
                GitHubActionsRun build = JsonConvert.DeserializeObject<GitHubActionsRun>(item.ToString());

                if (build.status == "completed")
                {
                    string partitionKey = workflowName;
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

    }
}
