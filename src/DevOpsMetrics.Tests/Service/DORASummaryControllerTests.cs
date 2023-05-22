using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.Common;
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
            string organization = "DeveloperMetrics";
            string repository = "DevOpsMetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            string resourceGroup = "DevOpsMetrics";
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

    }
}
