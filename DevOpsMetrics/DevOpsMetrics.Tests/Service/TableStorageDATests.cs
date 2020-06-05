using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class TableStorageDATests
    {

        [TestMethod]
        public async Task UpdateAzureDevOpsBuildsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsBuilds(It.IsAny<string>(), It.IsAny<TableStorageAuth>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string patToken = "";
            string organization = "";
            string project = "";
            string branch = "";
            string buildName = "";
            string buildId = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateAzureDevOpsBuilds(patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateGitHubActionRunsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateGitHubActionRuns(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageAuth>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string clientId = "";
            string clientSecret = "";
            string owner = "";
            string repo = "";
            string branch = "";
            string workflowName = "";
            string workflowId = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateGitHubActionRuns(clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateAzureDevOpsPullRequestsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsPullRequests(It.IsAny<string>(), It.IsAny<TableStorageAuth>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string patToken = "";
            string organization = "";
            string project = "";
            string repositoryId = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateAzureDevOpsPullRequests(patToken, organization, project, repositoryId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateGitHubActionPullRequestsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateGitHubActionPullRequests(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageAuth>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string clientId = "";
            string clientSecret = "";
            string owner = "";
            string repo = "";
            string branch = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateGitHubActionPullRequests(clientId, clientSecret, owner, repo, branch, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateAzureDevOpsPullRequestCommitsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsPullRequestCommits(It.IsAny<string>(), It.IsAny<TableStorageAuth>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),  It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string patToken = "";
            string organization = "";
            string project = "";
            string repositoryId = "";
            string pullRequestId = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateAzureDevOpsPullRequestCommits(patToken, organization, project, repositoryId, pullRequestId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateGitHubActionPullRequestCommitsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateGitHubActionPullRequestCommits(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageAuth>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(GetSampleUpdateData()));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string clientId = "";
            string clientSecret = "";
            string owner = "";
            string repo = "";
            string pull_number = "";

            //Act
            int result = await controller.UpdateGitHubActionPullRequestCommits(clientId, clientSecret, owner, repo, pull_number);

            //Assert
            Assert.AreEqual(7, result);
        }

        private int GetSampleUpdateData()
        {
            return 7;
        }

    }
}
