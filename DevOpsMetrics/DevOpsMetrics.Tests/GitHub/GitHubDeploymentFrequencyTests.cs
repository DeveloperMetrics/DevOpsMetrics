using DevOpsMetrics.Service.DataAccess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.GitHub
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class GitHubDeploymentFrequencyTests
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
        public async Task GitHubDeploymentFrequencyControllerIntegrationTest()
        {
            if (Client != null)
            {
                //Arrange
                string owner = "samsmithnz";
                string repo = "samsfeatureflags";
                string branch = "master";
                string workflowId = "108084";
                int numberOfDays = 7;

                //Act
                HttpResponseMessage response = await Client.GetAsync($"/api/DeploymentFrequency/GetGHDeploymentFrequency?owner={owner}&repo={repo}&GHbranch={branch}&workflowId={workflowId}&numberOfDays={numberOfDays}");
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();
                response.Dispose();

                //Assert
                Assert.IsTrue(result != null);
                Assert.IsTrue(float.Parse(result) > 0);
            }
        }

        [TestMethod]
        public async Task GitHubDeploymentFrequencyIntegrationTest()
        {
            //Arrange
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowId = "108084";
            int numberOfDays = 7;

            //Act
            GitHubDeploymentFrequencyDA da = new GitHubDeploymentFrequencyDA();
            float deploymentFrequencyResult = await da.GetDeploymentFrequency(owner, repo, branch, workflowId, numberOfDays);

            //Assert
            Assert.IsTrue(deploymentFrequencyResult > 0);
        }

    }
}
