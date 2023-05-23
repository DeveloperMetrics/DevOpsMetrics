using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class LeadTimeForChangesControllerTests : BaseConfiguration
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task AzLeadTimeControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            LeadTimeForChangesController controller = new(base.Configuration);

            //Act
            LeadTimeForChangesModel model = await controller.GetAzureDevOpsLeadTimeForChanges(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

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
        public async Task GHLeadTimeControllerIntegrationTest()
        {
            //https://devops-prod-eu-service.azurewebsites.net/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?
            //getSampleData=False&clientId=&clientSecret=&owner=samsmithnz&repo=DevOpsMetrics&
            //branch=master&workflowId=1162561&numberOfDays=30&maxNumberOfItems=20
            //Arrange
            bool getSampleData = true;
            string owner = "DeveloperMetrics";
            string repo = "devopsmetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;
            LeadTimeForChangesController controller = new(base.Configuration);

            //Act
            LeadTimeForChangesModel model = await controller.GetGitHubLeadTimeForChanges(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

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
            Assert.AreEqual(1f, model.AverageBuildHours);
            Assert.AreEqual(20.33f, model.AveragePullRequestHours);
            Assert.AreEqual(21.33f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("High", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }


        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task AzLeadTimeControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            LeadTimeForChangesController controller = new(base.Configuration);

            //Act
            LeadTimeForChangesModel model = await controller.GetAzureDevOpsLeadTimeForChanges(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

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
        public async Task GHLeadTimeControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string owner = "DeveloperMetrics";
            string repo = "devopsmetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = true;
            LeadTimeForChangesController controller = new(base.Configuration);

            //Act
            LeadTimeForChangesModel model = await controller.GetGitHubLeadTimeForChanges(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

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
            Assert.AreEqual(1f, model.AverageBuildHours);
            Assert.AreEqual(20.33f, model.AveragePullRequestHours);
            Assert.AreEqual(21.33f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("High", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }


        [TestMethod]
        public async Task AzLeadTimeControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;
            LeadTimeForChangesController controller = new(base.Configuration);

            //Act
            LeadTimeForChangesModel model = await controller.GetAzureDevOpsLeadTimeForChanges(getSampleData, organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

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
                    Assert.IsTrue(model.PullRequests[0].Commits.Count >= 0);
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
                Assert.IsTrue(model.AverageBuildHours >= 0);
                Assert.IsTrue(model.AveragePullRequestHours >= 0);
                Assert.IsTrue(model.LeadTimeForChangesMetric >= 0);
                Assert.IsTrue(string.IsNullOrEmpty(model.LeadTimeForChangesMetricDescription) == false);
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(model.MaxNumberOfItems >= 0);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }


        [TestMethod]
        public async Task GHLeadTimeControllerAPILiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string owner = "DeveloperMetrics";
            string repo = "devopsmetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 20;
            int maxNumberOfItems = 60;
            bool useCache = true;
            LeadTimeForChangesController controller = new(base.Configuration);

            //Act
            LeadTimeForChangesModel model = await controller.GetGitHubLeadTimeForChanges(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

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
                Assert.IsTrue(model.AverageBuildHours >= 0);
                Assert.IsTrue(model.AveragePullRequestHours >= 0);
                Assert.IsTrue(model.LeadTimeForChangesMetric >= 0);
                Assert.IsTrue(string.IsNullOrEmpty(model.LeadTimeForChangesMetricDescription) == false);
                Assert.AreEqual(numberOfDays, model.NumberOfDays);
                Assert.IsTrue(model.MaxNumberOfItems >= 0);
                Assert.IsTrue(model.TotalItems >= 0);
            }
        }


        //[TestMethod]
        //public async Task GHFeatureFlagsLeadTimeControllerAPILiveIntegrationTest()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    string owner = "samsmithnz";
        //    string repo = "SamsFeatureFlags";
        //    string branch = "main";
        //    string workflowName = "SamsFeatureFlags.CI/CD";
        //    string workflowId = "108084";
        //    int numberOfDays = 20;
        //    int maxNumberOfItems = 60;
        //    bool useCache = true;
        //    LeadTimeForChangesController controller = new(base.Configuration);

        //    //Act
        //    LeadTimeForChangesModel model = await controller.GetGitHubLeadTimeForChanges(getSampleData, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

        //    //Assert
        //    Assert.IsTrue(model != null);
        //    if (model.RateLimitHit == false)
        //    {
        //        Assert.AreEqual(repo, model.ProjectName);
        //        Assert.IsTrue(model.PullRequests.Count >= 0);
        //        if (model.PullRequests.Count > 0)
        //        {
        //            Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].PullRequestId) == false);
        //            Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Branch) == false);
        //            Assert.IsTrue(model.PullRequests[0].BuildCount >= 0);
        //            Assert.IsTrue(model.PullRequests[0].Commits.Count > 0);
        //            if (model.PullRequests[0].Commits.Count > 0)
        //            {
        //                Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Commits[0].commitId) == false);
        //                Assert.IsTrue(model.PullRequests[0].Commits[0].date >= DateTime.MinValue);
        //                Assert.IsTrue(string.IsNullOrEmpty(model.PullRequests[0].Commits[0].name) == false);
        //            }
        //            Assert.IsTrue(Math.Round(model.PullRequests[0].Duration.TotalMinutes, 0) >= 0);
        //            Assert.IsTrue(model.PullRequests[0].DurationPercent >= 0);
        //            Assert.IsTrue(model.PullRequests[0].StartDateTime >= DateTime.MinValue);
        //            Assert.IsTrue(model.PullRequests[0].EndDateTime >= DateTime.MinValue);
        //        }
        //        Assert.IsTrue(model.AverageBuildHours >= 0);
        //        Assert.IsTrue(model.AveragePullRequestHours >= 0);
        //        Assert.IsTrue(model.LeadTimeForChangesMetric >= 0);
        //        Assert.IsTrue(string.IsNullOrEmpty(model.LeadTimeForChangesMetricDescription) == false);
        //        Assert.AreEqual(numberOfDays, model.NumberOfDays);
        //        Assert.IsTrue(model.MaxNumberOfItems >= 0);
        //        Assert.IsTrue(model.TotalItems >= 0);
        //    }
        //}


    }
}
