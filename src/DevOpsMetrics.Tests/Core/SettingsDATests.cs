using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L1Test")]
    [TestClass]
    public class SettingsDATests : BaseConfiguration
    {
        [TestMethod]
        public void TestAzureSettingsExistTest()
        {
            //Arrange
            string AzureDevOpsPatToken = base.Configuration["Appsettings:AzureDevOpsPatToken"];
            string GitHubClientId = base.Configuration["Appsettings:GitHubClientId"];
            string GitHubClientSecret = base.Configuration["Appsettings:GitHubClientSecret"];
            string StorageAccountConnectionString = base.Configuration["AppSettings:AzureStorageAccountConfigurationString"];

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
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);

            //Act
            AzureTableStorageDA da = new();
            List<AzureDevOpsSettings> results = da.GetAzureDevOpsSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings, null);

            //Assert
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void GHGetSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);

            //Act
            AzureTableStorageDA da = new();
            List<GitHubSettings> results = da.GetGitHubSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings, null);

            //Assert
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public async Task AzUpdateSamLearnsAzureSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repository = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673";
            string resourceGroupName = "SamLearnsAzureProd";
            int itemOrder = 1;
            bool showSetting = true;

            //Act
            AzureTableStorageDA da = new();
            bool result = await da.UpdateAzureDevOpsSettingInStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings,
                    organization, project, repository, branch, buildName, buildId, resourceGroupName, itemOrder, showSetting);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GHUpdateDevOpsMetricsSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            string resourceGroupName = "DevOpsMetrics";
            int itemOrder = 2;
            bool showSetting = true;

            //Act
            AzureTableStorageDA da = new();
            bool result = await da.UpdateGitHubSettingInStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder, showSetting);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GHUpdateSamsFeatureFlagsSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags CI/CD";
            string workflowId = "108084";
            string resourceGroupName = "SamLearnsAzureFeatureFlags";
            int itemOrder = 3;
            bool showSetting = true;

            //Act
            AzureTableStorageDA da = new();
            bool result = await da.UpdateGitHubSettingInStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder, showSetting);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GHUpdateAzurePipelinesToGitHubActionsConverterWebSettingDAIntegrationTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "samsmithnz";
            string repo = "AzurePipelinesToGitHubActionsConverterWeb";
            string branch = "main";
            string workflowName = "Pipelines to Actions website CI/CD";
            string workflowId = "43084";
            string resourceGroupName = "PipelinesToActions";
            int itemOrder = 3;
            bool showSetting = true;

            //Act
            AzureTableStorageDA da = new();
            bool result = await da.UpdateGitHubSettingInStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder, showSetting);

            //Assert
            Assert.IsTrue(result);
        }

    }
}
