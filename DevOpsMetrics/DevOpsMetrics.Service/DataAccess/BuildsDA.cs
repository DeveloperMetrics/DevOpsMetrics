using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class BuildsDA
    {
        public async Task<List<AzureDevOpsBuild>> GetAzureDevOpsBuilds(string patToken, string organization, string project, string branch, string buildId)
        {
            List<AzureDevOpsBuild> builds = new List<AzureDevOpsBuild>();
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1&queryOrder=BuildQueryOrder,finishTimeDescending";       
            string response = await MessageUtility.SendAzureDevOpsMessage(url, patToken);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = jsonObj.value;
                builds = JsonConvert.DeserializeObject<List<AzureDevOpsBuild>>(value.ToString());

                //construct and add in the Url to each build
                foreach (AzureDevOpsBuild item in builds)
                {
                    item.url = $"https://dev.azure.com/{organization}/{project}/_build/results?buildId={item.id}&view=results";
                }

                //sort the list
                builds = builds.OrderBy(o => o.queueTime).ToList();
            }


            return builds;
        }

        public async Task<List<GitHubActionsRun>> GetGitHubActionRuns(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowId)
        {
            List<GitHubActionsRun> runs = new List<GitHubActionsRun>();
            string url = $"https://api.github.com/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs?per_page=100";
            string response = await MessageUtility.SendGitHubMessage(url, clientId, clientSecret);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = jsonObj.workflow_runs;
                runs = JsonConvert.DeserializeObject<List<GitHubActionsRun>>(value.ToString());

                //sort the list
                runs = runs.OrderBy(o => o.created_at).ToList();
            }

            return runs;
        }

    }
}
