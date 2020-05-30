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
    public class ChangeFailureRateControllerTests
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
            config.AddUserSecrets<ChangeFailureRateControllerTests>();
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
        public async Task AzChangeFailureRateSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            bool isAzureDevOps = true;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            ChangeFailureRateController controller = new ChangeFailureRateController(Configuration);

            //Act
            ChangeFailureRateModel model = await controller.GetChangeFailureRate(getSampleData, 
                isAzureDevOps, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.IsAzureDevOps == isAzureDevOps);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task AzChangeFailureRateLiveControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            bool isAzureDevOps = true;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;
            ChangeFailureRateController controller = new ChangeFailureRateController(Configuration);

            //Act
            ChangeFailureRateModel model = await controller.GetChangeFailureRate(getSampleData,
                isAzureDevOps, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.IsAzureDevOps == isAzureDevOps);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric == 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreEqual("Elite", model.ChangeFailureRateMetricDescription);
        }
    
        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task GHChangeFailureRateSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowName = "samsfeatureflags CI/CD";
            string workflowId = "108084";
            bool isAzureDevOps = false;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            ChangeFailureRateController controller = new ChangeFailureRateController(Configuration);

            //Act
            ChangeFailureRateModel model = await controller.GetChangeFailureRate(getSampleData,
               isAzureDevOps, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.IsAzureDevOps == isAzureDevOps);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task GHChangeFailureRateLiveControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowName = "samsfeatureflags CI/CD";
            string workflowId = "108084";
            bool isAzureDevOps = false;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            ChangeFailureRateController controller = new ChangeFailureRateController(Configuration);

            //Act
            ChangeFailureRateModel model = await controller.GetChangeFailureRate(getSampleData,
               isAzureDevOps, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.IsAzureDevOps == isAzureDevOps);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric == 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreEqual("Elite", model.ChangeFailureRateMetricDescription);
        }

    }
}
