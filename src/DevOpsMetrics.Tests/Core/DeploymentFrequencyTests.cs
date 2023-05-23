using System;
using System.Collections.Generic;
using DevOpsMetrics.Core;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class DeploymentFrequencyTests
    {
        [TestMethod]
        public void DeploymentFrequencySingleOneDayTest()
        {
            //Arrange
            DeploymentFrequency metrics = new();
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, DateTime>> deploymentFrequencyList = new()
            {
                new KeyValuePair<DateTime, DateTime>(DateTime.Now, DateTime.Now)
            };

            //Act
            float metric = metrics.ProcessDeploymentFrequency(deploymentFrequencyList, numberOfDays);
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(1f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("High", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(7, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("times per week", model.DeploymentsToDisplayUnit);
        }

        [TestMethod]
        public void DeploymentFrequencyNullOneDayTest()
        {
            //Arrange
            DeploymentFrequency metrics = new();
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, DateTime>> deploymentFrequencyList = null;

            //Act
            float metric = metrics.ProcessDeploymentFrequency(deploymentFrequencyList, numberOfDays);
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric),
                IsProjectView = true,
                ItemOrder = 1,
                RateLimitHit = false
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(0f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("None", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(0, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("times per year", model.DeploymentsToDisplayUnit);
            Assert.AreEqual(true, model.IsProjectView);
            Assert.AreEqual(1, model.ItemOrder);
            Assert.AreEqual(false, model.RateLimitHit);
        }

        [TestMethod]
        public void DeploymentFrequencyFiveSevenDaysTest()
        {
            //Arrange
            DeploymentFrequency metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, DateTime>> deploymentFrequencyList = new()
            {
                new KeyValuePair<DateTime, DateTime>(DateTime.Now, DateTime.Now),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-2)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-3)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-4), DateTime.Now.AddDays(-4)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-8), DateTime.Now.AddDays(-8)) //this record should be out of range, as it's older than 7 days
            };

            //Act
            float metric = metrics.ProcessDeploymentFrequency(deploymentFrequencyList, numberOfDays);
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(0.7143f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("Medium", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(5.0000997f, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("times per week", model.DeploymentsToDisplayUnit);
        }

        [TestMethod]
        public void DeploymentFrequencyRatingHigh2Test()
        {
            //Arrange
            float metric = 1.01f; //daily

            //Act
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(1.01f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("High", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(1.01f, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("per day", model.DeploymentsToDisplayUnit);
        }

        [TestMethod]
        public void DeploymentFrequencyRatingHighTest()
        {
            //Arrange
            float metric = 1f / 7f; //weekly

            //Act
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(1f / 7f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("Medium", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(1f, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("times per week", model.DeploymentsToDisplayUnit);
        }

        [TestMethod]
        public void DeploymentFrequencyRatingMediumTest()
        {
            //Arrange
            float metric = 1f / (30f * 6); //six monthly

            //Act
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(0.0055555557f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("Low", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(0.16666667f, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("times per month", model.DeploymentsToDisplayUnit);

        }

        [TestMethod]
        public void DeploymentFrequencyRatingLowTest()
        {
            //Arrange
            float metric = (1f / 365f); //more once than every 6 monthes

            //Act
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(metric, model.DeploymentsPerDayMetric);
            Assert.AreEqual("Low", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(1f, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("times per year", model.DeploymentsToDisplayUnit);
        }

        [TestMethod]
        public void DeploymentFrequencyRatingZeroNoneTest()
        {
            //Arrange
            float metric = 0f; //None

            //Act
            DeploymentFrequencyModel model = new()
            {
                DeploymentsPerDayMetric = metric,
                DeploymentsPerDayMetricDescription = DeploymentFrequency.GetDeploymentFrequencyRating(metric)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(0f, model.DeploymentsPerDayMetric);
            Assert.AreEqual("None", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(0, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("times per year", model.DeploymentsToDisplayUnit);
        }

    }
}