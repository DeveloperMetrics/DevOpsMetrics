using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class GitHubDeploymentFrequencyDA
    {
        public async Task<List<GitHubActionsRun>> GetDeployments(string owner, string repo, string branch, string workflowId)
        {
            List<GitHubActionsRun> deployments = new List<GitHubActionsRun>();
            string runListResponse = await SendGitHubMessage($"repos/{owner}/{repo}/actions/workflows/{workflowId}/runs", "https://api.github.com/");
            if (string.IsNullOrEmpty(runListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(runListResponse);
                Newtonsoft.Json.Linq.JArray workflow_runs = buildListObject.workflow_runs;
                deployments = JsonConvert.DeserializeObject<List<GitHubActionsRun>>(workflow_runs.ToString());
            }

            return deployments;
        }

        public async Task<float> GetDeploymentFrequency(string owner, string repo, string branch, string workflowId, int numberOfDays)
        {
            //Lists the workflows in a repository. 
            //GET /repos/:owner/:repo/actions/workflows

            //List all workflow runs for a workflow.
            //GET /repos/:owner/:repo/actions/workflows/:workflow_id/runs

            float deploymentFrequencyResult = 0;
            string runListResponse = await SendGitHubMessage($"repos/{owner}/{repo}/actions/workflows/{workflowId}/runs", "https://api.github.com/");
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

        public async Task<string> SendGitHubMessage(string url, string baseURL)
        {
            string responseBody = "";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DevOpsMetrics", "0.1"));

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return responseBody;
        }
    }
}
