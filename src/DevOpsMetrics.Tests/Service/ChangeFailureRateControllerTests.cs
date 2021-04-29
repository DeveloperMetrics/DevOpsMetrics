using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L1Test")]
    [TestClass]
    public class ChangeFailureRateControllerTests : BaseConfiguration
    {
        [TestCategory("ControllerTest")]
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
            ChangeFailureRateController controller = new ChangeFailureRateController(base.Configuration);

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

        [TestCategory("ControllerTest")]
        [TestMethod]
        public void AzChangeFailureRateLiveControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            ChangeFailureRateController controller = new ChangeFailureRateController(base.Configuration);

            //Act
            ChangeFailureRateModel model = controller.GetChangeFailureRate(getSampleData,
                targetDevOpsPlatform, organization, project, branch, buildName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.IsTrue(model.DeploymentName == buildName);
            Assert.IsTrue(model.ChangeFailureRateMetric >= 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        [TestCategory("ControllerTest")]
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
            ChangeFailureRateController controller = new ChangeFailureRateController(base.Configuration);

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

        [TestCategory("ControllerTest")]
        [TestMethod]
        public void GHChangeFailureRateLiveControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string owner = "samsmithnz";
            string repo = "SamsFeatureFlags";
            string branch = "main";
            string workflowName = "SamsFeatureFlags.CI/CD";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            ChangeFailureRateController controller = new ChangeFailureRateController(base.Configuration);

            //Act
            ChangeFailureRateModel model = controller.GetChangeFailureRate(getSampleData,
               targetDevOpsPlatform, owner, repo, branch, workflowName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.IsTrue(model.DeploymentName == workflowName);
            Assert.IsTrue(model.ChangeFailureRateMetric >= 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);

        }

        
        [TestMethod]
        public void ChangeFailureRateControllerEliteBadgeTest()
        {
            //Arrange
            ChangeFailureRateModel model = new ChangeFailureRateModel
            {
                ChangeFailureRateMetricDescription = "Elite",
                ChangeFailureRateMetric = 0
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Change%20failure%20rate-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Change%20failure%20rate%20(0.00%25)-Elite-brightgreen", model.BadgeWithMetricURL);
        }

        
        [TestMethod]
        public void ChangeFailureRateControllerHighBadgeTest()
        {
            //Arrange
            ChangeFailureRateModel model = new ChangeFailureRateModel
            {
                ChangeFailureRateMetricDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Change%20failure%20rate-High-green", model.BadgeURL);
        }

        
        [TestMethod]
        public void ChangeFailureRateControllerMediumBadgeTest()
        {
            //Arrange
            ChangeFailureRateModel model = new ChangeFailureRateModel
            {
                ChangeFailureRateMetricDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Change%20failure%20rate-Medium-orange", model.BadgeURL);
        }

        
        [TestMethod]
        public void ChangeFailureRateControllerLowBadgeTest()
        {
            //Arrange
            ChangeFailureRateModel model = new ChangeFailureRateModel
            {
                ChangeFailureRateMetricDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Change%20failure%20rate-Low-red", model.BadgeURL);
        }

        
        [TestMethod]
        public void ChangeFailureRateControllerNoneBadgeTest()
        {
            //Arrange
            ChangeFailureRateModel model = new ChangeFailureRateModel
            {
                ChangeFailureRateMetricDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Change%20failure%20rate-None-lightgrey", model.BadgeURL);
        }

    }
}
