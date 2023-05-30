using System.Collections.Generic;
using System.Linq;
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
    public class BuildsDA
    {
        public static async Task<List<AzureDevOpsBuild>> GetAzureDevOpsBuilds(string patToken, TableStorageConfiguration tableStorageConfig,
                string organization, string project, string buildName, bool useCache)
        {
            List<AzureDevOpsBuild> builds = new();
            JArray list = null;
            if (useCache)
            {
                //Get the builds from Azure storage
                AzureTableStorageDA daTableStorage = new();
                list = await daTableStorage.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsBuilds, PartitionKeys.CreateBuildWorkflowPartitionKey(organization, project, buildName));
            }
            else
            {
                //Get the builds from the Azure DevOps API
                AzureDevOpsAPIAccess api = new();
                list = await AzureDevOpsAPIAccess.GetAzureDevOpsBuildsJArray(patToken, organization, project);
            }
            if (list != null)
            {
                builds = JsonConvert.DeserializeObject<List<AzureDevOpsBuild>>(list.ToString());
                //We need to do some post processing and loop over the list to construct a usable url
                foreach (AzureDevOpsBuild item in builds)
                {
                    item.url = $"https://dev.azure.com/{organization}/{project}/_build/results?buildId={item.id}&view=results";
                }

                //sort the final list
                builds = builds.OrderBy(o => o.queueTime).ToList();
            }

            return builds;
        }

        public static async Task<List<GitHubActionsRun>> GetGitHubActionRuns(string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig,
                string owner, string repo, string workflowName, string workflowId, bool useCache)
        {
            List<GitHubActionsRun> runs = new();
            JArray list = null;
            if (useCache)
            {
                //Get the builds from Azure storage
                AzureTableStorageDA daTableStorage = new();
                list = await daTableStorage.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubRuns, PartitionKeys.CreateBuildWorkflowPartitionKey(owner, repo, workflowName));
            }
            else
            {
                //Get the builds from the GitHub API
                GitHubAPIAccess api = new();
                list = await GitHubAPIAccess.GetGitHubActionRunsJArray(clientId, clientSecret, owner, repo, workflowId);
            }
            if (list != null)
            {
                runs = JsonConvert.DeserializeObject<List<GitHubActionsRun>>(list.ToString());

                //sort the final list
                runs = runs.OrderBy(o => o.created_at).ToList();
            }

            return runs;
        }

    }
}
