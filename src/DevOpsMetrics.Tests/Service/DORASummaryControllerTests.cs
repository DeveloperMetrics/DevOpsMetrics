using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service.Controllers;
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
            string organization = "DeveloperMetrics";
            string repository = "DevOpsMetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            string resourceGroup = "DevOpsMetrics";
            //string organization = "samsmithnz";
            //string repository = "AzurePipelinesToGitHubActionsConverter";
            //string branch = "main";
            //string workflowName = "CI/ CD";
            //string workflowId = "38158";
            //string resourceGroup = null;
            //string organization = "samsmithnz";
            //string repository = "AzurePipelinesToGitHubActionsConverterWeb";
            //string branch = "main";
            //string workflowName = "Pipelines to Actions website CI/CD";
            //string workflowId = "43084";
            //string resourceGroup = "PipelinesToActions";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            DORASummaryController controller = new(base.Configuration);

            //Act
            ProcessingResult model = await controller.UpdateDORASummaryItem(organization, "", repository,
                branch, workflowName, workflowId, resourceGroup, numberOfDays, maxNumberOfItems,
                null, true, true);

            DORASummaryItem doraSummaryItem = await controller.GetDORASummaryItem(organization, repository);

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

            //Act
            ProcessingResult model = await controller.UpdateDORASummaryItem(organization, project, repository,
                branch, buildName, buildId, resourceGroup, numberOfDays, maxNumberOfItems,
                null, true, false);

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

    }
}
