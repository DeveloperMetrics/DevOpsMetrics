using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class DeploymentFrequencyControllerTests
    {
        private TestServer _server;
        public HttpClient Client;
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<DeploymentFrequencyControllerTests>();
            Configuration = config.Build();

            //Setup the test server
            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseConfiguration(Configuration)
                .UseStartup<DevOpsMetrics.Service.Startup>());
            Client = _server.CreateClient();
            //Client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task AzDeploymentsSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.AreEqual(DevOpsPlatform.AzureDevOps, model.TargetDevOpsPlatform);
            Assert.AreEqual(buildName, model.DeploymentName);
            Assert.AreEqual(10f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("Elite", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10, model.BuildList.Count);
            Assert.AreEqual(70, model.BuildList[0].BuildDurationPercent);
            Assert.AreEqual("1", model.BuildList[0].BuildNumber);
            Assert.AreEqual("master", model.BuildList[0].Branch);
            Assert.AreEqual("completed", model.BuildList[0].Status);
            Assert.AreEqual("https://dev.azure.com/samsmithnz/samlearnsazure/1", model.BuildList[0].Url);
            Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
            Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task AzDeploymentsAPIControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

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
            }
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task AzDeploymentsCacheControllerIntegrationTest()
        {
            //https://devopsmetrics-prod-eu-service.azurewebsites.net//api/DeploymentFrequency/GetAzureDevOpsDeploymentFrequency?getSampleData=False&patToken=&organization=samsmithnz&project=SamLearnsAzure&branch=refs/heads/master&buildName=SamLearnsAzure.CI&buildId=3673&numberOfDays=30&maxNumberOfItems=20&useCache=true

            //Arrange
            bool getSampleData = false;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

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
            }
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task GHDeploymentsSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.AreEqual(DevOpsPlatform.GitHub, model.TargetDevOpsPlatform);
            Assert.AreEqual(workflowName, model.DeploymentName);
            Assert.AreEqual(10f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("Elite", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10, model.BuildList.Count);
            Assert.AreEqual(70, model.BuildList[0].BuildDurationPercent);
            Assert.AreEqual("1", model.BuildList[0].BuildNumber);
            Assert.AreEqual("master", model.BuildList[0].Branch);
            Assert.AreEqual("completed", model.BuildList[0].Status);
            Assert.AreEqual("https://GitHub.com/samsmithnz/devopsmetrics/1", model.BuildList[0].Url);
            Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
            Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task GHDeploymentsAPIControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

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
            }
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task GHDeploymentsCacheControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

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
            }
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task AzDeploymentsControllerAPILiveWithCacheIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

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
            }
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task GHDeploymentsControllerAPILiveWithCacheIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.AreEqual(DevOpsPlatform.GitHub, model.TargetDevOpsPlatform);
            Assert.AreEqual(workflowName, model.DeploymentName);
            Assert.AreEqual(10f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("Elite", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10, model.BuildList.Count);
            Assert.AreEqual(70, model.BuildList[0].BuildDurationPercent);
            Assert.AreEqual("1", model.BuildList[0].BuildNumber);
            Assert.AreEqual("master", model.BuildList[0].Branch);
            Assert.AreEqual("completed", model.BuildList[0].Status);
            Assert.AreEqual("https://GitHub.com/samsmithnz/devopsmetrics/1", model.BuildList[0].Url);
            Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
            Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task AzDeploymentsControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

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
            }
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task GHDeploymentsControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            DeploymentFrequencyController controller = new DeploymentFrequencyController(Configuration);

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

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
            }
        }

    }
}
