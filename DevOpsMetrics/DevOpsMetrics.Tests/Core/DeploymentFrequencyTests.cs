using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Tests.Core
{
    [TestClass]
    public class DeploymentFrequencyTests
    {
        [TestMethod]
        public void DeploymentFrequencySingleOneDayTest()
        {
            //Arrange
            DeploymentFrequency metrics = new DeploymentFrequency();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, DateTime>> deploymentFrequencyList = new List<KeyValuePair<DateTime, DateTime>>
            {
                new KeyValuePair<DateTime, DateTime>(DateTime.Now, DateTime.Now)
            };

            //Act
            float result = metrics.ProcessDeploymentFrequency(deploymentFrequencyList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(1f, result);
        }

        [TestMethod]
        public void DeploymentFrequencyNullOneDayTest()
        {
            //Arrange
            DeploymentFrequency metrics = new DeploymentFrequency();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, DateTime>> deploymentFrequencyList = null;

            //Act
            float result = metrics.ProcessDeploymentFrequency(deploymentFrequencyList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(0f, result);
        }

        [TestMethod]
        public void DeploymentFrequencyFiveSevenDaysTest()
        {
            //Arrange
            DeploymentFrequency metrics = new DeploymentFrequency();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, DateTime>> deploymentFrequencyList = new List<KeyValuePair<DateTime, DateTime>>
            {
                new KeyValuePair<DateTime, DateTime>(DateTime.Now, DateTime.Now),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-2)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-3)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-4), DateTime.Now.AddDays(-4)),
                new KeyValuePair<DateTime, DateTime>(DateTime.Now.AddDays(-14), DateTime.Now.AddDays(-14)) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessDeploymentFrequency(deploymentFrequencyList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(0.71428573f, result);
        }

    }
}