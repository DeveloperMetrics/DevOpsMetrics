using DevOpsMetrics.Service.DataAccess.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class FractionConverterTests
    {
                [TestMethod]
        public void Fraction0PercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 0;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(0, model.Numerator);
            Assert.AreEqual(1, model.Denominator);
        }

        [TestMethod]
        public void Fraction10PercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 10;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(1, model.Numerator);
            Assert.AreEqual(10, model.Denominator);
        }

        [TestMethod]
        public void Fraction25PercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 25;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(1, model.Numerator);
            Assert.AreEqual(4, model.Denominator);
        }

        [TestMethod]
        public void Fraction50PercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 50;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(1, model.Numerator);
            Assert.AreEqual(2, model.Denominator);
        }

        [TestMethod]
        public void Fraction75PercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 75;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(3, model.Numerator);
            Assert.AreEqual(4, model.Denominator);
        }

        [TestMethod]
        public void Fraction98PercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 98;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(49, model.Numerator);
            Assert.AreEqual(50, model.Denominator);
        }

        [TestMethod]
        public void Fraction100PercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 100;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(1, model.Numerator);
            Assert.AreEqual(1, model.Denominator);
        }

        [TestMethod]
        public void FractionOtherPercentTest()
        {
            //Arrange
            FractionConverter fraction = new FractionConverter();
            int percent = 1000;

            //Act
            FractionModel model = fraction.ConvertToFraction(percent);

            //Assert
            Assert.AreNotEqual(null, model);
            Assert.AreEqual(1, model.Numerator);
            Assert.AreEqual(1, model.Denominator);
        }
    }
}
