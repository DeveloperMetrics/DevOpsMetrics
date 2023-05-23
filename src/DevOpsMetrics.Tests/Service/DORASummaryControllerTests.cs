using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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
    [TestCategory("L1Test")]
    [TestClass]
    public class DORASummaryControllerTests : BaseConfiguration
    {
        [TestMethod]
        public void DORASummaryControllerGetIntegrationTest()
        {
            //Arrange
            string organization = "DeveloperMetrics";
            string repository = "DevOpsMetrics";
            DORASummaryController controller = new(base.Configuration);

            //Act
            DORASummaryItem model = controller.GetDORASummaryItem(organization, repository);

            //Assert
            Assert.IsNotNull(model);

        }

        [TestMethod]
        public async Task DORASummaryControllerUpdateIntegrationTest()
        {
            //Arrange
            //string organization = "DeveloperMetrics";
            //string repository = "DevOpsMetrics";
            //string branch = "main";
            //string workflowName = "CI/CD";
            //string workflowId = "1162561";
            //string resourceGroup = "DevOpsMetrics";
            string organization = "samsmithnz";
            string repository = "AzurePipelinesToGitHubActionsConverter";
            string branch = "main";
            string workflowName = "CI/ CD";
            string workflowId = "38158";
            string resourceGroup = null;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            DORASummaryController controller = new(base.Configuration);

            //Act
            ProcessingResult model = await controller.UpdateDORASummaryItem(organization, repository,
                branch, workflowName, workflowId, resourceGroup, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsNotNull(model);
            //Assert.IsTrue(model.TotalResults > 0);
        }

        //[TestMethod]
        //public async Task DORASummaryControllerUpdateALLIntegrationTest()
        //{
        //    //Arrange
        //    int numberOfDays = 30;
        //    int maxNumberOfItems = 20;
        //    int totalResults = 0;
        //    DORASummaryController controller = new(base.Configuration);
        //    SettingsController settingsController = new(base.Configuration, new AzureTableStorageDA());

        //    //Act
        //    List<AzureDevOpsSettings> azSettings = settingsController.GetAzureDevOpsSettings();
        //    List<GitHubSettings> ghSettings = settingsController.GetGitHubSettings();

        //    foreach (AzureDevOpsSettings item in azSettings)
        //    {
        //        //    (int, string) buildsUpdated = (0, null);
        //        //    (int, string) prsUpdated = (0, null);
        //        //    try
        //        //    {
        //        //log.LogInformation($"Processing Azure DevOps organization {item.Organization}, project {item.Project}");
        //        //        buildsUpdated = await api.UpdateAzureDevOpsBuilds(item.Organization, item.Project, item.Repository, item.Branch, item.BuildName, item.BuildId, numberOfDays, maxNumberOfItems);
        //        //        prsUpdated = await api.UpdateAzureDevOpsPullRequests(item.Organization, item.Project, item.Repository, numberOfDays, maxNumberOfItems);
        //        //        log.LogInformation($"Processed Azure DevOps organization {item.Organization}, project {item.Project}. {buildsUpdated.Item1} builds and {prsUpdated.Item1} prs/commits updated");
        //        //        totalResults += buildsUpdated.Item1 + prsUpdated.Item1;
        //        //        await api.UpdateAzureDevOpsProjectLog(item.Organization, item.Project, item.Repository, buildsUpdated.Item1, prsUpdated.Item1, buildsUpdated.Item2, prsUpdated.Item2, null, null);
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        string error = $"Exception while processing Azure DevOps organization {item.Organization}, project {item.Project}. {buildsUpdated.Item1} builds and {prsUpdated.Item1} prs/commits updated";
        //        //        log.LogInformation(error);
        //        //        await api.UpdateAzureDevOpsProjectLog(item.Organization, item.Project, item.Repository, buildsUpdated.Item1, prsUpdated.Item1, buildsUpdated.Item2, prsUpdated.Item2, ex.Message, error);
        //        //    }
        //    }

        //    foreach (GitHubSettings ghSetting in ghSettings)
        //    {
        //        Debug.WriteLine($"Owner {ghSetting.Owner}, Repo {ghSetting.Repo}");
        //        ProcessingResult ghResult = await controller.UpdateDORASummaryItem(
        //            ghSetting.Owner, ghSetting.Repo, ghSetting.Branch,
        //            ghSetting.WorkflowName, ghSetting.WorkflowId,
        //            ghSetting.ProductionResourceGroup,
        //            numberOfDays, maxNumberOfItems);
        //        totalResults += ghResult.TotalResults;
        //        Assert.IsNotNull(ghResult);
        //    }

        //    //ProcessingResult model = await controller.UpdateDORASummaryItem(organization, repository,
        //    //    branch, workflowName, workflowId, resourceGroup, numberOfDays, maxNumberOfItems);

        //    //Assert
        //    Assert.IsTrue(totalResults > 0);
        //}

    }
}
