using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.AzureDevOps
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

        [TestMethod]
        public async Task AzDeploymentsControllerDirectIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            DeploymentFrequencyController controller = new DeploymentFrequencyController();

            //Act
            List<AzureDevOpsBuild> list = await controller.GetAzureDevOpsDeployments(patToken, organization, project, branch, buildId);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list[0].status != null);
        }

        [TestMethod]
        public async Task AzDeploymentsControllerAPIIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI

            //Act
            string url = $"/api/DeploymentFrequency/GetAzureDevOpsDeployments?patToken={patToken}&organization={organization}&project={project}&AzureDevOpsbranch={branch}&buildId={buildId}";
            TestResponse<List<AzureDevOpsBuild>> httpResponse = new TestResponse<List<AzureDevOpsBuild>>();
            List<AzureDevOpsBuild> list = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list[0].status != null);
        }

        [TestMethod]
        public async Task AzDeploymentFrequencyControllerIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            DeploymentFrequencyController controller = new DeploymentFrequencyController();

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(patToken, organization, project, branch, buildId, numberOfDays);

            //Assert
            Assert.IsTrue(model.deploymentsPerDay > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.deploymentsPerDayDescription));
            Assert.AreNotEqual("Unknown", model.deploymentsPerDayDescription);
        }

    }
}
