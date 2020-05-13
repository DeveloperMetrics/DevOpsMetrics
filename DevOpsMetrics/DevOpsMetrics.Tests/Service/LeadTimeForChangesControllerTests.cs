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
        public async Task AzLeadTimeControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            LeadTimeForChangesController controller = new LeadTimeForChangesController();

            //Act
            LeadTimeForChangesModel model = await controller.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken, organization, project, repositoryId, branch, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(project, model.ProjectName);
            Assert.IsTrue(model.PullRequests.Count > 0);
            Assert.AreEqual("123", model.PullRequests[0].PullRequestId);
            Assert.AreEqual("branch1", model.PullRequests[0].Branch);
            Assert.AreEqual(1, model.PullRequests[0].BuildCount);
            Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
            Assert.AreEqual("abc", model.PullRequests[0].Commits[0].commitId);
            Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
            Assert.AreEqual("name1", model.PullRequests[0].Commits[0].name);
            Assert.AreEqual(60, Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0));
            Assert.AreEqual(33f, model.PullRequests[0].DurationPercent);
            Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
            Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
            Assert.AreEqual(12f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task GHLeadTimeControllerIntegrationTest()
        {
            //https://devopsmetrics-prod-eu-service.azurewebsites.net/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?
            //getSampleData=False&clientId=&clientSecret=&owner=samsmithnz&repo=DevOpsMetrics&
            //branch=master&workflowId=1162561&numberOfDays=30&maxNumberOfItems=20
            //Arrange
            bool getSampleData = true;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string branch = "master";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            LeadTimeForChangesController controller = new LeadTimeForChangesController();

            //Act
            LeadTimeForChangesModel model = await controller.GetGitHubLeadTimeForChanges(getSampleData, clientId, clientSecret, owner, repo, branch, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(repo, model.ProjectName);
            Assert.IsTrue(model.PullRequests.Count > 0);
            Assert.AreEqual("123", model.PullRequests[0].PullRequestId);
            Assert.AreEqual("branch1", model.PullRequests[0].Branch);
            Assert.AreEqual(1, model.PullRequests[0].BuildCount);
            Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
            Assert.AreEqual("abc", model.PullRequests[0].Commits[0].commitId);
            Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
            Assert.AreEqual("name1", model.PullRequests[0].Commits[0].name);
            Assert.AreEqual(60, Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0));
            Assert.AreEqual(33f, model.PullRequests[0].DurationPercent);
            Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
            Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
            Assert.AreEqual(20.33f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task AzLeadTimeControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;

            //Act            
            string url = $"/api/LeadTimeForChanges/GetAzureDevOpsLeadTimeForChanges?getSampleData={getSampleData}&patToken={patToken}&organization={organization}&project={project}&repositoryId={repositoryId}&branch={branch}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<LeadTimeForChangesModel> httpResponse = new TestResponse<LeadTimeForChangesModel>();
            LeadTimeForChangesModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(project, model.ProjectName);
            Assert.IsTrue(model.PullRequests.Count > 0);
            Assert.AreEqual("123", model.PullRequests[0].PullRequestId);
            Assert.AreEqual("branch1", model.PullRequests[0].Branch);
            Assert.AreEqual(1, model.PullRequests[0].BuildCount);
            Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
            Assert.AreEqual("abc", model.PullRequests[0].Commits[0].commitId);
            Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
            Assert.AreEqual("name1", model.PullRequests[0].Commits[0].name);
            Assert.AreEqual(60, Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0));
            Assert.AreEqual(33f, model.PullRequests[0].DurationPercent);
            Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
            Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
            Assert.AreEqual(12f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task GHLeadTimeControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string branch = "master";
            string workflowId = "1162561";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;

            //Act
            string url = $"/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<LeadTimeForChangesModel> httpResponse = new TestResponse<LeadTimeForChangesModel>();
            LeadTimeForChangesModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(repo, model.ProjectName);
            Assert.IsTrue(model.PullRequests.Count > 0);
            Assert.AreEqual("123", model.PullRequests[0].PullRequestId);
            Assert.AreEqual("branch1", model.PullRequests[0].Branch);
            Assert.AreEqual(1, model.PullRequests[0].BuildCount);
            Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
            Assert.AreEqual("abc", model.PullRequests[0].Commits[0].commitId);
            Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
            Assert.AreEqual("name1", model.PullRequests[0].Commits[0].name);
            Assert.AreEqual(60, Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0));
            Assert.AreEqual(33f, model.PullRequests[0].DurationPercent);
            Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
            Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
            Assert.AreEqual(20.33f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task AzLeadTimeControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act            
            string url = $"/api/LeadTimeForChanges/GetAzureDevOpsLeadTimeForChanges?getSampleData={getSampleData}&patToken={patToken}&organization={organization}&project={project}&repositoryId={repositoryId}&branch={branch}&buildId={buildId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<LeadTimeForChangesModel> httpResponse = new TestResponse<LeadTimeForChangesModel>();
            LeadTimeForChangesModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(project, model.ProjectName);
                Assert.IsTrue(model.PullRequests.Count >= 0);
                if (model.PullRequests.Count > 0)
                {
                    Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].PullRequestId) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Branch) == false);
                    Assert.IsTrue(model.PullRequests[0].BuildCount >= 0);
                    Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
                    if (model.PullRequests[0].Commits.Count > 0)
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Commits[0].commitId) == false);
                        Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
                        Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Commits[0].name) == false);
                    }
                    Assert.IsTrue(Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0) >= 0);
                    Assert.IsTrue(model.PullRequests[0].DurationPercent >= 0);
                    Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
                    Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
                }
                Assert.IsTrue(model.LeadTimeForChangesMetric >= 0);
                Assert.IsTrue(string.IsNullOrEmpty(model.LeadTimeForChangesMetricDescription) == false);
            }
        }

        [TestCategory("APITest")]
        [TestMethod]
        public async Task GHLeadTimeControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string branch = "master";
            string workflowId = "1162561";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;

            //Act
            string url = $"/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowId={workflowId}&numberOfDays={numberOfDays}&maxNumberOfItems={maxNumberOfItems}";
            TestResponse<LeadTimeForChangesModel> httpResponse = new TestResponse<LeadTimeForChangesModel>();
            LeadTimeForChangesModel model = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.IsTrue(model != null);
            if (model.RateLimitHit == false)
            {
                Assert.AreEqual(repo, model.ProjectName);
                Assert.IsTrue(model.PullRequests.Count >= 0);
                if (model.PullRequests.Count > 0)
                {
                    Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].PullRequestId) == false);
                    Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Branch) == false);
                    Assert.IsTrue(model.PullRequests[0].BuildCount >= 0);
                    Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
                    if (model.PullRequests[0].Commits.Count > 0)
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Commits[0].commitId) == false);
                        Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
                        Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Commits[0].name) == false);
                    }
                    Assert.IsTrue(Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0) >= 0);
                    Assert.IsTrue(model.PullRequests[0].DurationPercent >= 0);
                    Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
                    Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
                }
                Assert.IsTrue(model.LeadTimeForChangesMetric >= 0);
                Assert.IsTrue(string.IsNullOrEmpty(model.LeadTimeForChangesMetricDescription) == false);
            }
        }

    }
}
