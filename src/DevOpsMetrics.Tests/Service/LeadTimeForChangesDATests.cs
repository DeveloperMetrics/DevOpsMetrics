using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class LeadTimeForChangesDATests
    {
        private IConfigurationRoot _configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<LeadTimeForChangesDATests>();
            _configuration = config.Build();
        }

        [TestMethod]
        public async Task AzLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = _configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repository = "SamLearnsAzure";
            string mainBranch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel model = await da.GetAzureDevOpsLeadTimesForChanges(getSampleData, patToken, tableStorageConfig,
                    organization, project, repository, mainBranch, buildName, 
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
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

             [TestMethod]
        public async Task GHLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = _configuration["AppSettings:GitHubClientId"];
            string clientSecret = _configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string mainBranch = "master";
            string workflowName = "DevOpsMetrics.CI";
            string workflowId = "1162561";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;

            //Act
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            LeadTimeForChangesModel model = await da.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, tableStorageConfig,
                    owner, repo, mainBranch, workflowName, workflowId,
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
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        //[TestMethod]
        //public async Task GHLeadTimeForChangesDAIntegrationTest2()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    string clientId = _configuration["AppSettings:GitHubClientId"];
        //    string clientSecret = _configuration["AppSettings:GitHubClientSecret"];
        //    tableStorageConfig tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
        //    string owner = "samsmithnz";
        //    string repo = "SamsFeatureFlags";
        //    string mainBranch = "main";
        //    string workflowName = "SamsFeatureFlags CI/CD";
        //    string workflowId = "108084";
        //    int numberOfDays = 7;
        //    int maxNumberOfItems = 20;
        //    bool useCache = false;

        //    //Act
        //    LeadTimeForChangesDA da = new LeadTimeForChangesDA();
        //    LeadTimeForChangesModel model = await da.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, tableStorageConfig,
        //            owner, repo, mainBranch, workflowName, workflowId,
        //            numberOfDays, maxNumberOfItems, useCache);

        //    //Assert
        //    Assert.IsTrue(model != null);
        //    Assert.AreEqual(repo, model.ProjectName);
        //    Assert.IsTrue(model.PullRequests.Count > 0);
        //    Assert.IsTrue(model.PullRequests.Count <= 20);
        //    Assert.AreEqual("123", model.PullRequests[0].PullRequestId);
        //    Assert.AreEqual("branch1", model.PullRequests[0].Branch);
        //    Assert.AreEqual(1, model.PullRequests[0].BuildCount);
        //    Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
        //    Assert.AreEqual("abc", model.PullRequests[0].Commits[0].commitId);
        //    Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
        //    Assert.AreEqual("name1", model.PullRequests[0].Commits[0].name);
        //    Assert.AreEqual(60, Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0));
        //    Assert.AreEqual(33f, model.PullRequests[0].DurationPercent);
        //    Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
        //    Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
        //    Assert.AreEqual(1f, model.AverageBuildHours);
        //    Assert.AreEqual(20.33f, model.AveragePullRequestHours);
        //    Assert.AreEqual(21.33f, model.LeadTimeForChangesMetric);
        //    Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
        //}

    }
}
