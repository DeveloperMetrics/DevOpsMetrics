using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
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
            Configuration = config.Build();

            //Setup the test server
            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseConfiguration(Configuration)
                .UseStartup<DevOpsMetrics.Service.Startup>());
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task AzDeploymentFrequencyControllerIntegrationTest()
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
            DeploymentFrequencyController controller = new DeploymentFrequencyController();

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(true, model.IsAzureDevOps);
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
        public async Task GHDeploymentFrequencyControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowName = "samsfeatureflags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            DeploymentFrequencyController controller = new DeploymentFrequencyController();

            //Act
            DeploymentFrequencyModel model = await controller.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(false, model.IsAzureDevOps);
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
        public async Task AzDeploymentsControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;

            //Act
            string url = $"/api/DeploymentFrequency/GetAzureDevOpsDeploymentFrequency?getSampleData={getSampleData}&patToken={patToken}&organization={organization}&project={project}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<DeploymentFrequencyModel> httpResponse = new TestResponse<DeploymentFrequencyModel>();
            DeploymentFrequencyModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.AreEqual(true, model.IsAzureDevOps);
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

        [TestCategory("APITest")]
        [TestMethod]
        public async Task GHDeploymentsControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowName = "samsfeatureflags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;

            //Act
            string url = $"/api/DeploymentFrequency/GetGitHubDeploymentFrequency?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<DeploymentFrequencyModel> httpResponse = new TestResponse<DeploymentFrequencyModel>();
            DeploymentFrequencyModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.AreEqual(false, model.IsAzureDevOps);
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
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;

            //Act
            string url = $"/api/DeploymentFrequency/GetAzureDevOpsDeploymentFrequency?getSampleData={getSampleData}&patToken={patToken}&organization={organization}&project={project}&branch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<DeploymentFrequencyModel> httpResponse = new TestResponse<DeploymentFrequencyModel>();
            DeploymentFrequencyModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.AreEqual(true, model.IsAzureDevOps);
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

        [TestCategory("APITest")]
        [TestMethod]
        public async Task GHDeploymentsControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowName = "samsfeatureflags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;

            //Act
            string url = $"/api/DeploymentFrequency/GetGitHubDeploymentFrequency?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<DeploymentFrequencyModel> httpResponse = new TestResponse<DeploymentFrequencyModel>();
            DeploymentFrequencyModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.AreEqual(false, model.IsAzureDevOps);
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
