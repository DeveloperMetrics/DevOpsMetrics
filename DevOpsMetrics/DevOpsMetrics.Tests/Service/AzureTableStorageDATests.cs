using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class AzureTableStorageDATests
    {
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<AzureTableStorageDATests>();
            Configuration = config.Build();
        }

        [TestMethod]
        public void AzGetBuildsDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string buildName = "SamLearnsAzure.CI";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableAzureDevOpsBuilds, da.CreateAzureDevOpsBuildPartitionKey(organization, project, buildName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdateBuildsDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateAzureDevOpsBuilds(patToken, tableStorageAuth, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRsDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableAzureDevOpsBuilds, da.CreateAzureDevOpsPRPartitionKey(organization, project));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdatePRsDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateAzureDevOpsPullRequests(patToken, tableStorageAuth,
                organization, project, repositoryId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRCommitsDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray prList = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableAzureDevOpsPRs, da.CreateAzureDevOpsPRPartitionKey(organization, project));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());
                string pullRequestId = pullRequest.PullRequestId;
                JArray list = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableAzureDevOpsPRCommits, da.CreateAzureDevOpsPRCommitPartitionKey(organization, project, pullRequestId));
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
        public void GHGetBuildsDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string workflowName = "DevOpsMetrics CI/CD";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableGitHubRuns, da.CreateGitHubRunPartitionKey(owner, repo, workflowName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionRuns(clientId, clientSecret, tableStorageAuth,
                    owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionRuns(clientId, clientSecret, tableStorageAuth,
                    owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetPRsDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableGitHubPRs, da.CreateGitHubPRPartitionKey(owner, repo));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsPRsDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionPullRequests(clientId, clientSecret, tableStorageAuth, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsPRsDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionPullRequests(clientId, clientSecret, tableStorageAuth, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

    }
}
