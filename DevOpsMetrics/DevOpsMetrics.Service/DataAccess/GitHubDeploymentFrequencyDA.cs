using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class GitHubDeploymentFrequencyDA
    {
        public async Task<float> GetDeploymentFrequency(string owner, string repo, string branch, string workflowId, int numberOfDays)
        {
            //Lists the workflows in a repository. 
            //GET /repos/:owner/:repo/actions/workflows

            //List all workflow runs for a workflow.
            //GET /repos/:owner/:repo/actions/workflows/:workflow_id/runs

            string runListResponse = await Base.SendGitHubMessage($"repos/{owner}/{repo}/actions/workflows/{workflowId}/runs", "https://api.github.com/");
            Console.WriteLine(runListResponse);
            float deploymentFrequencyResult = 0;

            if (string.IsNullOrEmpty(runListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(runListResponse);
                Newtonsoft.Json.Linq.JArray workflow_runs = buildListObject.workflow_runs;
                IEnumerable<GitHubActionsRun> runs = JsonConvert.DeserializeObject<List<GitHubActionsRun>>(workflow_runs.ToString());

                List<KeyValuePair<DateTime, DateTime>> dateList = new List<KeyValuePair<DateTime, DateTime>>();
                foreach (GitHubActionsRun item in runs)
                {
                    if (item.status == "completed" && item.head_branch == branch && item.created_at > DateTime.Now.AddDays(-numberOfDays))
                    {
                        KeyValuePair<DateTime, DateTime> newItem = new KeyValuePair<DateTime, DateTime>(item.created_at, item.created_at);
                        dateList.Add(newItem);
                    }
                }

                DeploymentFrequency deploymentFrequency = new DeploymentFrequency();
                deploymentFrequencyResult = deploymentFrequency.ProcessDeploymentFrequency(dateList, "", numberOfDays);
            }
            return deploymentFrequencyResult;
        }
    }
}
