﻿using DevOpsMetrics.Core;
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
    public class DeploymentFrequencyDA
    {
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
        {
            Utility<Build> utility = new Utility<Build>();
            if (getSampleData == false)
            {
                float deploymentsPerDay = 0;
                DeploymentFrequency deploymentFrequency = new DeploymentFrequency();
                List<Build> builds = new List<Build>();
                BuildsDA buildsDA = new BuildsDA();

                //Gets a list of builds
                List<AzureDevOpsBuild> azureDevOpsBuilds = await buildsDA.GetAzureDevOpsBuilds(patToken, organization, project, branch, buildId);
                List<KeyValuePair<DateTime, DateTime>> dateList = new List<KeyValuePair<DateTime, DateTime>>();

                //Translate the Azure DevOps build to a generic build object
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


                DeploymentFrequencyModel model = new DeploymentFrequencyModel
                {
                    IsAzureDevOps = true,
                    DeploymentName = buildName,
                    BuildList = utility.GetLastNItems(builds, maxNumberOfItems),
                    DeploymentsPerDayMetric = deploymentsPerDay,
                    DeploymentsPerDayMetricDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay),
                    NumberOfDays = numberOfDays
                };
                return model;
            }
            else
            {
                DeploymentFrequencyModel model = new DeploymentFrequencyModel
                {
                    IsAzureDevOps = true,
                    DeploymentName = buildName,
                    BuildList = utility.GetLastNItems(GetSampleAzureDevOpsBuilds(), maxNumberOfItems),
                    DeploymentsPerDayMetric = 10f,
                    DeploymentsPerDayMetricDescription = "Elite",
                    NumberOfDays = numberOfDays
                };
                return model;
            }
        }

        public async Task<int> RefreshAzureDevOpsDeploymentFrequency(string patToken, string accountName, string accessKey, string tableName, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems)
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
                    if (await tableDA.AddItem( newItem) == true)
                    {
                        itemsAdded++;
                    }
                }
            }

            return itemsAdded;
        }

        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            Utility<Build> utility = new Utility<Build>();
            if (getSampleData == false)
            {
                float deploymentsPerDay = 0;
                DeploymentFrequency deploymentFrequency = new DeploymentFrequency();
                List<Build> builds = new List<Build>();
                BuildsDA buildsDA = new BuildsDA();

                //Lists the workflows in a repository. 
                List<GitHubActionsRun> gitHubRuns = await buildsDA.GetGitHubActionRuns(getSampleData, clientId, clientSecret, owner, repo, branch, workflowId);
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

                DeploymentFrequencyModel model = new DeploymentFrequencyModel
                {
                    IsAzureDevOps = false,
                    DeploymentName = workflowName,
                    BuildList = utility.GetLastNItems(builds, maxNumberOfItems),
                    DeploymentsPerDayMetric = deploymentsPerDay,
                    DeploymentsPerDayMetricDescription = deploymentFrequency.GetDeploymentFrequencyRating(deploymentsPerDay),
                    NumberOfDays = numberOfDays
                };
                return model;
            }
            else
            {
                DeploymentFrequencyModel model = new DeploymentFrequencyModel
                {
                    IsAzureDevOps = false,
                    DeploymentName = workflowName,
                    BuildList = utility.GetLastNItems(GetSampleGitHubBuilds(), maxNumberOfItems),
                    DeploymentsPerDayMetric = 10f,
                    DeploymentsPerDayMetricDescription = "Elite",
                    NumberOfDays = numberOfDays
                };
                return model;
            }
        }

        public async Task<int> RefreshGitHubDeployments(string clientId, string clientSecret, string accountName, string accessKey, string tableName, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems)
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

        private List<Build> GetSampleAzureDevOpsBuilds()
        {
            List<Build> results = new List<Build>();
            Build item1 = new Build
            {
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                BuildDurationPercent = 70,
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
                BuildDurationPercent = 40,
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
                BuildDurationPercent = 20,
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
                BuildDurationPercent = 50,
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
                BuildDurationPercent = 60,
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
                BuildDurationPercent = 70,
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
                BuildDurationPercent = 70,
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
                BuildDurationPercent = 90,
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
                BuildDurationPercent = 40,
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
                BuildDurationPercent = 45,
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
                BuildDurationPercent = 50,
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
                BuildDurationPercent = 20,
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
