using DevOpsMetrics.Service.DataAccess.APIAccess;
using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class BuildsDA
    {
        public async Task<List<AzureDevOpsBuild>> GetAzureDevOpsBuilds(string patToken, TableStorageAuth tableStorageAuth,
                string organization, string project, string buildName, bool useCache)
        {
            List<AzureDevOpsBuild> builds = new List<AzureDevOpsBuild>();
            Newtonsoft.Json.Linq.JArray list = null;
            if (useCache == true)
            {
                //Get the builds from Azure storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                list = daTableStorage.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableAzureDevOpsBuilds, daTableStorage.CreateBuildWorkflowPartitionKey(organization, project, buildName));
            }
            else
            {
                //Get the builds from the Azure DevOps API
                AzureDevOpsAPIAccess api = new AzureDevOpsAPIAccess();
                list = await api.GetAzureDevOpsBuildsJArray(patToken, organization, project);
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

        public async Task<List<GitHubActionsRun>> GetGitHubActionRuns(string clientId, string clientSecret, TableStorageAuth tableStorageAuth,
                string owner, string repo, string workflowName, string workflowId, bool useCache)
        {
            List<GitHubActionsRun> runs = new List<GitHubActionsRun>();
            Newtonsoft.Json.Linq.JArray list = null;
            if (useCache == true)
            {
                //Get the builds from Azure storage
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                list = daTableStorage.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableGitHubRuns, daTableStorage.CreateBuildWorkflowPartitionKey(owner, repo, workflowName));
            }
            else
            {
                //Get the builds from the GitHub API
                GitHubAPIAccess api = new GitHubAPIAccess();
                list = await api.GetGitHubActionRunsJArray(clientId, clientSecret, owner, repo, workflowId);
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
