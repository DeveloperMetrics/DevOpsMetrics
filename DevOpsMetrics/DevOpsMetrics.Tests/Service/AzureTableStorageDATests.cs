using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
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
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string buildName = "SamLearnsAzure.CI";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(accountName, accountAccessKey, tableName, da.CreateAzureDevOpsBuildPartitionKey(organization, project, buildName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdateBuildsDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateAzureDevOpsBuilds(patToken, accountName, accountAccessKey, tableName, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRsDAIntegrationTest()
        {
            //Arrange
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(accountName, accountAccessKey, tableName, da.CreateAzureDevOpsPRPartitionKey(organization, project));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task AzUpdatePRsDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateAzureDevOpsPullRequests(patToken, accountName, accountAccessKey, tableName, organization, project, repositoryId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void AzGetPRCommitsDAIntegrationTest()
        {
            //Arrange
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string prTableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"];
            string prcommitTableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray prList = da.GetTableStorageItems(accountName, accountAccessKey, prTableName, da.CreateAzureDevOpsPRPartitionKey(organization, project));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());
                string pullRequestId = pullRequest.PullRequestId;
                JArray list = da.GetTableStorageItems(accountName, accountAccessKey, prcommitTableName, da.CreateAzureDevOpsPRCommitPartitionKey(organization, project));
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
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string prTableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"];
            string prcommitTableName = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray prList = da.GetTableStorageItems(accountName, accountAccessKey, prTableName, da.CreateAzureDevOpsPRPartitionKey(organization, project));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                AzureDevOpsPR pullRequest = JsonConvert.DeserializeObject<AzureDevOpsPR>(item.ToString());
                string pullRequestId = pullRequest.PullRequestId;
                itemsAdded += await da.UpdateAzureDevOpsPullRequestCommits(patToken, accountName, accountAccessKey, prcommitTableName, organization, project, repositoryId, pullRequestId, numberOfDays, maxNumberOfItems);
            }

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetBuildsDAIntegrationTest()
        {
            //Arrange
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"];
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string workflowName = "DevOpsMetrics CI/CD";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(accountName, accountAccessKey, tableName, da.CreateGitHubRunPartitionKey(owner, repo, workflowName));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdateBuildsDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"];
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionRuns(clientId, clientSecret, accountName, accountAccessKey, tableName, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetPRsDAIntegrationTest()
        {
            //Arrange
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"];
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray list = da.GetTableStorageItems(accountName, accountAccessKey, tableName, da.CreateGitHubPRPartitionKey(owner, repo));

            //Assert
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public async Task GHUpdatePRsDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string tableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"];
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            int itemsAdded = await da.UpdateGitHubActionPullRequests(clientId, clientSecret, accountName, accountAccessKey, tableName, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

        [TestMethod]
        public void GHGetPRCommitsDAIntegrationTest()
        {
            //Arrange
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string prTableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"];
            string prcommitTableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"];
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray prList = da.GetTableStorageItems(accountName, accountAccessKey, prTableName, da.CreateGitHubPRPartitionKey(owner, repo));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                GitHubPR pullRequest = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());
                string pullRequestId = pullRequest.number;
                JArray list = da.GetTableStorageItems(accountName, accountAccessKey, prcommitTableName, da.CreateGitHubPRCommitPartitionKey(owner, repo));
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
            string accountName = Configuration["AppSettings:AzureStorageAccountName"];
            string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
            string prTableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"];
            string prcommitTableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"];
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            JArray prList = da.GetTableStorageItems(accountName, accountAccessKey, prTableName, da.CreateGitHubPRPartitionKey(owner, repo));
            int itemsAdded = 0;
            foreach (JToken item in prList)
            {
                GitHubPR pullRequest = JsonConvert.DeserializeObject<GitHubPR>(item.ToString());
                string pullRequestId = pullRequest.number;
                itemsAdded += await da.UpdateGitHubActionPullRequestCommits(clientId, clientSecret, accountName, accountAccessKey, prcommitTableName, owner, repo, branch, workflowName, workflowId, pullRequestId, numberOfDays, maxNumberOfItems);
            }

            //Assert
            Assert.IsTrue(itemsAdded >= 0);
        }

    }
}
