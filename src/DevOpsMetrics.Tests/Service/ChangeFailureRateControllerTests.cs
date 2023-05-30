using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class ChangeFailureRateControllerTests : BaseConfiguration
    {
        [TestMethod]
        public async Task AzChangeFailureRateSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            ChangeFailureRateController controller = new(base.Configuration);

            //Act
            ChangeFailureRateModel model = await controller.GetChangeFailureRate(getSampleData,
                targetDevOpsPlatform, organization, project, branch, buildName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        //[TestCategory("IntegrationTest")]
        //[TestMethod]
        //public void AzChangeFailureRateLiveControllerIntegrationTest()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    string organization = "samsmithnz";
        //string project = "AzDoDevOpsMetrics";
        //string branch = "refs/heads/main";
        //string buildName = "azure-pipelines.yml";
        //    DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
        //    int numberOfDays = 30;
        //    int maxNumberOfItems = 20;
        //    ChangeFailureRateController controller = new(base.Configuration);

        //    //Act
        //    ChangeFailureRateModel model = controller.GetChangeFailureRate(getSampleData,
        //        targetDevOpsPlatform, organization, project, branch, buildName, numberOfDays, maxNumberOfItems);

        //    //Assert
        //    Assert.IsTrue(model != null);
        //    Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
        //    Assert.IsTrue(model.DeploymentName == buildName);
        //    Assert.IsTrue(model.ChangeFailureRateMetric >= 0f);
        //    Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
        //    Assert.AreEqual(numberOfDays, model.NumberOfDays);
        //    Assert.IsTrue(model.MaxNumberOfItems > 0);
        //    Assert.IsTrue(model.TotalItems > 0);
        //}

        [TestMethod]
        public async Task GHChangeFailureRateSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            //string owner = "samsmithnz";
            //string repo = "SamsFeatureFlags";
            //string branch = "main";
            //string workflowName = "SamsFeatureFlags.CI/CD";
            string owner = "samsmithnz";
            string repo = "AzurePipelinesToGitHubActionsConverter";
            string branch = "main";
            string workflowName = "CI/ CD";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            ChangeFailureRateController controller = new(base.Configuration);

            //Act
            ChangeFailureRateModel model = await controller.GetChangeFailureRate(getSampleData,
               targetDevOpsPlatform, owner, repo, branch, workflowName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        [TestCategory("IntegrationTest")]
        [TestMethod]
        public async Task GHChangeFailureRateLiveControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string owner = "samsmithnz";
            string repo = "AzurePipelinesToGitHubActionsConverter";
            string branch = "main";
            string workflowName = "CI/ CD";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            ChangeFailureRateController controller = new(base.Configuration);

            //Act
            ChangeFailureRateModel model = await controller.GetChangeFailureRate(getSampleData,
               targetDevOpsPlatform, owner, repo, branch, workflowName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.IsTrue(model.DeploymentName == workflowName);
            Assert.IsTrue(model.ChangeFailureRateMetric >= -1f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);

        }


    }
}
