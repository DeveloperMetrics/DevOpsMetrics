using DevOpsMetrics.Core.Models.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class BadgeTests
    {

        [TestMethod]
        public void DeploymentFrequencyEliteBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetricDescription = "Elite",
                DeploymentsPerDayMetric = 12
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency%20%2812.00%20per%20day%29-Elite-brightgreen", model.BadgeWithMetricURL);
        }


        [TestMethod]
        public void DeploymentFrequencyHighBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetricDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-High-green", model.BadgeURL);
        }


        [TestMethod]
        public void DeploymentFrequencyMediumBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetricDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-Medium-orange", model.BadgeURL);
        }


        [TestMethod]
        public void DeploymentFrequencyLowBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetricDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-Low-red", model.BadgeURL);
        }


        [TestMethod]
        public void DeploymentFrequencyNoneBadgeTest()
        {
            //Arrange
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetricDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Deployment%20frequency-None-lightgrey", model.BadgeURL);
        }

        [TestMethod]
        public void ChangeFailureRateControllerHighBadgeTest()
        {
            //Arrange
            ChangeFailureRateModel model = new()
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
            ChangeFailureRateModel model = new()
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
            ChangeFailureRateModel model = new()
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
            ChangeFailureRateModel model = new()
            {
                ChangeFailureRateMetricDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Change%20failure%20rate-None-lightgrey", model.BadgeURL);
        }

        [TestMethod]
        public void LeadTimeForChangesEliteBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new()
            {
                LeadTimeForChangesMetricDescription = "Elite",
                LeadTimeForChangesMetric = 5.3f
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes%20%285.3%20hours%29-Elite-brightgreen", model.BadgeWithMetricURL);
        }


        [TestMethod]
        public void LeadTimeForChangesHighBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new()
            {
                LeadTimeForChangesMetricDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-High-green", model.BadgeURL);
        }


        [TestMethod]
        public void LeadTimeForChangesMediumBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new()
            {
                LeadTimeForChangesMetricDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-Medium-orange", model.BadgeURL);
        }


        [TestMethod]
        public void LeadTimeForChangesLowBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new()
            {
                LeadTimeForChangesMetricDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-Low-red", model.BadgeURL);
        }


        [TestMethod]
        public void LeadTimeForChangesNoneBadgeTest()
        {
            //Arrange
            LeadTimeForChangesModel model = new()
            {
                LeadTimeForChangesMetricDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual("https://img.shields.io/badge/Lead%20time%20for%20changes-None-lightgrey", model.BadgeURL);
        }


        [TestMethod]
        public void MeanTimeToRestoreEliteBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new()
            {
                MTTRAverageDurationDescription = "Elite",
                MTTRAverageDurationInHours = 0.12f
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service%20%280.12%20hours%29-Elite-brightgreen", model.BadgeWithMetricURL);
        }


        [TestMethod]
        public void MeanTimeToRestoreHighBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new()
            {
                MTTRAverageDurationDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-High-green", model.BadgeURL);
        }


        [TestMethod]
        public void MeanTimeToRestoreMediumBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new()
            {
                MTTRAverageDurationDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange", model.BadgeURL);
        }


        [TestMethod]
        public void MeanTimeToRestoreLowBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new()
            {
                MTTRAverageDurationDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Low-red", model.BadgeURL);
        }


        [TestMethod]
        public void MeanTimeToRestoreNoneBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new()
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
