using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.GitHub
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class LeadTimeForChangesControllerTests
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
        public async Task GHLeadTimeControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string branch = "master";
            string workflowId = "1162561";
            LeadTimeForChangesController controller = new LeadTimeForChangesController();

            //Act
            List<LeadTimeForChangesModel> list = await controller.GetGitHubLeadTimeForChanges(getSampleData, clientId, clientSecret, owner, repo, branch, workflowId);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list[0].Branch != null);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task GHLeadTimeControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string branch = "master";
            string workflowId = "1162561";

            //Act
            string url = $"/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowId={workflowId}";
            TestResponse<List<LeadTimeForChangesModel>> httpResponse = new TestResponse<List<LeadTimeForChangesModel>>();
            List<LeadTimeForChangesModel> list = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            //Assert.IsTrue(list[0].branch != null);
        }

    }
}
