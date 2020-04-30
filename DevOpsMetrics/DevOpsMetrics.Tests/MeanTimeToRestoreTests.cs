using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DevOpsMetrics.Tests
{
    [TestClass]
    public class MeanTimeToRestoreTests
    {
        [TestMethod]
        public void AddMeanTimeToRestoreTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new MeanTimeToRestore();
            string pipelineName = "TestPipeline.CI";
            TimeSpan timeDuration = new TimeSpan(0, 45, 0);

            //Act
            bool result = metrics.AddMeanTimeToRestore(pipelineName, timeDuration);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CalculateMeanTimeToRestoreTest()
        {
            //Arrange
            MeanTimeToRestore metrics = new MeanTimeToRestore();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;

            //Act
            float result = metrics.CalculateMeanTimeToRestore(pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(63f, result);
        }
    }
}
