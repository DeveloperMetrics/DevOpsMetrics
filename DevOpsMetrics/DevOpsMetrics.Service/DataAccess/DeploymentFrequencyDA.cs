using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class DeploymentFrequencyDA
    {
        public async Task<List<AzureDevOpsBuild>> GetAzureDevOpsDeployments(string patToken, string organization, string project, string branch, string buildId)
        {
            List<AzureDevOpsBuild> builds = new List<AzureDevOpsBuild>();
            string buildListResponse = await MessageUtility.SendAzureDevOpsMessage( $"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1&queryOrder=BuildQueryOrder,finishTimeDescending",patToken);
            if (string.IsNullOrEmpty(buildListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(buildListResponse);
                Newtonsoft.Json.Linq.JArray value = buildListObject.value;
                builds = JsonConvert.DeserializeObject<List<AzureDevOpsBuild>>(value.ToString());
            }
            //construct the Url to the build
            foreach (AzureDevOpsBuild item in builds)
            {
                item.url = $"https://dev.azure.com/{organization}/{project}/_build/results?buildId={item.id}&view=results";
            }
            //sort the list
            builds = builds.OrderBy(o => o.queueTime).ToList();
            return builds;
        }

        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(string patToken, string organization, string project, string branch, string buildId, int numberOfDays)
        {
            float deploymentsPerDay = 0;
            DeploymentFrequency deploymentFrequency = new DeploymentFrequency();

            ////Gets a list of builds
            //GET https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1         
            string buildListResponse = await MessageUtility.SendAzureDevOpsMessage($"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1&queryOrder=BuildQueryOrder,finishTimeDescending", patToken);
            //Console.WriteLine(buildListResponse);
            if (string.IsNullOrEmpty(buildListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(buildListResponse);
                Newtonsoft.Json.Linq.JArray value = buildListObject.value;
                IEnumerable<AzureDevOpsBuild> builds = JsonConvert.DeserializeObject<List<AzureDevOpsBuild>>(value.ToString());

                List<KeyValuePair<DateTime, DateTime>> dateList = new List<KeyValuePair<DateTime, DateTime>>();
                foreach (AzureDevOpsBuild item in builds)
                {
                    if (item.status == "completed" && item.sourceBranch == branch && item.queueTime > DateTime.Now.AddDays(-numberOfDays))
                    {
                        KeyValuePair<DateTime, DateTime> newItem = new KeyValuePair<DateTime, DateTime>(item.queueTime, item.queueTime);
                        dateList.Add(newItem);
                    }
                }

                deploymentsPerDay = deploymentFrequency.ProcessDeploymentFrequency(dateList, "", numberOfDays);
            }
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                DeploymentsPerDay = deploymentsPerDay,
                DeploymentsPerDayDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay)
            };
            return model;
        }

        public async Task<List<GitHubActionsRun>> GetGitHubDeployments(string clientId, string clientSecret, string owner, string repo, string branch, string workflowId)
        {
            List<GitHubActionsRun> deployments = new List<GitHubActionsRun>();
            string runListResponse = await MessageUtility.SendGitHubMessage($"https://api.github.com/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs", clientId, clientSecret);
            if (string.IsNullOrEmpty(runListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(runListResponse);
                Newtonsoft.Json.Linq.JArray workflow_runs = buildListObject.workflow_runs;
                deployments = JsonConvert.DeserializeObject<List<GitHubActionsRun>>(workflow_runs.ToString());
            }

            //sort the list
            deployments = deployments.OrderBy(o => o.created_at).ToList();

            return deployments;
        }

        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(string clientId, string clientSecret, string owner, string repo, string branch, string workflowId, int numberOfDays)
        {
            //Lists the workflows in a repository. 
            //GET /repos/:owner/:repo/actions/workflows

            //List all workflow runs for a workflow.
            //GET /repos/:owner/:repo/actions/workflows/:workflow_id/runs

            float deploymentsPerDay = 0;
            DeploymentFrequency deploymentFrequency = new DeploymentFrequency();
            string runListResponse = await MessageUtility.SendGitHubMessage($"https://api.github.com/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs", clientId, clientSecret);
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

                deploymentsPerDay = deploymentFrequency.ProcessDeploymentFrequency(dateList, "", numberOfDays);
            }
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                DeploymentsPerDay = deploymentsPerDay,
                DeploymentsPerDayDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay)
            };
            return model;
        }

    }
}
