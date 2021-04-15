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
        private IConfigurationRoot _configuration;

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
        public void TestAzureSettingsExistTest()
        {
            //Arrange
            string AzureDevOpsPatToken = _configuration["Appsettings:AzureDevOpsPatToken"];
            string GitHubClientId = _configuration["Appsettings:GitHubClientId"];
            string GitHubClientSecret = _configuration["Appsettings:GitHubClientSecret"];
            string StorageAccountConnectionString = _configuration["AppSettings:AzureStorageAccountConfigurationString"];

            //Act

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(AzureDevOpsPatToken) == false);
            Assert.IsTrue(string.IsNullOrEmpty(GitHubClientId) == false);
            Assert.IsTrue(string.IsNullOrEmpty(GitHubClientSecret) == false);
            Assert.IsTrue(string.IsNullOrEmpty(StorageAccountConnectionString) == false);
        }

        [TestMethod]
        public void AzGetSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<AzureDevOpsSettings> results = da.GetAzureDevOpsSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings, null);

            //Assert
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void GHGetSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            List<GitHubSettings> results = da.GetGitHubSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings, null);

            //Assert
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public async Task AzUpdateSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            string patToken = _configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repository = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673";
            string resourceGroupName = "SamLearnsAzureProd";
            int itemOrder = 1;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateAzureDevOpsSettingInStorage(patToken, tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings,
                    organization, project, repository, branch, buildName, buildId, resourceGroupName, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsSettingDAIntegrationTest()
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
            string resourceGroupName = "DevOpsMetrics";
            int itemOrder = 2;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateGitHubSettingInStorage(clientId, clientSecret, tableStorageConfig, tableStorageConfig.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsSettingDAIntegrationTest()
        {
            //Arrange
            string clientId = _configuration["AppSettings:GitHubClientId"];
            string clientSecret = _configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableAuthorization(_configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            string resourceGroupName = "SamLearnsAzureFeatureFlags";
            int itemOrder = 3;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateGitHubSettingInStorage(clientId, clientSecret, tableStorageConfig, tableStorageConfig.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

    }
}
