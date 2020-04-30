using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DevOpsMetrics.Tests
{
    [TestClass]
    public class LeadTimeForChangesTests
    {
        [TestMethod]
        public void AddLeadTimeForChangesTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            TimeSpan timeDuration = new TimeSpan(0, 45, 0);

            //Act
            bool result = metrics.AddLeadTimeForChanges(pipelineName, timeDuration);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CalculateLeadTimeForChangesTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;

            //Act
            float result = metrics.CalculateLeadTimeForChanges(pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(63f, result);
        }
    }
}
