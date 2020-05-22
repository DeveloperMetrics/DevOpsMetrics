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
            TableStorageAuth tableStorageAuth = new TableStorageAuth
            {
                AccountName = Configuration["AppSettings:AzureStorageAccountName"],
                AccountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"],
                TableAzureDevOpsBuilds = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"],
                TableAzureDevOpsPRs = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"],
                TableAzureDevOpsPRCommits = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"],
                TableGitHubRuns = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"],
                TableGitHubPRs = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"],
                TableGitHubPRCommits = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"],
            };
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
            int itemsAdded = await da.UpdateAzureDevOpsBuilds(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsBuilds, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

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
            int itemsAdded = await da.UpdateAzureDevOpsPullRequests(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsBuilds,
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
                JArray list = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableAzureDevOpsPRCommits, da.CreateAzureDevOpsPRCommitPartitionKey(organization, project));
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
        public async Task AzUpdatePRCommitsDAIntegrationTest()
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
            JArray prList = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableAzureDevOpsPRs, da.CreateAzureDevOpsPRPartitionKey(organization, project));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());
                string pullRequestId = pullRequest.PullRequestId;
                itemsAdded += await da.UpdateAzureDevOpsPullRequestCommits(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsPRCommits, organization, project, repositoryId, pullRequestId, numberOfDays, maxNumberOfItems);
            }

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task AzUpdateSettingDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateAzureDevOpsSetting(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings,
                    organization, project, repositoryId, branch, buildName, buildId);

            //Assert
            Assert.IsTrue(result == true);
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
        public async Task GHUpdateBuildsDAIntegrationTest()
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
            int itemsAdded = await da.UpdateGitHubActionRuns(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubRuns,
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
        public async Task GHUpdatePRsDAIntegrationTest()
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
            int itemsAdded = await da.UpdateGitHubActionPullRequests(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubPRs, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetPRCommitsDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray prList = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableGitHubPRs, da.CreateGitHubPRPartitionKey(owner, repo));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                GitHubPR pullRequest = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());
                string pullRequestId = pullRequest.number;
                JArray list = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableGitHubPRCommits, da.CreateGitHubPRCommitPartitionKey(owner, repo));
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
        public async Task GHUpdatePRCommitsDAIntegrationTest()
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
            JArray prList = da.GetTableStorageItems(tableStorageAuth, tableStorageAuth.TableGitHubPRs, da.CreateGitHubPRPartitionKey(owner, repo));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                GitHubPR pullRequest = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());
                string pullRequestId = pullRequest.number;
                itemsAdded += await da.UpdateGitHubActionPullRequestCommits(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubPRCommits,
                        owner, repo, branch, workflowName, workflowId, pullRequestId, numberOfDays, maxNumberOfItems);
            }

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public async Task GHUpdateSettingDAIntegrationTest()
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

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateGitHubActionSetting(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId);

            //Assert
            Assert.IsTrue(result == true);
        }

    }
}
