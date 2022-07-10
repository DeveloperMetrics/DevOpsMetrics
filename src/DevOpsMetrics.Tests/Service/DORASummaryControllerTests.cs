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
            string organization = "samsmithnz";
            string repository = "AzurePipelinesToGitHubActionsConverter";
            DORASummaryController controller = new(base.Configuration);

            //Act
            DORASummaryItem model = controller.GetDORASummaryItems(organization, repository);

            //Assert
            Assert.IsNotNull(model);
            //Assert.AreEqual(DevOpsPlatform.AzureDevOps, model.TargetDevOpsPlatform);
            //Assert.AreEqual(buildName, model.DeploymentName);
            //Assert.AreEqual(10f, model.DeploymentsPerDayMetric);
            //Assert.AreEqual("Elite", model.DeploymentsPerDayMetricDescription);
            //Assert.AreEqual(10, model.BuildList.Count);
            //Assert.AreEqual(70, model.BuildList[0].BuildDurationPercent);
            //Assert.AreEqual("1", model.BuildList[0].BuildNumber);
            //Assert.AreEqual("main", model.BuildList[0].Branch);
            //Assert.AreEqual("completed", model.BuildList[0].Status);
            //Assert.AreEqual("https://dev.azure.com/samsmithnz/samlearnsazure/1", model.BuildList[0].Url);
            //Assert.IsTrue(model.BuildList[0].StartTime > DateTime.MinValue);
            //Assert.IsTrue(model.BuildList[0].EndTime > DateTime.MinValue);
        }

    }
}
