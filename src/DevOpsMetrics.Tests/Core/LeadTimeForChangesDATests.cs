using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class LeadTimeForChangesDATests : BaseConfiguration
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task AzLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = base.Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            LeadTimeForChangesModel model = await LeadTimeForChangesDA.GetAzureDevOpsLeadTimesForChanges(getSampleData, patToken, tableStorageConfig,
                    organization, project, repository, branch, buildName,
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
            Assert.AreEqual("High", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task GHLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = base.Configuration["AppSettings:GitHubClientId"];
            string clientSecret = base.Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "DeveloperMetrics";
            string repo = "devopsmetrics";
            string mainBranch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;

            //Act
            LeadTimeForChangesModel model = await LeadTimeForChangesDA.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, tableStorageConfig,
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
            Assert.AreEqual("High", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        //[TestMethod]
        //public async Task GHLeadTimeForChangesDAIntegrationTest2()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    string clientId = base.Configuration["AppSettings:GitHubClientId"];
        //    string clientSecret = base.Configuration["AppSettings:GitHubClientSecret"];
        //    tableStorageConfig tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
        //    string owner = "samsmithnz";
        //    string repo = "SamsFeatureFlags";
        //    string mainBranch = "main";
        //    string workflowName = "SamsFeatureFlags CI/CD";
        //    string workflowId = "108084";
        //    int numberOfDays = 7;
        //    int maxNumberOfItems = 20;
        //    bool useCache = false;

        //    //Act
        //    LeadTimeForChangesDA da = new();
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
