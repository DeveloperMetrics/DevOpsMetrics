using System;
using System.Collections.Generic;
using DevOpsMetrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class SLATests
    {

        [TestMethod]
        public void SLANoneTest()
        {
            //Goal: We have less than 90.0% uptime (downtime less than the Daily: 2h 24m 0s or Weekly: 16h 48m 0s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new();

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(-1, result);
            Assert.AreEqual("no data", slaDescription);
        }

        [TestMethod]
        public void SLANoNineTest()
        {
            //Goal: We have less than 90.0% uptime (downtime less than the Daily: 2h 24m 0s or Weekly: 16h 48m 0s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-1), new TimeSpan(23, 00, 0))
            };

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(0.8630952f, result);
            Assert.AreEqual("less than 90% SLA", slaDescription);
        }

        [TestMethod]
        public void SLAOneNineTest()
        {
            //Goal: We have more than 90.0% uptime (downtime no more than Daily: 2h 24m 0s or Weekly: 16h 48m 0s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(16, 48, 0))
            };

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(0.9f, result);
            Assert.AreEqual("over 90.0% SLA", slaDescription);
        }

        [TestMethod]
        public void SLATwoNinesTest()
        {
            //Goal: We have more than 99.0% uptime (downtime no more than Daily: 14m 24s or Weekly: 1h 40m 48s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(1, 40, 0))
            };

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(0.990079343f, result);
            Assert.AreEqual("over 99.0% SLA", slaDescription);
        }


        [TestMethod]
        public void SLAThreeNinesTest()
        {
            //Goal: We have more than 99.9% uptime (downtime no more than Daily: 1m 26s or Weekly: 10m 4s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 10, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 0, 4)),
            };

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(0.9990014f, result);
            Assert.AreEqual("over 99.9% SLA", slaDescription);
        }

        [TestMethod]
        public void SLAFourNinesTest()
        {
            //Goal: We have more than 99.99% uptime (downtime no more than Daily: 8s or Weekly: 1m 0s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 1, 0))
            };

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(0.9999008f, result);
            Assert.AreEqual("over 99.99% SLA", slaDescription);
        }

        [TestMethod]
        public void SLAFiveNinesTest()
        {
            //Goal: We have more than 99.999% uptime (downtime no more than Daily: 0s or Weekly: 6s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 0, 6)),
            };

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(0.9999901f, result);
            Assert.AreEqual("over 99.999% SLA", slaDescription);
        }

        [TestMethod]
        public void SLASixNinesTest()
        {
            //Goal: We have more than 99.9999% uptime (downtime no more than Daily: 0s or Weekly: 0s)

            //Arrange
            SLA metrics = new();
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> SLAList = new()
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3), new TimeSpan(0, 0, 0)),
            };

            //Act
            float result = metrics.ProcessSLA(SLAList, numberOfDays);
            string slaDescription = SLA.GetSLARating(result);

            //Assert
            Assert.AreEqual(1f, result);
            Assert.AreEqual("over 99.9999% SLA", slaDescription);
        }


    }
}
