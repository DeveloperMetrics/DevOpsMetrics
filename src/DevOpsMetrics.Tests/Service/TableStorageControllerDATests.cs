using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L0Test")]
    [TestClass]
    public class TableStorageControllerDATests
    {
        private IConfiguration _configuration;

        [TestInitialize]
        public void TestStart()
        {
            //Arrange
            Dictionary<string, string> inMemorySettings = new()
            {
                {
                    "AppSettings:KeyVaultURL",
                    "keyvaultURLTest"
                },
                {
                    PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken("", "", ""),
                    "patTokenSecret"
                },
                {
                    PartitionKeys.CreateGitHubSettingsPartitionKeyClientId("", ""),
                    "clientIdTest"
                },
                {
                    PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret("", ""),
                    "clientSecretTest"
                }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [TestMethod]
        public async Task UpdateAzureDevOpsBuildsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsBuildsInStorage(It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.Setup(repo => repo.GetAzureDevOpsSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<AzureDevOpsSettings> { new AzureDevOpsSettings() });
            BuildsController controller = new(_configuration, mockDA.Object);
            string organization = "";
            string project = "";
            string repository = "";
            string branch = "";
            string buildName = "";
            string buildId = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateAzureDevOpsBuilds(organization, project, repository, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateGitHubActionRunsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateGitHubActionRunsInStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.Setup(repo => repo.GetGitHubSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<GitHubSettings> { new GitHubSettings() });
            BuildsController controller = new(_configuration, mockDA.Object);
            string owner = "";
            string repo = "";
            string branch = "";
            string workflowName = "";
            string workflowId = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateGitHubActionRuns(owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateAzureDevOpsPullRequestsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsPullRequestsInStorage(It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.Setup(repo => repo.GetAzureDevOpsSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<AzureDevOpsSettings> { new AzureDevOpsSettings() });
            PullRequestsController controller = new(_configuration, mockDA.Object);
            string organization = "";
            string project = "";
            string repository = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateAzureDevOpsPullRequests(organization, project, repository, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateGitHubActionPullRequestsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateGitHubActionPullRequestsInStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.Setup(repo => repo.GetGitHubSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<GitHubSettings> { new GitHubSettings() });
            PullRequestsController controller = new(_configuration, mockDA.Object);
            string owner = "";
            string repo = "";
            string branch = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateGitHubActionPullRequests(owner, repo, branch, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateAzureDevOpsPullRequestCommitsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsPullRequestCommitsInStorage(It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.Setup(repo => repo.GetAzureDevOpsSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<AzureDevOpsSettings> { new AzureDevOpsSettings() });
            PullRequestsController controller = new(_configuration, mockDA.Object);
            string organization = "";
            string project = "";
            string repository = "";
            string pullRequestId = "";
            int numberOfDays = 0;
            int maxNumberOfItems = 0;

            //Act
            int result = await controller.UpdateAzureDevOpsPullRequestCommits(organization, project, repository, pullRequestId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public async Task UpdateGitHubActionPullRequestCommitsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateGitHubActionPullRequestCommitsInStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.Setup(repo => repo.GetGitHubSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<GitHubSettings> { new GitHubSettings() });
            PullRequestsController controller = new(_configuration, mockDA.Object);
            string owner = "";
            string repo = "";
            string pull_number = "";

            //Act
            int result = await controller.UpdateGitHubActionPullRequestCommits(owner, repo, pull_number);

            //Assert
            Assert.AreEqual(7, result);
        }


        [TestMethod]
        public void GetAzureDevOpsSettingsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.GetAzureDevOpsSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), null)).Returns(GetSampleAzureDevOpsSettingsData());
            SettingsController controller = new(_configuration, mockDA.Object);

            //Act
            List<AzureDevOpsSettings> result = controller.GetAzureDevOpsSettings();

            //Assert
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void GetGitHubSettingsTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.GetGitHubSettingsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>())).Returns(GetSampleGitHubSettingsData());
            SettingsController controller = new(_configuration, mockDA.Object);

            //Act
            List<GitHubSettings> result = controller.GetGitHubSettings();

            //Assert
            Assert.IsTrue(result != null);
        }


        [TestMethod]
        public async Task UpdateAzureDevOpsSettingTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateAzureDevOpsSettingInStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA.Object);
            string patToken = "";
            string organization = "";
            string project = "";
            string repository = "";
            string branch = "";
            string buildName = "";
            string buildId = "";
            string resourceGroup = "";
            int itemOrder = 0;
            bool showSetting = false;

            //Act
            bool result = await controller.UpdateAzureDevOpsSetting(patToken, organization, project, repository, branch, buildName, buildId, resourceGroup, itemOrder, showSetting);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task UpdateGitHubSettingTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateGitHubSettingInStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA.Object);
            string clientId = "";
            string clientSecret = "";
            string owner = "";
            string repo = "";
            string branch = "";
            string workflowName = "";
            string workflowId = "";
            string resourceGroup = "";
            int itemOrder = 0;
            bool showSetting = false;

            //Act
            bool result = await controller.UpdateGitHubSetting(clientId, clientSecret, owner, repo, branch, workflowName, workflowId, resourceGroup, itemOrder, showSetting);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task UpdateDevOpsMonitoringEventTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateDevOpsMonitoringEventInStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<MonitoringEvent>())).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA.Object);
            MonitoringEvent newEvent = new();

            //Act
            bool result = await controller.UpdateDevOpsMonitoringEvent(newEvent);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void GetAzureDevOpsLogTest()
        {
            //Arrange
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.GetProjectLogsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>())).Returns(new List<ProjectLog> { new ProjectLog() });
            SettingsController controller = new(_configuration, mockDA.Object);
            string organization = "TestOrg";
            string project = "TestProject";
            string repository = "TestRepo";

            //Act
            List<ProjectLog> results = controller.GetAzureDevOpsProjectLog(organization, project, repository);

            //Assert
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count >= 0);
        }

        [TestMethod]
        public async Task UpdateAzureDevOpsLogTest()
        {
            //Arrange
            //Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateProjectLogInStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<ProjectLog>())).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA.Object);
            string organization = "TestOrg";
            string project = "TestProject";
            string repository = "TestRepo";
            int buildsUpdated = 0;
            int prsUpdated = 0;
            string buildUrl = "urlBuildTest";
            string prUrl = "urlPRTest";
            string exceptionMessage = null;
            string exceptionStackTrace = null;

            //Act
            bool result = await controller.UpdateAzureDevOpsProjectLog(organization, project, repository,
                buildsUpdated, prsUpdated, buildUrl, prUrl, exceptionMessage, exceptionStackTrace);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void GetGitHubLogTest()
        {
            //Arrange
            //Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.GetProjectLogsFromStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<string>())).Returns(new List<ProjectLog> { new ProjectLog() });
            SettingsController controller = new(_configuration, mockDA.Object);
            string owner = "TestOrg";
            string repo = "TestRepo";

            //Act
            List<ProjectLog> results = controller.GetGitHubProjectLog(owner, repo);

            //Assert
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count >= 0);
        }

        [TestMethod]
        public async Task UpdateGitHubLogTest()
        {
            //Arrange
            //Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            Mock<IAzureTableStorageDA> mockDA = new();
            mockDA.Setup(repo => repo.UpdateProjectLogInStorage(It.IsAny<TableStorageConfiguration>(), It.IsAny<ProjectLog>())).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA.Object);
            string owner = "TestOrg";
            string repo = "TestRepo";
            int buildsUpdated = 0;
            int prsUpdated = 0;
            string buildUrl = "urlBuildTest";
            string prUrl = "urlPRTest";
            string exceptionMessage = null;
            string exceptionStackTrace = null;

            //Act
            bool result = await controller.UpdateGitHubProjectLog(owner, repo,
                buildsUpdated, prsUpdated, buildUrl, prUrl, exceptionMessage, exceptionStackTrace);

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
