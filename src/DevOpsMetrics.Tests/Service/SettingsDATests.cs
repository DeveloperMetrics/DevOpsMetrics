using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class SettingsDATests
    {
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<TableStorageDATests>();
            Configuration = config.Build();
        }

        [TestMethod]
        public void TestAzureSettingsExistTest()
        {
            //Arrange
            string AzureDevOpsPatToken = Configuration["Appsettings:AzureDevOpsPatToken"];
            string GitHubClientId = Configuration["Appsettings:GitHubClientId"];
            string GitHubClientSecret = Configuration["Appsettings:GitHubClientSecret"];
            string AccountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"];

            //Act

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(AzureDevOpsPatToken) == false);
            Assert.IsTrue(string.IsNullOrEmpty(GitHubClientId) == false);
            Assert.IsTrue(string.IsNullOrEmpty(GitHubClientSecret) == false);
            Assert.IsTrue(string.IsNullOrEmpty(AccountAccessKey) == false);
        }

        [TestMethod]
        public void AzGetSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageAuth = Common.GenerateTableAuthorization(Configuration);

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<AzureDevOpsSettings> results = da.GetAzureDevOpsSettings(tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings);

            //Assert
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void GHGetSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageAuth = Common.GenerateTableAuthorization(Configuration);

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<GitHubSettings> results = da.GetGitHubSettings(tableStorageAuth, tableStorageAuth.TableGitHubSettings);

            //Assert
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public async Task AzUpdateSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673";
            string resourceGroupName = "SamLearnsAzureProd";
            int itemOrder = 1;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateAzureDevOpsSetting(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings,
                    organization, project, repositoryId, branch, buildName, buildId, resourceGroupName, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

        //[TestMethod]
        //public async Task AzUpdatePartsUnlimitedSettingDAIntegrationTest()
        //{
        //    //Arrange
        //    string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
        //    TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
        //    string organization = "samsmithnz";
        //    string project = "PartsUnlimited";
        //    string repositoryId = "PartsUnlimited";
        //    string branch = "refs/heads/master";
        //    string buildName = "PartsUnlimited.CI";
        //    string buildId = "75";
        //    string resourceGroupName = "PartsUnlimited";
        //    int itemOrder = 4;

        //    //Act
        //    AzureTableStorageDA da = new AzureTableStorageDA();
        //    bool result = await da.UpdateAzureDevOpsSetting(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings,
        //            organization, project, repositoryId, branch, buildName, buildId, resourceGroupName, itemOrder);

        //    //Assert
        //    Assert.IsTrue(result == true);
        //}

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsSettingDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            string resourceGroupName = "DevOpsMetrics";
            int itemOrder = 2;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateGitHubSetting(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsSettingDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            string resourceGroupName = "SamLearnsAzureFeatureFlags";
            int itemOrder = 3;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateGitHubSetting(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

    }
}
