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
    public class MeanTimeToRestoreTests
    {

        [TestMethod]
        public void MTTRHigh2Test()
        {
            //Arrange
            MeanTimeToRestore metrics = new();
            SLA slaMetrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 40, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-4), new TimeSpan(0, 50, 0)),
            };

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, numberOfDays);
            float sla = slaMetrics.ProcessSLA(meanTimeToRestoreList, numberOfDays);
            MeanTimeToRestoreModel model = new()
            {
                MTTRAverageDurationInHours = result,
                MTTRAverageDurationDescription = MeanTimeToRestore.GetMeanTimeToRestoreRating(result),
                IsProjectView = false,
                ItemOrder = 1,
                SLA = sla,
                SLADescription = SLA.GetSLARating(sla)
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(0.75f, model.MTTRAverageDurationInHours);
            Assert.AreEqual("High", model.MTTRAverageDurationDescription);
            Assert.AreEqual(false, model.IsProjectView);
            Assert.AreEqual(1, model.ItemOrder);
            Assert.AreEqual(0.9910714f, model.SLA);
            Assert.AreEqual("over 99.0% SLA", model.SLADescription);
        }


        [TestMethod]
        public void MTTRHighTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(2, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-1), new TimeSpan(0, 45, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-2), new TimeSpan(1, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 40, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-4), new TimeSpan(0, 50, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-14), new TimeSpan(12, 0, 0)) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, numberOfDays);
            string rating = MeanTimeToRestore.GetMeanTimeToRestoreRating(result);

            //Assert
            Assert.AreEqual(1.05f, result);
            Assert.AreEqual("High", rating);
        }

        [TestMethod]
        public void MTTRMediumTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new();
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(25, 0, 0))
            };

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, numberOfDays);
            string rating = MeanTimeToRestore.GetMeanTimeToRestoreRating(result);

            //Assert
            Assert.AreEqual(25f, result);
            Assert.AreEqual("Medium", rating);
        }

        [TestMethod]
        public void MTTRLowTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new();
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(170, 0, 0))
            };

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, numberOfDays);
            string rating = MeanTimeToRestore.GetMeanTimeToRestoreRating(result);

            //Assert
            Assert.AreEqual(170f, result);
            Assert.AreEqual("Low", rating);
        }

        [TestMethod]
        public void MTTRNoneTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new();
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = null;

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, numberOfDays);
            string rating = MeanTimeToRestore.GetMeanTimeToRestoreRating(result);

            //Assert
            Assert.AreEqual(0f, result);
            Assert.AreEqual("None", rating);
        }

    }
}
