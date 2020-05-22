using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            config.AddUserSecrets<AzureTableStorageDATests>();
            Configuration = config.Build();
        }

        [TestMethod]
        public void AzGetSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);

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
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);

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
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673";
            int itemOrder = 1;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateAzureDevOpsSetting(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings,
                    organization, project, repositoryId, branch, buildName, buildId, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public async Task AzUpdatePartsUnlimitedSettingDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "PartsUnlimited";
            string repositoryId = "PartsUnlimited";
            string branch = "refs/heads/master";
            string buildName = "PartsUnlimited.CI";
            string buildId = "75";
            int itemOrder = 4;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateAzureDevOpsSetting(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsSettings,
                    organization, project, repositoryId, branch, buildName, buildId, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsSettingDAIntegrationTest()
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
            int itemOrder = 2;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateGitHubSetting(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsSettingDAIntegrationTest()
        {
            //Arrange
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "master";
            string workflowName = "SamsFeatureFlags.CI/CD";
            string workflowId = "108084";
            int itemOrder = 3;

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateGitHubSetting(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, itemOrder);

            //Assert
            Assert.IsTrue(result == true);
        }

    }
}
