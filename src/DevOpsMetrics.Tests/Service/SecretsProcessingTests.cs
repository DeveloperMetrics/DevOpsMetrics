using DevOpsMetrics.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L0Test")]
    [TestClass]
    public class SecretsProcessingTests
    {
        [TestMethod]
        public void SecretWithDotTest()
        {
            //Arrange
            string name = "SamLearnsAzure.CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("SamLearnsAzure-CI", result);
        }

        [TestMethod]
        public void SecretWithSpaceTest()
        {
            //Arrange
            string name = "SamLearnsAzure CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("SamLearnsAzure-CI", result);
        }

        [TestMethod]
        public void SecretWithColonTest()
        {
            //Arrange
            string name = "SamLearnsAzure:CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("SamLearnsAzure-CI", result);
        }

        [TestMethod]
        public void SecretWithQuestionTest()
        {
            //Arrange
            string name = "SamLearnsAzure?CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("SamLearnsAzure-CI", result);
        }

        [TestMethod]
        public void SecretThatIsValidTest()
        {
            //Arrange
            string name = "SamLearnsAzure123abcCI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("SamLearnsAzure123abcCI", result);
        }

    }
}
