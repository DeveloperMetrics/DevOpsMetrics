using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DevOpsMetrics.Tests
{
    [TestClass]
    public class DeploymentFrequencyTests
    {
        [TestMethod]
        public void AddDeploymentFrequencyTest()
        {
            //Arrange
            DeploymentFrequency metrics = new DeploymentFrequency();
            string pipelineName = "TestPipeline.CI";
            DateTime deploymentTime = DateTime.Now;

            //Act
            bool result = metrics.AddDeploymentFrequency(pipelineName, deploymentTime);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CalculateDeploymentFrequencyTest()
        {
            //Arrange
            DeploymentFrequency metrics = new DeploymentFrequency();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;

            //Act
            float result = metrics.CalculateDeploymentFrequency(pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(1.4f, result);
        }
    }
}
