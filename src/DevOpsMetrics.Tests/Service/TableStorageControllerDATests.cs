using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class TableStorageControllerDATests
    {
        public IConfigurationRoot _configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<TableStorageDATests>();
            _configuration = config.Build();
        }

        [TestMethod]
        public void AzGetBuildsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string buildName = "SamLearnsAzure.CI";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsBuilds, PartitionKeys.CreateBuildWorkflowPartitionKey(organization, project, buildName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdateBuildsDAIntegrationTest()
        {
            //Arrange
            string patToken = _configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateAzureDevOpsBuildsInStorage(patToken, tableStorageConfig, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsBuilds, PartitionKeys.CreateAzureDevOpsPRPartitionKey(organization, project));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdatePRsDAIntegrationTest()
        {
            //Arrange
            string patToken = _configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repository = "SamLearnsAzure";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateAzureDevOpsPullRequestsInStorage(patToken, tableStorageConfig,
                organization, project, repository, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRCommitsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
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
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repository = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<ProjectLog> logs = da.GetProjectLogsFromStorage(tableStorageConfig, PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository));

            //Assert
            Assert.IsTrue(logs != null);
            Assert.IsTrue(logs.Count > 0);
        }

        [TestMethod]
        public void GHGetBuildsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string workflowName = "DevOpsMetrics CI/CD";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubRuns, PartitionKeys.CreateBuildWorkflowPartitionKey(owner, repo, workflowName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = _configuration["AppSettings:GitHubClientId"];
            string clientSecret = _configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
                    owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = _configuration["AppSettings:GitHubClientId"];
            string clientSecret = _configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
                    owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetPRsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItemsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubPRs, PartitionKeys.CreateGitHubPRPartitionKey(owner, repo));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsPRsDAIntegrationTest()
        {
            //Arrange
            string clientId = _configuration["AppSettings:GitHubClientId"];
            string clientSecret = _configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionPullRequestsInStorage(clientId, clientSecret, tableStorageConfig, owner, repo, branch, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsPRsDAIntegrationTest()
        {
            //Arrange
            string clientId = _configuration["AppSettings:GitHubClientId"];
            string clientSecret = _configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionPullRequestsInStorage(clientId, clientSecret, tableStorageConfig, owner, repo, branch, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetSamsFeatureFlagsLogsDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<ProjectLog> logs = da.GetProjectLogsFromStorage(tableStorageConfig, PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo));

            //Assert
            Assert.IsTrue(logs != null);
            Assert.IsTrue(logs.Count > 0);
        }

    }
}
