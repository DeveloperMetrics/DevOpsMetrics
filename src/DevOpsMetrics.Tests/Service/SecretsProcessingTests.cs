using DevOpsMetrics.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class SecretsProcessingTests
    {
        [TestMethod]
        public void SecretWithDotTest()
        {
            //Arrange
            string name = "Secret.CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("Secret-CI", result);
        }

        [TestMethod]
        public void SecretWithSpaceTest()
        {
            //Arrange
            string name = "Secret CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("Secret-CI", result);
        }

        [TestMethod]
        public void SecretWithColonTest()
        {
            //Arrange
            string name = "Secret:CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("Secret-CI", result);
        }

        [TestMethod]
        public void SecretWithQuestionTest()
        {
            //Arrange
            string name = "Secret?CI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("Secret-CI", result);
        }

        [TestMethod]
        public void SecretThatIsValidTest()
        {
            //Arrange
            string name = "Secret123abcCI";

            //Act
            string result = SecretsProcessing.CleanKey(name);

            //Assert
            Assert.AreEqual("Secret123abcCI", result);
        }

    }
}
