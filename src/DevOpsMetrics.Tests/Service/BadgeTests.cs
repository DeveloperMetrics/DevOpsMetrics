using DevOpsMetrics.Core.Models.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L0Test")]
    [TestClass]
    public class BadgeTests
    {

        [TestCategory("L0Test")]
        [TestMethod]
        public void DeploymentFrequencyEliteBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                DeploymentsPerDayMetricDescription = "Elite",
                DeploymentsPerDayMetric = 12
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency%20(12.00%20per%20day)-Elite-brightgreen", model.BadgeWithMetricURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void DeploymentFrequencyHighBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                DeploymentsPerDayMetricDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-High-green", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void DeploymentFrequencyMediumBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                DeploymentsPerDayMetricDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-Medium-orange", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void DeploymentFrequencyLowBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                DeploymentsPerDayMetricDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-Low-red", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void DeploymentFrequencyNoneBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new DeploymentFrequencyModel
            {
                DeploymentsPerDayMetricDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-None-lightgrey", model.BadgeURL);
        }

        [TestCategory("L0Test")]
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

        [TestCategory("L0Test")]
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


        [TestCategory("L0Test")]
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


        [TestCategory("L0Test")]
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


        [TestCategory("L0Test")]
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

        [TestCategory("L0Test")]
        [TestMethod]
        public void LeadTimeForChangesEliteBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new LeadTimeForChangesModel
            {
                LeadTimeForChangesMetricDescription = "Elite",
                LeadTimeForChangesMetric = 5.3f
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes%20(5.3%20hours)-Elite-brightgreen", model.BadgeWithMetricURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void LeadTimeForChangesHighBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new LeadTimeForChangesModel
            {
                LeadTimeForChangesMetricDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-High-green", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void LeadTimeForChangesMediumBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new LeadTimeForChangesModel
            {
                LeadTimeForChangesMetricDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-Medium-orange", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void LeadTimeForChangesLowBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new LeadTimeForChangesModel
            {
                LeadTimeForChangesMetricDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-Low-red", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void LeadTimeForChangesNoneBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new LeadTimeForChangesModel
            {
                LeadTimeForChangesMetricDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-None-lightgrey", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void MeanTimeToRestoreEliteBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "Elite",
                MTTRAverageDurationInHours = 0.12f
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service%20(0.12%20hours)-Elite-brightgreen", model.BadgeWithMetricURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void MeanTimeToRestoreHighBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-High-green", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void MeanTimeToRestoreMediumBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void MeanTimeToRestoreLowBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Low-red", model.BadgeURL);
        }


        [TestCategory("L0Test")]
        [TestMethod]
        public void MeanTimeToRestoreNoneBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-None-lightgrey", model.BadgeURL);
        }
    }
}
