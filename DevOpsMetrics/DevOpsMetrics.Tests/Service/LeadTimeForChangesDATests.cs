using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class LeadTimeForChangesDATests
    {
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<LeadTimeForChangesDATests>();
            Configuration = config.Build();
        }

        [TestMethod]
        public async Task AzLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string masterBranch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel model = await da.GetAzureDevOpsLeadTimesForChanges(getSampleData, patToken, tableStorageAuth,
                    organization, project, repositoryId, masterBranch, buildName, buildId,
                    numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(project, model.ProjectName);
            Assert.IsTrue(model.PullRequests.Count > 0);
            Assert.IsTrue(model.PullRequests.Count <= 20);
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
            Assert.AreEqual(1f, model.AverageBuildHours);
            Assert.AreEqual(12f, model.AveragePullRequestHours);
            Assert.AreEqual(13f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
        }

        [TestMethod]
        public async Task GHLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string masterBranch = "master";
            string workflowName = "DevOpsMetrics.CI";
            string workflowId = "1162561";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;

            //Act
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel model = await da.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, tableStorageAuth,
                    owner, repo, masterBranch, workflowName, workflowId,
                    numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(repo, model.ProjectName);
            Assert.IsTrue(model.PullRequests.Count > 0);
            Assert.IsTrue(model.PullRequests.Count <= 20);
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
            Assert.AreEqual(1f, model.AverageBuildHours);
            Assert.AreEqual(20.33f, model.AveragePullRequestHours);
            Assert.AreEqual(21.33f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
        }

    }
}
