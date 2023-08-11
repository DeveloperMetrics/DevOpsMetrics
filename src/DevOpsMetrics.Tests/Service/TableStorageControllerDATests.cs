using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateAzureDevOpsBuildsInStorage(Arg.Any<string>(), Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.GetAzureDevOpsSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new List<AzureDevOpsSettings> { new AzureDevOpsSettings() }));
            BuildsController controller = new(_configuration, mockDA);
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateGitHubActionRunsInStorage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.GetGitHubSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new List<GitHubSettings> { new GitHubSettings() }));
            BuildsController controller = new(_configuration, mockDA);
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateAzureDevOpsPullRequestsInStorage(Arg.Any<string>(), Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.GetAzureDevOpsSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new List<AzureDevOpsSettings> { new AzureDevOpsSettings() }));
            PullRequestsController controller = new(_configuration, mockDA);
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateGitHubActionPullRequestsInStorage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.GetGitHubSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new List<GitHubSettings> { new GitHubSettings() }));
            PullRequestsController controller = new(_configuration, mockDA);
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateAzureDevOpsPullRequestCommitsInStorage(Arg.Any<string>(), Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.GetAzureDevOpsSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new List<AzureDevOpsSettings> { new AzureDevOpsSettings() }));
            PullRequestsController controller = new(_configuration, mockDA);
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateGitHubActionPullRequestCommitsInStorage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(GetSampleUpdateData()));
            mockDA.GetGitHubSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new List<GitHubSettings> { new GitHubSettings() }));
            PullRequestsController controller = new(_configuration, mockDA);
            string owner = "";
            string repo = "";
            string pull_number = "";

            //Act
            int result = await controller.UpdateGitHubActionPullRequestCommits(owner, repo, pull_number);

            //Assert
            Assert.AreEqual(7, result);
        }


        [TestMethod]
        public async Task GetAzureDevOpsSettingsTest()
        {
            //Arrange
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.GetAzureDevOpsSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(GetSampleAzureDevOpsSettingsData()));
            SettingsController controller = new(_configuration, mockDA);

            //Act
            List<AzureDevOpsSettings> result = await controller.GetAzureDevOpsSettings();

            //Assert
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public async Task GetGitHubSettingsTest()
        {
            //Arrange
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.GetGitHubSettingsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(GetSampleGitHubSettingsData()));
            SettingsController controller = new(_configuration, mockDA);

            //Act
            List<GitHubSettings> result = await controller.GetGitHubSettings();

            //Assert
            Assert.IsTrue(result != null);
        }


        [TestMethod]
        public async Task UpdateAzureDevOpsSettingTest()
        {
            //Arrange
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateAzureDevOpsSettingInStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA);
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateGitHubSettingInStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA);
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
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateDevOpsMonitoringEventInStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<MonitoringEvent>()).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA);
            MonitoringEvent newEvent = new();

            //Act
            bool result = await controller.UpdateDevOpsMonitoringEvent(newEvent);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task GetAzureDevOpsLogTest()
        {
            //Arrange
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.GetProjectLogsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>()).Returns(Task.FromResult(new List<ProjectLog> { new ProjectLog() }));
            SettingsController controller = new(_configuration, mockDA);
            string organization = "TestOrg";
            string project = "TestProject";
            string repository = "TestRepo";

            //Act
            List<ProjectLog> results = await controller.GetAzureDevOpsProjectLog(organization, project, repository);

            //Assert
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count >= 0);
        }

        [TestMethod]
        public async Task UpdateAzureDevOpsLogTest()
        {
            //Arrange
            //Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateProjectLogInStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<ProjectLog>()).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA);
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
        public async Task GetGitHubLogTest()
        {
            //Arrange
            //Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.GetProjectLogsFromStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<string>()).Returns(Task.FromResult(new List<ProjectLog> { new ProjectLog() }));
            SettingsController controller = new(_configuration, mockDA);
            string owner = "TestOrg";
            string repo = "TestRepo";

            //Act
            List<ProjectLog> results = await controller.GetGitHubProjectLog(owner, repo);

            //Assert
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count >= 0);
        }

        [TestMethod]
        public async Task UpdateGitHubLogTest()
        {
            //Arrange
            //Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            IAzureTableStorageDA mockDA = Substitute.For<IAzureTableStorageDA>();
            mockDA.UpdateProjectLogInStorage(Arg.Any<TableStorageConfiguration>(), Arg.Any<ProjectLog>()).Returns(Task.FromResult(true));
            SettingsController controller = new(_configuration, mockDA);
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
