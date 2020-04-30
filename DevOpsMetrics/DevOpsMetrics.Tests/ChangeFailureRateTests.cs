using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests
{
    [TestClass]
    public class ChangeFailureRateTests
    {
        [TestMethod]
        public void AddChangeFailureRateTest()
        {
            //Arrange
            ChangeFailureRate metrics = new ChangeFailureRate();
            string pipelineName = "TestPipeline.CI";
            bool deploymentSuccessful = true;

            //Act
            bool result = metrics.AddChangeFailureRate(pipelineName, deploymentSuccessful);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CalculateChangeFailureRateTest()
        {
            //Arrange
            ChangeFailureRate metrics = new ChangeFailureRate();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;

            //Act
            float result = metrics.CalculateChangeFailureRate(pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(0.6f, result);
        }
    }
}
