using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class DeploymentFrequencyControllerTests : BaseConfiguration
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task AzDeploymentsSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.AreEqual(DevOpsPlatform.AzureDevOps, model.TargetDevOpsPlatform);
            Assert.AreEqual(buildName, model.DeploymentName);
            Assert.AreEqual(10f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("High", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10, model.BuildList.Count);
            Assert.AreEqual(70, model.BuildList[0].BuildDurationPercent);
            Assert.AreEqual("1", model.BuildList[0].BuildNumber);
            Assert.AreEqual("main", model.BuildList[0].Branch);
            Assert.AreEqual("completed", model.BuildList[0].Status);
            Assert.AreEqual("https://dev.azure.com/samsmithnz/samlearnsazure/1", model.BuildList[0].Url);
            Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
            Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
        }


        [TestMethod]
        public async Task AzDeploymentsAPIControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(DevOpsPlatform.AzureDevOps, model.TargetDevOpsPlatform);
                Assert.AreEqual(buildName, model.DeploymentName);
                Assert.IsTrue(model.DeploymentsPerDayMetric >= 0f);
                Assert.IsTrue(string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription) == false);
                Assert.IsTrue(model.BuildList.Count >= 0);
                if (model.BuildList.Count > 0)
                {
                    Assert.IsTrue(model.BuildList[0].BuildDurationPercent >= 0f);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].BuildNumber) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Branch) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Status) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Url) == false);
                    Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
                    Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
                }
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(maxNumberOfItems >= model.MaxNumberOfItems);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }


        [TestMethod]
        public async Task AzDeploymentsCacheControllerIntegrationTest()
        {
            //https://devops-prod-eu-service.azurewebsites.net//api/DeploymentFrequency/GetAzureDevOpsDeploymentFrequency?getSampleData=False&organization=samsmithnz&project=AzDoDevOpsMetrics&repository=SamLearnsAzure&branch=refs/heads/main&buildName=azure-pipelines.yml&buildId=3673&numberOfDays=30&maxNumberOfItems=20&useCache=true

            //Arrange
            bool getSampleData = false;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(DevOpsPlatform.AzureDevOps, model.TargetDevOpsPlatform);
                Assert.AreEqual(buildName, model.DeploymentName);
                Assert.IsTrue(model.DeploymentsPerDayMetric >= 0f);
                Assert.IsTrue(string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription) == false);
                Assert.IsTrue(model.BuildList.Count >= 0);
                if (model.BuildList.Count > 0)
                {
                    Assert.IsTrue(model.BuildList[0].BuildDurationPercent >= 0f);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].BuildNumber) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Branch) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Status) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Url) == false);
                    Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
                    Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
                }
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(maxNumberOfItems >= model.MaxNumberOfItems);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }


        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task GHDeploymentsSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.AreEqual(DevOpsPlatform.GitHub, model.TargetDevOpsPlatform);
            Assert.AreEqual(workflowName, model.DeploymentName);
            Assert.AreEqual(10f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("High", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10, model.BuildList.Count);
            Assert.AreEqual(70, model.BuildList[0].BuildDurationPercent);
            Assert.AreEqual("1", model.BuildList[0].BuildNumber);
            Assert.AreEqual("main", model.BuildList[0].Branch);
            Assert.AreEqual("completed", model.BuildList[0].Status);
            Assert.AreEqual("https://GitHub.com/samsmithnz/devopsmetrics/1", model.BuildList[0].Url);
            Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
            Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }


        [TestMethod]
        public async Task GHDeploymentsAPIControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(DevOpsPlatform.GitHub, model.TargetDevOpsPlatform);
                Assert.AreEqual(workflowName, model.DeploymentName);
                Assert.IsTrue(model.DeploymentsPerDayMetric >= 0f);
                Assert.IsTrue(string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription) == false);
                Assert.IsTrue(model.BuildList.Count >= 0);
                if (model.BuildList.Count > 0)
                {
                    Assert.IsTrue(model.BuildList[0].BuildDurationPercent >= 0f);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].BuildNumber) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Branch) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Status) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Url) == false);
                    Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
                    Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
                }
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(maxNumberOfItems >= model.MaxNumberOfItems);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }


        [TestMethod]
        public async Task GHDeploymentsCacheControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(DevOpsPlatform.GitHub, model.TargetDevOpsPlatform);
                Assert.AreEqual(workflowName, model.DeploymentName);
                Assert.IsTrue(model.DeploymentsPerDayMetric >= 0f);
                Assert.IsTrue(string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription) == false);
                Assert.IsTrue(model.BuildList.Count >= 0);
                if (model.BuildList.Count > 0)
                {
                    Assert.IsTrue(model.BuildList[0].BuildDurationPercent >= 0f);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].BuildNumber) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Branch) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Status) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Url) == false);
                    Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
                    Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
                }
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(maxNumberOfItems >= model.MaxNumberOfItems);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }



        [TestMethod]
        public async Task AzDeploymentsControllerAPILiveWithCacheIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(DevOpsPlatform.AzureDevOps, model.TargetDevOpsPlatform);
                Assert.AreEqual(buildName, model.DeploymentName);
                Assert.IsTrue(model.DeploymentsPerDayMetric >= 0f);
                Assert.IsTrue(string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription) == false);
                Assert.IsTrue(model.BuildList.Count >= 0);
                if (model.BuildList.Count > 0)
                {
                    Assert.IsTrue(model.BuildList[0].BuildDurationPercent >= 0f);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].BuildNumber) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Branch) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Status) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Url) == false);
                    Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
                    Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
                }
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(maxNumberOfItems >= model.MaxNumberOfItems);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }



        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task GHDeploymentsControllerAPILiveWithCacheIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.AreEqual(DevOpsPlatform.GitHub, model.TargetDevOpsPlatform);
            Assert.AreEqual(workflowName, model.DeploymentName);
            Assert.AreEqual(10f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("High", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10, model.BuildList.Count);
            Assert.AreEqual(70, model.BuildList[0].BuildDurationPercent);
            Assert.AreEqual("1", model.BuildList[0].BuildNumber);
            Assert.AreEqual("main", model.BuildList[0].Branch);
            Assert.AreEqual("completed", model.BuildList[0].Status);
            Assert.AreEqual("https://GitHub.com/samsmithnz/devopsmetrics/1", model.BuildList[0].Url);
            Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
            Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }



        [TestMethod]
        public async Task AzDeploymentsControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(DevOpsPlatform.AzureDevOps, model.TargetDevOpsPlatform);
                Assert.AreEqual(buildName, model.DeploymentName);
                Assert.IsTrue(model.DeploymentsPerDayMetric >= 0f);
                Assert.IsTrue(string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription) == false);
                Assert.IsTrue(model.BuildList.Count >= 0);
                if (model.BuildList.Count > 0)
                {
                    Assert.IsTrue(model.BuildList[0].BuildDurationPercent >= 0f);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].BuildNumber) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Branch) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Status) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Url) == false);
                    Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
                    Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
                }
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(maxNumberOfItems >= model.MaxNumberOfItems);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }



        [TestMethod]
        public async Task GHDeploymentsControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new(base.Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(DevOpsPlatform.GitHub, model.TargetDevOpsPlatform);
                Assert.AreEqual(workflowName, model.DeploymentName);
                Assert.IsTrue(model.DeploymentsPerDayMetric >= 0f);
                Assert.IsTrue(string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription) == false);
                Assert.IsTrue(model.BuildList.Count >= 0);
                if (model.BuildList.Count > 0)
                {
                    Assert.IsTrue(model.BuildList[0].BuildDurationPercent >= 0f);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].BuildNumber) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Branch) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Status) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.BuildList[0].Url) == false);
                    Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
                    Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
                }
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(maxNumberOfItems >= model.MaxNumberOfItems);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }

    }
}
