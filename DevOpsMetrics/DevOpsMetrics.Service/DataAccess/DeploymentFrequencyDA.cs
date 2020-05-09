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
    public class DeploymentFrequencyDA
    {
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays)
        {
            float deploymentsPerDay = 0;
            DeploymentFrequency deploymentFrequency = new DeploymentFrequency();
            List<Build> builds = new List<Build>();

            ////Gets a list of builds
            //GET https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1      
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/build/builds?api-version=5.1&queryOrder=BuildQueryOrder,finishTimeDescending";
            string buildListResponse = await MessageUtility.SendAzureDevOpsMessage(url, patToken);
            //Console.WriteLine(buildListResponse);
            if (string.IsNullOrEmpty(buildListResponse) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(buildListResponse);
                Newtonsoft.Json.Linq.JArray value = buildListObject.value;
                IEnumerable<AzureDevOpsBuild> azureDevOpsBuilds = JsonConvert.DeserializeObject<List<AzureDevOpsBuild>>(value.ToString());

                List<KeyValuePair<DateTime, DateTime>> dateList = new List<KeyValuePair<DateTime, DateTime>>();
                foreach (AzureDevOpsBuild item in azureDevOpsBuilds)
                {
                    //Only return completed builds on the target branch
                    if (item.status == "completed" && item.sourceBranch == branch && item.queueTime > DateTime.Now.AddDays(-numberOfDays))
                    {
                        KeyValuePair<DateTime, DateTime> newItem = new KeyValuePair<DateTime, DateTime>(item.queueTime, item.queueTime);
                        dateList.Add(newItem);
                        builds.Add(
                            new Build
                            {
                                Id = item.id,
                                Branch = item.sourceBranch,
                                BuildNumber = item.buildNumber,
                                StartTime = item.queueTime,
                                EndTime = item.finishTime,
                                Status = item.status,
                                Url = item.url
                            }
                        );
                    }
                }

                deploymentsPerDay = deploymentFrequency.ProcessDeploymentFrequency(dateList, "", numberOfDays);
            }

            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                IsAzureDevOps = true,
                DeploymentName = buildName,
                BuildList = builds,
                DeploymentsPerDayMetric = deploymentsPerDay,
                DeploymentsPerDayMetricDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay)
            };
            return model;
        }

        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowId, int numberOfDays)
        {
            //Lists the workflows in a repository. 
            //GET /repos/:owner/:repo/actions/workflows

            //List all workflow runs for a workflow.
            //GET /repos/:owner/:repo/actions/workflows/:workflow_id/runs

            float deploymentsPerDay = 0;
            DeploymentFrequency deploymentFrequency = new DeploymentFrequency();
            string url = $"https://api.github.com/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs";
            string runListResponse = await MessageUtility.SendGitHubMessage(url, clientId, clientSecret);
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
                IsAzureDevOps = false,
                //DeploymentName = buildName,
                //BuildList = builds,
                DeploymentsPerDayMetric = deploymentsPerDay,
                DeploymentsPerDayMetricDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay)
            };
            return model;
        }

    }
}
