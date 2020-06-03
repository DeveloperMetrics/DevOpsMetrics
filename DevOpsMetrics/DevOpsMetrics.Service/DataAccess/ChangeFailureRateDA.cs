using DevOpsMetrics.Core;
using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class ChangeFailureRateDA
    {
        public async Task<ChangeFailureRateModel> GetChangeFailureRate(bool getSampleData, TableStorageAuth tableStorageAuth,
                DevOpsPlatform targetDevOpsPlatform, string organization_owner, string project_repo, string branch, string buildName_workflowName, string buildId_workflowId,
                int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ListUtility<ChangeFailureRateBuild> utility = new ListUtility<ChangeFailureRateBuild>();
            ChangeFailureRate changeFailureRate = new ChangeFailureRate();
            if (getSampleData == false)
            {
                //Gets a list of change failure rate builds
                AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
                Newtonsoft.Json.Linq.JArray list = daTableStorage.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableChangeFailureRate, daTableStorage.CreateBuildWorkflowPartitionKey(organization_owner, project_repo, buildName_workflowName));
                List<ChangeFailureRateBuild> initialBuilds = JsonConvert.DeserializeObject<List<ChangeFailureRateBuild>>(list.ToString());

                //Build the date list and then generate the change failure rate metric
                List<ChangeFailureRateBuild> builds = new List<ChangeFailureRateBuild>();
                List<KeyValuePair<DateTime, bool>> dateList = new List<KeyValuePair<DateTime, bool>>();
                float maxBuildDuration = 0f;
                foreach (ChangeFailureRateBuild item in initialBuilds)
                {
                    if (item.Branch == branch && item.StartTime > DateTime.Now.AddDays(-numberOfDays))
                    {
                        //Special branch for Azure DevOps to construct the Url to each build
                        if (targetDevOpsPlatform == DevOpsPlatform.AzureDevOps)
                        {
                            item.Url = $"https://dev.azure.com/{organization_owner}/{project_repo}/_build/results?buildId={item.Id}&view=results";
                        }
                        builds.Add(item);
                    }
                }

                //then build the calcuation
                foreach (ChangeFailureRateBuild item in builds)
                {
                    KeyValuePair<DateTime, bool> newItem = new KeyValuePair<DateTime, bool>(item.StartTime, item.DeploymentWasSuccessful);
                    dateList.Add(newItem);
                }
                //calculate the metric on all of the results
                float changeFailureRateMetric = changeFailureRate.ProcessChangeFailureRate(dateList, "", numberOfDays);

                //Filter the results to return the last n (maxNumberOfItems)
                List<ChangeFailureRateBuild> uiBuilds = utility.GetLastNItems(builds, maxNumberOfItems);
                foreach (ChangeFailureRateBuild item in uiBuilds)
                {
                    if (item.BuildDuration > maxBuildDuration)
                    {
                        maxBuildDuration = item.BuildDuration;
                    }
                }
                //We need to do some post processing and loop over the list a couple times to find the max build duration, construct a usable url, and calculate a build duration percentage
                foreach (ChangeFailureRateBuild item in uiBuilds)
                {
                    float interiumResult = ((item.BuildDuration / maxBuildDuration) * 100f);
                    item.BuildDurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                }

                ChangeFailureRateModel model = new ChangeFailureRateModel
                {
                    TargetDevOpsPlatform = targetDevOpsPlatform,
                    DeploymentName = buildName_workflowName,
                    ChangeFailureRateBuildList = uiBuilds,
                    ChangeFailureRateMetric = changeFailureRateMetric,
                    ChangeFailureRateMetricDescription = changeFailureRate.GetChangeFailureRateRating(changeFailureRateMetric),
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = uiBuilds.Count,
                    TotalItems = builds.Count
                };
                return model;
            }
            else
            {
                List<ChangeFailureRateBuild> sampleBuilds = utility.GetLastNItems(GetSampleBuilds(), maxNumberOfItems);
                ChangeFailureRateModel model = new ChangeFailureRateModel
                {
                    TargetDevOpsPlatform = targetDevOpsPlatform,
                    DeploymentName = buildName_workflowName,
                    ChangeFailureRateBuildList = sampleBuilds,
                    ChangeFailureRateMetric = 2f / 10f,
                    ChangeFailureRateMetricDescription = changeFailureRate.GetChangeFailureRateRating(2f / 10f),
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = sampleBuilds.Count,
                    TotalItems = sampleBuilds.Count
                };
                return model;
            }
        }

        public async Task<bool> UpdateChangeFailureRate(TableStorageAuth tableStorageAuth,
               string organization_owner, string project_repo, string buildName_workflowName,
               int percentComplete)
        {
            //Gets a list of change failure rate builds
            AzureTableStorageDA daTableStorage = new AzureTableStorageDA();
            string partitionKey = daTableStorage.CreateBuildWorkflowPartitionKey(organization_owner, project_repo, buildName_workflowName);
            Newtonsoft.Json.Linq.JArray list = daTableStorage.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableChangeFailureRate, partitionKey);
            List<ChangeFailureRateBuild> builds = JsonConvert.DeserializeObject<List<ChangeFailureRateBuild>>(list.ToString());

            int numerator = 0;
            int denominator = 0;
            switch (percentComplete)
            {
                case 0:
                    numerator = 0;
                    denominator = 1;
                    break;
                case 10:
                    numerator = 1;
                    denominator = 10;
                    break;
                case 25:
                    numerator = 1;
                    denominator = 4;
                    break;
                case 50:
                    numerator = 1;
                    denominator = 2;
                    break;
                case 75:
                    numerator = 3;
                    denominator = 4;
                    break;
                case 100:
                    numerator = 1;
                    denominator = 1;
                    break;
            }

            ListUtility<ChangeFailureRateBuild> listUtility = new ListUtility<ChangeFailureRateBuild>();
            List<ChangeFailureRateBuild> postiveBuilds = listUtility.GetLastNItems(builds.Where((x, numerator) => numerator % denominator == 0).ToList(), 20);
            List<ChangeFailureRateBuild> negativeBuilds = listUtility.GetLastNItems(builds.Where((x, numerator) => numerator % denominator != 0).ToList(), 20);
            TableStorageCommonDA tableChangeFailureRateDA = new TableStorageCommonDA(tableStorageAuth, tableStorageAuth.TableChangeFailureRate);
            foreach (ChangeFailureRateBuild item in postiveBuilds)
            {
                item.DeploymentWasSuccessful = true;
                await daTableStorage.UpdateChangeFailureRate(tableChangeFailureRateDA, item, partitionKey, true);
            }
            foreach (ChangeFailureRateBuild item in negativeBuilds)
            {
                item.DeploymentWasSuccessful = false;
                await daTableStorage.UpdateChangeFailureRate(tableChangeFailureRateDA, item, partitionKey, true);
            }

            return true;
        }

        private List<ChangeFailureRateBuild> GetSampleBuilds()
        {
            List<ChangeFailureRateBuild> results = new List<ChangeFailureRateBuild>();
            ChangeFailureRateBuild item1 = new ChangeFailureRateBuild
            {
                StartTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                BuildDurationPercent = 70,
                BuildNumber = "1",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/1",
                DeploymentWasSuccessful = true
            };
            results.Add(item1);
            results.Add(item1);
            results.Add(item1);
            results.Add(item1);
            ChangeFailureRateBuild item2 = new ChangeFailureRateBuild
            {
                StartTime = DateTime.Now.AddDays(-5).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-5).AddMinutes(0),
                BuildDurationPercent = 40,
                BuildNumber = "2",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/2",
                DeploymentWasSuccessful = true
            };
            results.Add(item2);
            results.Add(item2);
            results.Add(item2);
            results.Add(item2);
            ChangeFailureRateBuild item3 = new ChangeFailureRateBuild
            {
                StartTime = DateTime.Now.AddDays(-4).AddMinutes(-1),
                EndTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                BuildDurationPercent = 20,
                BuildNumber = "3",
                Branch = "master",
                Status = "failed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/3",
                DeploymentWasSuccessful = false
            };
            results.Add(item3);
            results.Add(item3);
            ChangeFailureRateBuild item4 = new ChangeFailureRateBuild
            {
                StartTime = DateTime.Now.AddDays(-3).AddMinutes(-4),
                EndTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                BuildDurationPercent = 50,
                BuildNumber = "4",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/4",
                DeploymentWasSuccessful = true
            };
            results.Add(item4);
            results.Add(item4);
            results.Add(item4);
            results.Add(item4);
            ChangeFailureRateBuild item5 = new ChangeFailureRateBuild
            {
                StartTime = DateTime.Now.AddDays(-2).AddMinutes(-7),
                EndTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                BuildDurationPercent = 60,
                BuildNumber = "5",
                Branch = "master",
                Status = "completed",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/5",
                DeploymentWasSuccessful = true
            };
            results.Add(item5);
            results.Add(item5);
            results.Add(item5);
            results.Add(item5);
            ChangeFailureRateBuild item6 = new ChangeFailureRateBuild
            {
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(-5),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                BuildDurationPercent = 70,
                BuildNumber = "6",
                Branch = "master",
                Status = "inProgress",
                Url = "https://dev.azure.com/samsmithnz/samlearnsazure/6",
                DeploymentWasSuccessful = false
            };
            results.Add(item6);
            results.Add(item6);
            results.Add(item6);

            return results;
        }

    }
}
