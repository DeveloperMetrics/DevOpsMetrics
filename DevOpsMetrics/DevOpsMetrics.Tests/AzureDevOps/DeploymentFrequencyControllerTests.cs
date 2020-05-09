using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
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

        //[TestMethod]
        //public async Task AzDeploymentsControllerDirectIntegrationTest()
        //{
        //    //Arrange
        //    string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
        //    string organization = "samsmithnz";
        //    string project = "SamLearnsAzure";
        //    string branch = "refs/heads/master";
        //    string buildId = "3673"; //SamLearnsAzure.CI
        //    DeploymentFrequencyController controller = new DeploymentFrequencyController();

        //    //Act
        //    List<AzureDevOpsBuild> list = await controller.GetAzureDevOpsDeployments(patToken, organization, project, branch, buildId);

        //    //Assert
        //    Assert.IsTrue(list != null);
        //    Assert.IsTrue(list.Count > 0);
        //    Assert.IsTrue(list[0].status != null);
        //}

        //[TestCategory("APITest")]
        //[TestMethod]
        //public async Task AzDeploymentsControllerAPIIntegrationTest()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    string patToken = Configuration["AppSettings:PatToken"];
        //    string organization = "samsmithnz";
        //    string project = "SamLearnsAzure";
        //    string branch = "refs/heads/master";
        //    string buildId = "3673"; //SamLearnsAzure.CI

        //    //Act
        //    string url = $"/api/DeploymentFrequency/GetAzureDevOpsDeployments?getSampleData={getSampleData}&patToken ={patToken}&organization={organization}&project={project}&branch={branch}&buildId={buildId}";
        //    TestResponse<List<AzureDevOpsBuild>> httpResponse = new TestResponse<List<AzureDevOpsBuild>>();
        //    List<AzureDevOpsBuild> list = await httpResponse.GetResponse(Client, url);

        //    //Assert
        //    Assert.IsTrue(list != null);
        //    Assert.IsTrue(list.Count > 0);
        //    Assert.IsTrue(list[0].status != null);
        //}

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
            DeploymentFrequencyController controller = new DeploymentFrequencyController();

            //Act
            DeploymentFrequencyModel model = await controller.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays);

            //Assert
            Assert.IsTrue(model.DeploymentsPerDayMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription));
            Assert.AreNotEqual("Unknown", model.DeploymentsPerDayMetricDescription);
        }

    }
}
