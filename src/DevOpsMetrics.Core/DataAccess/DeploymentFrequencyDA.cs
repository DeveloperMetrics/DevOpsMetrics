using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.Common;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;

namespace DevOpsMetrics.Core.DataAccess
{
    public class DeploymentFrequencyDA
    {
        public static async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, TableStorageConfiguration tableStorageConfig,
                string organization, string project, string branch, string buildName,
                int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ListUtility<Build> utility = new();
            DeploymentFrequency deploymentFrequency = new();
            if (getSampleData == false)
            {
                //Get a list of builds
                List<AzureDevOpsBuild> azureDevOpsBuilds = await BuildsDA.GetAzureDevOpsBuilds(patToken, tableStorageConfig, organization, project, buildName, useCache);
                if (azureDevOpsBuilds != null)
                {
                    //Translate the Azure DevOps build to a generic build object
                    List<Build> builds = new();
                    foreach (AzureDevOpsBuild item in azureDevOpsBuilds)
                    {
                        //Only return completed builds on the target branch, within the targeted date range
                        if (item.status == "completed" && item.sourceBranch == branch && item.queueTime > DateTime.Now.AddDays(-numberOfDays))
                        {
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

                    //Get the total builds used in the calculation
                    int buildTotal = builds.Count;

                    //then build the calcuation, loading the dates into a date array
                    List<KeyValuePair<DateTime, DateTime>> dateList = new();
                    foreach (Build item in builds)
                    {
                        KeyValuePair<DateTime, DateTime> newItem = new(item.StartTime, item.EndTime);
                        dateList.Add(newItem);
                    }

                    //then build the calcuation, loading the dates into a date array
                    float deploymentsPerDay;
                    deploymentsPerDay = deploymentFrequency.ProcessDeploymentFrequency(dateList, numberOfDays);

                    //Filter the results to return the last n (maxNumberOfItems), to return to the UI
                    builds = utility.GetLastNItems(builds, maxNumberOfItems);
                    //Find the max build duration
                    float maxBuildDuration = 0f;
                    foreach (Build item in builds)
                    {
                        if (item.BuildDuration > maxBuildDuration)
                        {
                            maxBuildDuration = item.BuildDuration;
                        }
                    }
                    //Calculate the percent scaling
                    foreach (Build item in builds)
                    {
                        float interiumResult = ((item.BuildDuration / maxBuildDuration) * 100f);
                        item.BuildDurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                    }

                    //Return the completed model
                    DeploymentFrequencyModel model = new()
                    {
                        TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                        DeploymentName = buildName,
                        BuildList = builds,
                        DeploymentsPerDayMetric = deploymentsPerDay,
                        DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay),
                        NumberOfDays = numberOfDays,
                        MaxNumberOfItems = builds.Count,
                        TotalItems = buildTotal
                    };
                    return model;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //Get sample data
                List<Build> builds = utility.GetLastNItems(GetSampleAzureDevOpsBuilds(), maxNumberOfItems);
                DeploymentFrequencyModel model = new()
                {
                    TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                    DeploymentName = buildName,
                    BuildList = builds,
                    DeploymentsPerDayMetric = 10f,
                    DeploymentsPerDayMetricDescription = "High",
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = builds.Count,
                    TotalItems = builds.Count
                };
                return model;
            }
        }

        public static async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig,
                string owner, string repo, string branch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ListUtility<Build> utility = new();
            DeploymentFrequency deploymentFrequency = new();
            if (getSampleData == false)
            {
                //Get a list of builds
                List<GitHubActionsRun> gitHubRuns = await BuildsDA.GetGitHubActionRuns(clientId, clientSecret, tableStorageConfig, owner, repo, workflowName, workflowId, useCache);
                if (gitHubRuns != null)
                {
                    //Translate the GitHub build to a generic build object
                    List<Build> builds = new();
                    foreach (GitHubActionsRun item in gitHubRuns)
                    {
                        //Only return completed builds on the target branch, within the targeted date range
                        if (item.status == "completed" && item.head_branch == branch && item.created_at > DateTime.Now.AddDays(-numberOfDays))
                        {
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

                    //Get the total builds used in the calculation
                    int buildTotal = builds.Count;

                    //then build the calcuation, loading the dates into a date array
                    List<KeyValuePair<DateTime, DateTime>> dateList = new();
                    foreach (Build item in builds)
                    {
                        KeyValuePair<DateTime, DateTime> newItem = new(item.StartTime, item.EndTime);
                        dateList.Add(newItem);
                    }

                    //then build the calcuation, loading the dates into a date array
                    float deploymentsPerDay;
                    deploymentsPerDay = deploymentFrequency.ProcessDeploymentFrequency(dateList, numberOfDays);

                    //Filter the results to return the last n (maxNumberOfItems), to return to the UI
                    builds = utility.GetLastNItems(builds, maxNumberOfItems);
                    //Find the max build duration
                    float maxBuildDuration = 0f;
                    foreach (Build item in builds)
                    {
                        if (item.BuildDuration > maxBuildDuration)
                        {
                            maxBuildDuration = item.BuildDuration;
                        }
                    }
                    //Calculate the percent scaling
                    foreach (Build item in builds)
                    {
                        float interiumResult = ((item.BuildDuration / maxBuildDuration) * 100f);
                        item.BuildDurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                    }

                    //Return the completed model
                    DeploymentFrequencyModel model = new()
                    {
                        TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                        DeploymentName = workflowName,
                        BuildList = builds,
                        DeploymentsPerDayMetric = deploymentsPerDay,
                        DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay),
                        NumberOfDays = numberOfDays,
                        MaxNumberOfItems = builds.Count,
                        TotalItems = buildTotal
                    };
                    return model;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<Build> builds = utility.GetLastNItems(GetSampleGitHubBuilds(), maxNumberOfItems);
                DeploymentFrequencyModel model = new()
                {
                    TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                    DeploymentName = workflowName,
                    BuildList = builds,
                    DeploymentsPerDayMetric = 10f,
                    DeploymentsPerDayMetricDescription = "High",
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = builds.Count,
                    TotalItems = builds.Count
                };
                return model;
            }
        }

        //Return a sample dataset to help with testing
        private static List<Build> GetSampleAzureDevOpsBuilds()
        {
            List<Build> results = new();
            Build item1 = new()
            {
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                BuildDurationPercent = 70,
                BuildNumber = "1",
                Branch = "main",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/1"
            };
            results.Add(item1);
            results.Add(item1);
            Build item2 = new()
            {
                StartTime = DateTime.Now.AddDays(-5).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-5).AddMinutes(0),
                BuildDurationPercent = 40,
                BuildNumber = "2",
                Branch = "main",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/2"
            };
            results.Add(item2);
            results.Add(item2);
            Build item3 = new()
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-1),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                BuildDurationPercent = 20,
                BuildNumber = "3",
                Branch = "main",
                Status = "failed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/3"
            };
            results.Add(item3);
            Build item4 = new()
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                BuildDurationPercent = 50,
                BuildNumber = "4",
                Branch = "main",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/4"
            };
            results.Add(item4);
            results.Add(item4);
            Build item5 = new()
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-7),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                BuildDurationPercent = 60,
                BuildNumber = "5",
                Branch = "main",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/5"
            };
            results.Add(item5);
            results.Add(item5);
            Build item6 = new()
            {
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                BuildDurationPercent = 70,
                BuildNumber = "6",
                Branch = "main",
                Status = "inProgress",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/6"
            };
            results.Add(item6);

            return results;
        }

        private static List<Build> GetSampleGitHubBuilds()
        {
            List<Build> results = new();
            Build item1 = new()
            {
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-12),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                BuildDurationPercent = 70,
                BuildNumber = "1",
                Branch = "main",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/1"
            };
            results.Add(item1);
            Build item2 = new()
            {
                StartTime = DateTime.Now.AddDays(-6).AddMinutes(-16),
                EndTime = DateTime.Now.AddDays(-6).AddMinutes(0),
                BuildDurationPercent = 90,
                BuildNumber = "2",
                Branch = "main",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/2"
            };
            results.Add(item2);
            results.Add(item2);
            Build item3 = new()
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-9),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                BuildDurationPercent = 40,
                BuildNumber = "3",
                Branch = "main",
                Status = "failed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/3"
            };
            results.Add(item3);
            Build item4 = new()
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-10),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                BuildDurationPercent = 45,
                BuildNumber = "4",
                Branch = "main",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/4"
            };
            results.Add(item4);
            results.Add(item4);
            Build item5 = new()
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-11),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                BuildDurationPercent = 50,
                BuildNumber = "5",
                Branch = "main",
                Status = "failed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/5"
            };
            results.Add(item5);
            results.Add(item5);
            Build item6 = new()
            {
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(-8),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                BuildDurationPercent = 20,
                BuildNumber = "6",
                Branch = "main",
                Status = "completed",
                Url = "https://GitHub.com/samsmithnz/devopsmetrics/6"
            };
            results.Add(item6);
            results.Add(item6);

            return results;
        }


    }
}
