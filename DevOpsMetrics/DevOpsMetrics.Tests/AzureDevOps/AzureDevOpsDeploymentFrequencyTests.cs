using DevOpsMetrics.Service.DataAccess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.AzureDevOps
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class AzureDevOpsDeploymentFrequencyTests
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
        public async Task AzureDevOpsDeploymentFrequencyControllerIntegrationTest()
        {

            if (Client != null)
            {
                //Arrange
                string patToken = Configuration["AppSettings:PatToken"];
                string organization = "samsmithnz";
                string project = "SamLearnsAzure";
                string branch = "refs/heads/master";
                // string buildName = "SamLearnsAzure.CI";
                string buildId = "3673"; //SamLearnsAzure.CI
                int numberOfDays = 7;

                //Act
                HttpResponseMessage response = await Client.GetAsync($"/api/DeploymentFrequency/GetAzDeploymentFrequency?patToken={patToken}&organization={organization}&project={project}&AzureDevOpsbranch={branch}&buildId={buildId}&numberOfDays={numberOfDays}");
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();
                response.Dispose();

                //Assert
                Assert.IsTrue(result != null);
                Assert.IsTrue(float.Parse(result) > 0);
            }
        }

        [TestMethod]
        public async Task AzureDevOpsDeploymentFrequencyIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            // string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;

            //Act
            AzureDevOpsDeploymentFrequencyDA da = new AzureDevOpsDeploymentFrequencyDA();
            float deploymentFrequencyResult = await da.GetDeploymentFrequency(patToken, organization, project, branch, buildId, numberOfDays);

            //Assert
            Assert.IsTrue(deploymentFrequencyResult > 0);
        }

    }
}
