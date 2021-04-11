using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            mockDA.Setup(repo => repo.UpdateAzureDevOpsBuilds(It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
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
            mockDA.Setup(repo => repo.UpdateGitHubActionRuns(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
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
            mockDA.Setup(repo => repo.UpdateAzureDevOpsPullRequests(It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
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
            mockDA.Setup(repo => repo.UpdateGitHubActionPullRequests(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
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
            mockDA.Setup(repo => repo.UpdateAzureDevOpsPullRequestCommits(It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
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
            mockDA.Setup(repo => repo.UpdateGitHubActionPullRequestCommits(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(GetSampleUpdateData()));
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


        [TestMethod]
        public void GetAzureDevOpsSettingsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.GetAzureDevOpsSettings(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), null)).Returns(GetSampleAzureDevOpsSettingsData());
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);

            //Act
            List<AzureDevOpsSettings> result = controller.GetAzureDevOpsSettings();

            //Assert
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void GetGitHubSettingsTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.GetGitHubSettings(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>())).Returns(GetSampleGitHubSettingsData());
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);

            //Act
            List<GitHubSettings> result = controller.GetGitHubSettings();

            //Assert
            Assert.IsTrue(result != null);
        }


        [TestMethod]
        public async Task UpdateAzureDevOpsSettingTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsSetting(It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string patToken = "";
            string organization = "";
            string project = "";
            string repository = "";
            string branch = "";
            string buildName = "";
            string buildId = "";
            string resourceGroup = "";
            int itemOrder = 0;

            //Act
            bool result = await controller.UpdateAzureDevOpsSetting(patToken, organization, project, repository, branch, buildName, buildId, resourceGroup, itemOrder);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task UpdateGitHubSettingTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateGitHubSetting(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            string clientId = "";
            string clientSecret = "";
            string owner = "";
            string repo = "";
            string branch = "";
            string workflowName = "";
            string workflowId = "";
            string resourceGroup = "";
            int itemOrder = 0;

            //Act
            bool result = await controller.UpdateGitHubSetting(clientId, clientSecret, owner, repo, branch, workflowName, workflowId, resourceGroup, itemOrder);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task UpdateDevOpsMonitoringEventTest()
        {
            //Arrange
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new Mock<IAzureTableStorageDA>();
            mockDA.Setup(repo => repo.UpdateDevOpsMonitoringEvent(It.IsAny<TableStorageConfiguration>(), It.IsAny<MonitoringEvent>())).Returns(Task.FromResult(true));
            TableStorageController controller = new TableStorageController(mockConfig.Object, mockDA.Object);
            MonitoringEvent newEvent = new MonitoringEvent();

            //Act
            bool result = await controller.UpdateDevOpsMonitoringEvent(newEvent);

            //Assert
            Assert.AreEqual(true, result);
        }

        private static int GetSampleUpdateData()
        {
            return 7;
        }

        private static List<AzureDevOpsSettings> GetSampleAzureDevOpsSettingsData()
        {
            return new List<AzureDevOpsSettings>();
        }

        private static List<GitHubSettings> GetSampleGitHubSettingsData()
        {
            return new List<GitHubSettings>();
        }

    }
}
