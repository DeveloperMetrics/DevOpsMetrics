using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class PostiveNegativeListDATests
    {      

        [TestMethod]
        public void PositiveNegative000PercentUnitTest()
        {
            //Arrange
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            int percent = 0;
            int samples = 100;
            List<ChangeFailureRateBuild> builds = GenerateSamples(samples);

            //Act
            Tuple<List<ChangeFailureRateBuild>, List<ChangeFailureRateBuild>> positiveAndNegativeBuilds = da.GetPositiveAndNegativeLists(percent, builds);
            List<ChangeFailureRateBuild> positiveBuilds = positiveAndNegativeBuilds.Item1;
            List<ChangeFailureRateBuild> negativeBuilds = positiveAndNegativeBuilds.Item2;

            //Assert
            Assert.AreEqual(0, positiveBuilds.Count);
            Assert.AreEqual(100, negativeBuilds.Count);

        }

        [TestMethod]
        public void PositiveNegative010PercentUnitTest()
        {
            //Arrange
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            int percent = 10;
            int samples = 100;
            List<ChangeFailureRateBuild> builds = GenerateSamples(samples);

            //Act
            Tuple<List<ChangeFailureRateBuild>, List<ChangeFailureRateBuild>> positiveAndNegativeBuilds = da.GetPositiveAndNegativeLists(percent, builds);
            List<ChangeFailureRateBuild> positiveBuilds = positiveAndNegativeBuilds.Item1;
            List<ChangeFailureRateBuild> negativeBuilds = positiveAndNegativeBuilds.Item2;

            //Assert
            Assert.AreEqual(10, positiveBuilds.Count);
            Assert.AreEqual(90, negativeBuilds.Count);

        }

        [TestMethod]
        public void PositiveNegative025PercentUnitTest()
        {
            //Arrange
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            int percent = 25;
            int samples = 100;
            List<ChangeFailureRateBuild> builds = GenerateSamples(samples);

            //Act
            Tuple<List<ChangeFailureRateBuild>, List<ChangeFailureRateBuild>> positiveAndNegativeBuilds = da.GetPositiveAndNegativeLists(percent, builds);
            List<ChangeFailureRateBuild> positiveBuilds = positiveAndNegativeBuilds.Item1;
            List<ChangeFailureRateBuild> negativeBuilds = positiveAndNegativeBuilds.Item2;

            //Assert
            Assert.AreEqual(25, positiveBuilds.Count);
            Assert.AreEqual(75, negativeBuilds.Count);
        }

        [TestMethod]
        public void PositiveNegative050PercentUnitTest()
        {
            //Arrange
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            int percent = 50;
            int samples = 100;
            List<ChangeFailureRateBuild> builds = GenerateSamples(samples);

            //Act
            Tuple<List<ChangeFailureRateBuild>, List<ChangeFailureRateBuild>> positiveAndNegativeBuilds = da.GetPositiveAndNegativeLists(percent, builds);
            List<ChangeFailureRateBuild> positiveBuilds = positiveAndNegativeBuilds.Item1;
            List<ChangeFailureRateBuild> negativeBuilds = positiveAndNegativeBuilds.Item2;

            //Assert
            Assert.AreEqual(50, positiveBuilds.Count);
            Assert.AreEqual(50, negativeBuilds.Count);
        }

        [TestMethod]
        public void PositiveNegative075PercentUnitTest()
        {
            //Arrange
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            int percent = 75;
            int samples = 100;
            List<ChangeFailureRateBuild> builds = GenerateSamples(samples);

            //Act
            Tuple<List<ChangeFailureRateBuild>, List<ChangeFailureRateBuild>> positiveAndNegativeBuilds = da.GetPositiveAndNegativeLists(percent, builds);
            List<ChangeFailureRateBuild> positiveBuilds = positiveAndNegativeBuilds.Item1;
            List<ChangeFailureRateBuild> negativeBuilds = positiveAndNegativeBuilds.Item2;

            //Assert
            Assert.AreEqual(75, positiveBuilds.Count);
            Assert.AreEqual(25, negativeBuilds.Count);
        }

        [TestMethod]
        public void PositiveNegative098PercentUnitTest()
        {
            //Arrange
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            int percent = 98;
            int samples = 100;
            List<ChangeFailureRateBuild> builds = GenerateSamples(samples);

            //Act
            Tuple<List<ChangeFailureRateBuild>, List<ChangeFailureRateBuild>> positiveAndNegativeBuilds = da.GetPositiveAndNegativeLists(percent, builds);
            List<ChangeFailureRateBuild> positiveBuilds = positiveAndNegativeBuilds.Item1;
            List<ChangeFailureRateBuild> negativeBuilds = positiveAndNegativeBuilds.Item2;

            //Assert
            Assert.AreEqual(98, positiveBuilds.Count);
            Assert.AreEqual(2, negativeBuilds.Count);
        }

        [TestMethod]
        public void PositiveNegative100PercentUnitTest()
        {
            //Arrange
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            int percent = 100;
            int samples = 100;
            List<ChangeFailureRateBuild> builds = GenerateSamples(samples);

            //Act
            Tuple<List<ChangeFailureRateBuild>, List<ChangeFailureRateBuild>> positiveAndNegativeBuilds = da.GetPositiveAndNegativeLists(percent, builds);
            List<ChangeFailureRateBuild> positiveBuilds = positiveAndNegativeBuilds.Item1;
            List<ChangeFailureRateBuild> negativeBuilds = positiveAndNegativeBuilds.Item2;

            //Assert
            Assert.AreEqual(100, positiveBuilds.Count);
            Assert.AreEqual(0, negativeBuilds.Count);
        }

        private List<ChangeFailureRateBuild> GenerateSamples(int count)
        {
            List<ChangeFailureRateBuild> builds = new List<ChangeFailureRateBuild>();
            for (int i = 1; i <= count; i++)
            {
                builds.Add(new ChangeFailureRateBuild());
            }

            return builds;
        }

    }
}
