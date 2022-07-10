using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L1Test")]
    [TestClass]
    public class MeanTimeToRestoreDATests : BaseConfiguration
    {
        [TestMethod]
        public void MeanTimeToRestoreDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "DevOpsMetricsTestRG";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.AreEqual(resourceGroup, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count > 0);
            Assert.IsTrue(model.MTTRAverageDurationInHours > 0);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        [TestMethod]
        public void MeanTimeToRestoreDASamLearnsAzureProdIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "SamLearnsAzureProd";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.AreEqual(resourceGroup, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count >= 0);
            Assert.IsTrue(model.MTTRAverageDurationInHours >= 0);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }


        [TestMethod]
        public void TimeToRestoreServiceDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "SamLearnsAzureProd";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.AreEqual(resourceGroup, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count >= 0);
            Assert.IsTrue(model.MTTRAverageDurationInHours >= 0);
            Assert.IsTrue(model.MTTRAverageDurationDescription != "");
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }

        [TestMethod]
        public void TimeToRestoreServiceDALiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "SamLearnsAzureProd";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.AreEqual(resourceGroup, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count >= 0);
            Assert.IsTrue(model.MTTRAverageDurationInHours >= 0);
            Assert.IsTrue(model.MTTRAverageDurationDescription != "");
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }
    }
}
