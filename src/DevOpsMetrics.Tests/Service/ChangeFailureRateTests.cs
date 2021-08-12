using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L1Test")]
    [TestClass]
    public class ChangeFailureRateTests : BaseConfiguration
    {
        [TestMethod]
        public void AzChangeFailureRateSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            ChangeFailureRateController controller = new(base.Configuration);

            //Act
            ChangeFailureRateModel model = controller.GetChangeFailureRate(getSampleData,
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

        //[TestCategory("L1Test")]
        //[TestMethod]
        //public void AzChangeFailureRateLiveControllerIntegrationTest()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    string organization = "samsmithnz";
        //    string project = "SamLearnsAzure";
        //    string branch = "refs/heads/master";
        //    string buildName = "SamLearnsAzure.CI";
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

        [TestCategory("L1Test")]
        [TestMethod]
        public void GHChangeFailureRateSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            ChangeFailureRateController controller = new(base.Configuration);

            //Act
            ChangeFailureRateModel model = controller.GetChangeFailureRate(getSampleData,
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

        //[TestCategory("L1Test")]
        //[TestMethod]
        //public void GHChangeFailureRateLiveControllerIntegrationTest()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    string owner = "samsmithnz";
        //    string repo = "SamsFeatureFlags";
        //    string branch = "main";
        //    string workflowName = "SamsFeatureFlags.CI/CD";
        //    DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
        //    int numberOfDays = 7;
        //    int maxNumberOfItems = 20;
        //    ChangeFailureRateController controller = new(base.Configuration);

        //    //Act
        //    ChangeFailureRateModel model = controller.GetChangeFailureRate(getSampleData,
        //       targetDevOpsPlatform, owner, repo, branch, workflowName, numberOfDays, maxNumberOfItems);

        //    //Assert
        //    Assert.IsTrue(model != null);
        //    Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
        //    Assert.IsTrue(model.DeploymentName == workflowName);
        //    Assert.IsTrue(model.ChangeFailureRateMetric >= 0f);
        //    Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
        //    Assert.AreEqual(numberOfDays, model.NumberOfDays);
        //    Assert.IsTrue(model.MaxNumberOfItems > 0);
        //    Assert.IsTrue(model.TotalItems > 0);

        //}


    }
}
