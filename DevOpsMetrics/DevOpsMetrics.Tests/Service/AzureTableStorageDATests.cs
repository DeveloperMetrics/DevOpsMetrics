using DevOpsMetrics.Service.DataAccess.TableStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        //[TestMethod]
        //public async Task AzUpdatePRCommitsDAIntegrationTest()
        //{
        //    //Arrange
        //    string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
        //    string accountName = Configuration["AppSettings:AzureStorageAccountName"];
        //    string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
        //    string tableName = Configuration["AppSettings:DevOpsAzureDevOpsPRCommits"];
        //    string organization = "samsmithnz";
        //    string project = "SamLearnsAzure";
        //    string branch = "refs/heads/master";
        //    string buildName = "SamLearnsAzure.CI";
        //    string buildId = "3673"; //SamLearnsAzure.CI
        //    int numberOfDays = 30;
        //    int maxNumberOfItems = 20;

        //    //Act
        //    AzureTableStorageDA da = new AzureTableStorageDA();
        //    string pullRequestId = "";
        //    string repositoryId = "";
        //    int itemsAdded = await da.UpdateAzureDevOpsPullRequestCommits(patToken, accountName, accountAccessKey, tableName, organization, project, repositoryId, pullRequestId, numberOfDays, maxNumberOfItems);

        //    //Assert
        //    Assert.IsTrue(itemsAdded >= 0);
        //}



        [TestMethod]
        public void GHGetBuildsDAIntegrationTest()
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

        //[TestMethod]
        //public async Task GHUpdatePRCommitsDAIntegrationTest()
        //{
        //    //Arrange
        //    string clientId = Configuration["AppSettings:GitHubClientId"];
        //    string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
        //    string accountName = Configuration["AppSettings:AzureStorageAccountName"];
        //    string accountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];
        //    string tableName = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"];
        //    string owner = "samsmithnz";
        //    string repo = "DevOpsMetrics";
        //    string branch = "master";
        //    string workflowName = "DevOpsMetrics CI/CD";
        //    string workflowId = "1162561";
        //    int numberOfDays = 30;
        //    int maxNumberOfItems = 20;

        //    //Act
        //    AzureTableStorageDA da = new AzureTableStorageDA();

        //    string pullRequestNumber = "";
        //    int itemsAdded = await da.UpdateGitHubActionPullRequestCommits(clientId, clientSecret, accountName, accountAccessKey, tableName, owner, repo, branch, workflowName, workflowId, pullRequestNumber, numberOfDays, maxNumberOfItems);

        //    //Assert
        //    Assert.IsTrue(itemsAdded >= 0);
        //}

    }
}
