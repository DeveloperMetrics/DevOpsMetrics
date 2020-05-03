using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Tests.Core
{
    [TestCategory("UnitTest")]
    [TestClass]
    public class MeanTimeToRestoreTests
    {
        [TestMethod]
        public void MeanTimeToRestoreSingleOneDayTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new MeanTimeToRestore();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = new List<KeyValuePair<DateTime, TimeSpan>>
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(0, 45, 0))
            };

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(45f, result);
        }

        [TestMethod]
        public void MeanTimeToRestoreNullOneDayTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new MeanTimeToRestore();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = null;

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(0f, result);
        }

        [TestMethod]
        public void MeanTimeToRestoreFiveSevenDaysTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new MeanTimeToRestore();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> meanTimeToRestoreList = new List<KeyValuePair<DateTime, TimeSpan>>
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(2, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-1), new TimeSpan(0, 45, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-2), new TimeSpan(1, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 40, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-4), new TimeSpan(0, 50, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-14), new TimeSpan(12, 0, 0)) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessMeanTimeToRestore(meanTimeToRestoreList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(63f, result);
        }

    }
}
