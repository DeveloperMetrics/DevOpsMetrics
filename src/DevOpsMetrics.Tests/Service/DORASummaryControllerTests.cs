using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class DORASummaryControllerTests : BaseConfiguration
    {
        [TestMethod]
        public async Task DORASummaryControllerGetIntegrationTest()
        {
            //Arrange
            string organization = "DeveloperMetrics";
            string repository = "DevOpsMetrics";
            DORASummaryController controller = new(base.Configuration);

            //Act
            DORASummaryItem model = await controller.GetDORASummaryItem(organization, repository);

            //Assert
            Assert.IsNotNull(model);

        }
        [TestMethod]
        public async Task DORASummaryControllerGetIntegrationTest2()
        {
            //Arrange
            string organization = "samsmithnz";
            string repository = "AzurePipelinesToGitHubActionsConverter";
            DORASummaryController controller = new(base.Configuration);

            //Act
            DORASummaryItem model = await controller.GetDORASummaryItem(organization, repository);

            //Assert
            Assert.IsNotNull(model);

        }

        [TestMethod]
        public async Task DORASummaryControllerGitHubUpdateIntegrationTest()
        {
            //Arrange
            string project = null;
            //string organization = "DeveloperMetrics";
            //string repository = "DevOpsMetrics";
            //string organization = "samsmithnz";
            //string repository = "AzurePipelinesToGitHubActionsConverter";
            string organization = "samsmithnz";
            string repository = "DotNetCensus";
            //string organization = "samsmithnz";
            //string project = "SamLearnsAzure";
            //string repository = "SamLearnsAzure";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            DORASummaryController controller = new(base.Configuration);
            ProcessingResult model = null;
            DORASummaryItem doraSummaryItem = null;

            //Act
            (AzureDevOpsSettings, GitHubSettings) setting = await GetSettingWithName(organization, project, repository);
            if (setting.Item1 != null)
            {
                AzureDevOpsSettings azureDevOpsSettings = setting.Item1;
                model = await controller.UpdateDORASummaryItem(azureDevOpsSettings.Organization,
                    azureDevOpsSettings.Project, azureDevOpsSettings.Repository,
                    azureDevOpsSettings.Branch,
                    azureDevOpsSettings.BuildName, azureDevOpsSettings.BuildId,
                    azureDevOpsSettings.ProductionResourceGroup,
                    numberOfDays, maxNumberOfItems,
                    null, true, false);
                doraSummaryItem = await controller.GetDORASummaryItem(azureDevOpsSettings.Organization, azureDevOpsSettings.Repository);
            }
            else
            {
                GitHubSettings gitHubSettings = setting.Item2;
                model = await controller.UpdateDORASummaryItem(gitHubSettings.Owner, "", gitHubSettings.Repo,
                     gitHubSettings.Branch, gitHubSettings.WorkflowName, gitHubSettings.WorkflowId, gitHubSettings.ProductionResourceGroup,
                     numberOfDays, maxNumberOfItems,
                     null, true, false);
                doraSummaryItem = await controller.GetDORASummaryItem(gitHubSettings.Owner, gitHubSettings.Repo);
            }


            //Assert
            Assert.IsNotNull(model);
            Assert.IsNotNull(doraSummaryItem);
            Assert.IsTrue(doraSummaryItem.DeploymentFrequency >= 0);
            Assert.IsTrue(doraSummaryItem.LeadTimeForChanges >= 0);
            Assert.IsTrue(doraSummaryItem.MeanTimeToRestore >= 0);
            Assert.IsTrue(doraSummaryItem.ChangeFailureRate >= -1); //Change failure rate is -1 when there is no data (since 0 means something different from this metric)
        }

        [TestMethod]
        public async Task DORASummaryControllerAzureDevOpsUpdateIntegrationTest()
        {
            //Arrange
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string repository = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            string buildId = "3673";
            string resourceGroup = null;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            DORASummaryController controller = new(base.Configuration);
            ProcessingResult model = null;

            //Act
            (AzureDevOpsSettings, GitHubSettings) setting = await GetSettingWithName(organization, project, repository);
            if (setting.Item1 != null)
            {
                AzureDevOpsSettings azureDevOpsSettings = setting.Item1;
                model = await controller.UpdateDORASummaryItem(azureDevOpsSettings.Organization,
                    azureDevOpsSettings.Project, azureDevOpsSettings.Repository,
                    azureDevOpsSettings.Branch,
                    azureDevOpsSettings.BuildName, azureDevOpsSettings.BuildId,
                    azureDevOpsSettings.ProductionResourceGroup,
                    numberOfDays, maxNumberOfItems,
                    null, true, false);
            }
            else
            {
                GitHubSettings gitHubSettings = setting.Item2;
                model = await controller.UpdateDORASummaryItem(gitHubSettings.Owner, "", gitHubSettings.Repo,
                     gitHubSettings.Branch, gitHubSettings.WorkflowName, gitHubSettings.WorkflowId, gitHubSettings.ProductionResourceGroup,
                     numberOfDays, maxNumberOfItems,
                     null, true, false);
            }

            //Assert
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public async Task DORASummaryControllerUpdateALLIntegrationTest()
        {
            //Arrange
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            int totalResults = 0;
            DORASummaryController controller = new(base.Configuration);
            SettingsController settingsController = new(base.Configuration, new AzureTableStorageDA());
            bool runExpensiveTest = false;

            //Act
            if (runExpensiveTest)
            {
                List<AzureDevOpsSettings> azSettings = await settingsController.GetAzureDevOpsSettings();
                List<GitHubSettings> ghSettings = await settingsController.GetGitHubSettings();

                foreach (AzureDevOpsSettings azSetting in azSettings)
                {
                    Debug.WriteLine($"Processing Azure DevOps organization {azSetting.Organization}, project {azSetting.Project}");
                    ProcessingResult ghResult = await controller.UpdateDORASummaryItem(
                        azSetting.Organization, azSetting.Project, azSetting.Repository,
                        azSetting.Branch, azSetting.BuildName, azSetting.BuildId,
                        azSetting.ProductionResourceGroup,
                        numberOfDays, maxNumberOfItems, null, true, false);
                    totalResults += ghResult.TotalResults;
                }

                foreach (GitHubSettings ghSetting in ghSettings)
                {
                    Debug.WriteLine($"Processing GitHub owner {ghSetting.Owner}, repo {ghSetting.Repo}");
                    ProcessingResult ghResult = await controller.UpdateDORASummaryItem(
                        ghSetting.Owner, "", ghSetting.Repo, ghSetting.Branch,
                        ghSetting.WorkflowName, ghSetting.WorkflowId,
                        ghSetting.ProductionResourceGroup,
                        numberOfDays, maxNumberOfItems, null, true, true);
                    totalResults += ghResult.TotalResults;
                }
            }

            //Assert
            if (runExpensiveTest)
            {
                Assert.IsTrue(totalResults > 0);
            }
            else
            {
                Assert.IsTrue(totalResults == 0);
            }
        }

        private async Task<(AzureDevOpsSettings, GitHubSettings)> GetSettingWithName(string owner, string project, string repo)
        {
            (AzureDevOpsSettings, GitHubSettings) results = (null, null);
            SettingsController settingsController = new(base.Configuration, new AzureTableStorageDA());
            List<AzureDevOpsSettings> azSettings = await settingsController.GetAzureDevOpsSettings();
            List<GitHubSettings> ghSettings = await settingsController.GetGitHubSettings();

            foreach (AzureDevOpsSettings azSetting in azSettings)
            {
                if (azSetting.Organization == owner && azSetting.Project == project && azSetting.Repository == repo)
                {
                    results = (azSetting, null);
                    break;
                }
            }
            foreach (GitHubSettings ghSetting in ghSettings)
            {
                if (ghSetting.Owner == owner && ghSetting.Repo == repo)
                {
                    results = (null, ghSetting);
                    break;
                }
            }
            return results;
        }


    }
}