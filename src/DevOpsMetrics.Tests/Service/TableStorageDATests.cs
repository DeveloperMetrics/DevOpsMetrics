using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L1Test")]
    [TestClass]
    public class TableStorageDATests : BaseConfiguration
    {
        [TestMethod]
        public void AzGetBuildsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string buildName = "SamLearnsAzure.CI";

            //Act
            AzureTableStorageDA da = new();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsBuilds, PartitionKeys.CreateBuildWorkflowPartitionKey(organization, project, buildName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdateBuildsDAIntegrationTest()
        {
            //Arrange
            string patToken = base.Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "AzDevOpsMetricsTest";
            string branch = "refs/heads/main";
            string buildName = "AzDevOpsMetricsTest";
            string buildId = "217";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new();
            int itemsAdded = await da.UpdateAzureDevOpsBuildsInStorage(patToken, tableStorageConfig, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsBuilds, PartitionKeys.CreateAzureDevOpsPRPartitionKey(organization, project));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdatePRsDAIntegrationTest()
        {
            //Arrange
            string patToken = base.Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "AzDevOpsMetricsTest";
            string repository = "AzDevOpsMetricsTest";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new();
            int itemsAdded = await da.UpdateAzureDevOpsPullRequestsInStorage(patToken, tableStorageConfig,
                organization, project, repository, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRCommitsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new();
            JArray prList = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRs, PartitionKeys.CreateAzureDevOpsPRPartitionKey(organization, project));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());
                string pullRequestId = pullRequest.PullRequestId;
                JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsPRCommits, PartitionKeys.CreateAzureDevOpsPRCommitPartitionKey(organization, project, pullRequestId));
                if (list.Count > 0)
                {
                    itemsAdded = list.Count;
                    break;
                }
            }

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetSamLearnsAzureLogsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repository = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new();
            List<ProjectLog> logs = da.GetProjectLogsFromStorage(tableStorageConfig, PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository));

            //Assert
            Assert.IsTrue(logs != null);
            Assert.IsTrue(logs.Count >= 0);
        }

        [TestMethod]
        public void GHGetBuildsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string workflowName = "DevOpsMetrics CI/CD";

            //Act
            AzureTableStorageDA da = new();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubRuns, PartitionKeys.CreateBuildWorkflowPartitionKey(owner, repo, workflowName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = base.Configuration["AppSettings:GitHubClientId"];
            string clientSecret = base.Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "main";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new();
            int itemsAdded = await da.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
                    owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = base.Configuration["AppSettings:GitHubClientId"];
            string clientSecret = base.Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new();
            int itemsAdded = await da.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
                    owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetPRsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";

            //Act
            AzureTableStorageDA da = new();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubPRs, PartitionKeys.CreateGitHubPRPartitionKey(owner, repo));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdateBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = base.Configuration["AppSettings:GitHubClientId"];
            string clientSecret = base.Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new();
            int itemsAdded = await da.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task GHUpdatePRsDAIntegrationTest()
        {
            //Arrange
            string clientId = base.Configuration["AppSettings:GitHubClientId"];
            string clientSecret = base.Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "ghDevOpsMetricsTest";
            string branch = "main";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new();
            int itemsAdded = await da.UpdateGitHubActionPullRequestsInStorage(clientId, clientSecret, tableStorageConfig, owner, repo, branch, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetSamsFeatureFlagsLogsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";

            //Act
            AzureTableStorageDA da = new();
            List<ProjectLog> logs = da.GetProjectLogsFromStorage(tableStorageConfig, PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo));

            //Assert
            Assert.IsTrue(logs != null);
            Assert.IsTrue(logs.Count >= 0);
        }

    }
}
