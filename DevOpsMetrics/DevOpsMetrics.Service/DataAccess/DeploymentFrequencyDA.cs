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
            if (getSampleData == false)
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
                    azureDevOpsBuilds = ProcessAzureDevOpsBuilds(azureDevOpsBuilds.ToList());
                    
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
                                    BuildDurationPercent = item.buildDurationPercent,
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
            else
            {
                DeploymentFrequencyModel model = new DeploymentFrequencyModel
                {
                    IsAzureDevOps = true,
                    DeploymentName = buildName,
                    BuildList = GetSampleAzureDevOpsBuilds(),
                    DeploymentsPerDayMetric = 10f,
                    DeploymentsPerDayMetricDescription = "Elite"
                };
                return model;
            }
        }

        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays)
        {

            if (getSampleData == false)
            {
                float deploymentsPerDay = 0;
                DeploymentFrequency deploymentFrequency = new DeploymentFrequency();
                List<Build> builds = new List<Build>();

                //Lists the workflows in a repository. 
                string url = $"https://api.github.com/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs";
                string runListResponse = await MessageUtility.SendGitHubMessage(url, clientId, clientSecret);
                if (string.IsNullOrEmpty(runListResponse) == false)
                {
                    dynamic buildListObject = JsonConvert.DeserializeObject(runListResponse);
                    Newtonsoft.Json.Linq.JArray workflow_runs = buildListObject.workflow_runs;
                    IEnumerable<GitHubActionsRun> gitHubRuns = JsonConvert.DeserializeObject<List<GitHubActionsRun>>(workflow_runs.ToString());
                    gitHubRuns = ProcessGitHubRuns(gitHubRuns.ToList());

                    List<KeyValuePair<DateTime, DateTime>> dateList = new List<KeyValuePair<DateTime, DateTime>>();
                    foreach (GitHubActionsRun item in gitHubRuns)
                    {
                        if (item.status == "completed" && item.head_branch == branch && item.created_at > DateTime.Now.AddDays(-numberOfDays))
                        {
                            KeyValuePair<DateTime, DateTime> newItem = new KeyValuePair<DateTime, DateTime>(item.created_at, item.created_at);
                            dateList.Add(newItem);
                            builds.Add(
                                new Build
                                {
                                    Id = item.run_number,
                                    Branch = item.head_branch,
                                    BuildNumber = item.run_number,
                                    StartTime = item.created_at,
                                    EndTime = item.updated_at,
                                    BuildDurationPercent = item.buildDurationPercent,
                                    Status = item.status,
                                    Url = item.html_url
                                }
                            );
                        }
                    }

                    deploymentsPerDay = deploymentFrequency.ProcessDeploymentFrequency(dateList, "", numberOfDays);
                }
                DeploymentFrequencyModel model = new DeploymentFrequencyModel
                {
                    IsAzureDevOps = false,
                    DeploymentName = workflowName,
                    BuildList = builds,
                    DeploymentsPerDayMetric = deploymentsPerDay,
                    DeploymentsPerDayMetricDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay)
                };
                return model;
            }
            else
            {
                DeploymentFrequencyModel model = new DeploymentFrequencyModel
                {
                    IsAzureDevOps = false,
                    DeploymentName = workflowName,
                    BuildList = GetSampleGitHubBuilds(),
                    DeploymentsPerDayMetric = 10f,
                    DeploymentsPerDayMetricDescription = "Elite"
                };
                return model;
            }
        }

        private static List<AzureDevOpsBuild> ProcessAzureDevOpsBuilds(List<AzureDevOpsBuild> azList)
        {
            float maxBuildDuration = 0f;
            foreach (AzureDevOpsBuild item in azList)
            {
                if (item.buildDuration > maxBuildDuration)
                {
                    maxBuildDuration = item.buildDuration;
                }
            }
            foreach (AzureDevOpsBuild item in azList)
            {
                float interiumResult = ((item.buildDuration / maxBuildDuration) * 100f);
                item.buildDurationPercent = Utility.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
            }
            return azList;
        }

        private static List<GitHubActionsRun> ProcessGitHubRuns(List<GitHubActionsRun> ghList)
        {
            float maxBuildDuration = 0f;
            foreach (GitHubActionsRun item in ghList)
            {
                if (item.buildDuration > maxBuildDuration)
                {
                    maxBuildDuration = item.buildDuration;
                }
            }
            foreach (GitHubActionsRun item in ghList)
            {
                float interiumResult = ((item.buildDuration / maxBuildDuration) * 100f);
                item.buildDurationPercent = Utility.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
            }
            return ghList;
        }

        private List<Build> GetSampleAzureDevOpsBuilds()
        {
            List<Build> results = new List<Build>();
            Build item1 = new Build
            {
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                BuildNumber = "1",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/1"
            };
            results.Add(item1);
            results.Add(item1);
            Build item2 = new Build
            {
                StartTime = DateTime.Now.AddDays(-5).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-5).AddMinutes(0),
                BuildNumber = "2",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/2"
            };
            results.Add(item2);
            results.Add(item2);
            Build item3 = new Build
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-1),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                BuildNumber = "3",
                Branch = "master",
                Status = "failed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/3"
            };
            results.Add(item3);
            Build item4 = new Build
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                BuildNumber = "4",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/4"
            };
            results.Add(item4);
            results.Add(item4);
            Build item5 = new Build
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-7),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                BuildNumber = "5",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/5"
            };
            results.Add(item5);
            results.Add(item5);
            Build item6 = new Build
            {
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                BuildNumber = "6",
                Branch = "master",
                Status = "inProgress",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/6"
            };
            results.Add(item6);

            return results;
        }

        private List<Build> GetSampleGitHubBuilds()
        {
            List<Build> results = new List<Build>();
            Build item1 = new Build
            {
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-12),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                BuildNumber = "1",
                Branch = "master",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/1"
            };
            results.Add(item1);
            Build item2 = new Build
            {
                StartTime = DateTime.Now.AddDays(-6).AddMinutes(-16),
                EndTime = DateTime.Now.AddDays(-6).AddMinutes(0),
                BuildNumber = "2",
                Branch = "master",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/2"
            };
            results.Add(item2);
            results.Add(item2);
            Build item3 = new Build
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-9),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                BuildNumber = "3",
                Branch = "master",
                Status = "failed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/3"
            };
            results.Add(item3);
            Build item4 = new Build
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-10),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                BuildNumber = "4",
                Branch = "master",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/4"
            };
            results.Add(item4);
            results.Add(item4);
            Build item5 = new Build
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-11),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                BuildNumber = "5",
                Branch = "master",
                Status = "failed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/5"
            };
            results.Add(item5);
            results.Add(item5);
            Build item6 = new Build
            {
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(-8),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                BuildNumber = "6",
                Branch = "master",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/6"
            };
            results.Add(item6);
            results.Add(item6);

            return results;
        }


    }
}
