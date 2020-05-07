using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class ChangeFailureRateTests
    {
     
        [TestMethod]
        public void ChangeFailureRateSingleOneDayTest()
        {
            //Arrange
            ChangeFailureRate metrics = new ChangeFailureRate();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, bool>> changeFailureRateList = new List<KeyValuePair<DateTime, bool>>
            {
                new KeyValuePair<DateTime, bool>(DateTime.Now, true)
            };

            //Act
            float result = metrics.ProcessLeadTimeForChanges(changeFailureRateList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(1f, result);
        }

        [TestMethod]
        public void ChangeFailureRateNullOneDayTest()
        {
            //Arrange
            ChangeFailureRate metrics = new ChangeFailureRate();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, bool>> changeFailureRateList = null;

            //Act
            float result = metrics.ProcessLeadTimeForChanges(changeFailureRateList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(0f, result);
        }

        [TestMethod]
        public void ChangeFailureRateFiveSevenDaysTest()
        {
            //Arrange
            ChangeFailureRate metrics = new ChangeFailureRate();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, bool>> changeFailureRateList = new List<KeyValuePair<DateTime, bool>>
            {
                new KeyValuePair<DateTime, bool>(DateTime.Now, true),
                new KeyValuePair<DateTime, bool>(DateTime.Now.AddDays(-1), false),
                new KeyValuePair<DateTime, bool>(DateTime.Now.AddDays(-2), true),
                new KeyValuePair<DateTime, bool>(DateTime.Now.AddDays(-3), false),
                new KeyValuePair<DateTime, bool>(DateTime.Now.AddDays(-4), true),
                new KeyValuePair<DateTime, bool>(DateTime.Now.AddDays(-14), true) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessLeadTimeForChanges(changeFailureRateList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(0.6f, result);
        }

    }
}