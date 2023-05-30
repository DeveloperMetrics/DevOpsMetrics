using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class MeanTimeToRestoreDATests : BaseConfiguration
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task MeanTimeToRestoreDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "DevOpsMetricsTestRG";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = await MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

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
        public async Task MeanTimeToRestoreDADevOpsMetricsProdIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "DevOpsMetrics";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = await MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

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
        public async Task TimeToRestoreServiceDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "DevOpsMetrics";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = await MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

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
        public async Task TimeToRestoreServiceDALiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string resourceGroup = "DevOpsMetrics";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;

            //Act
            MeanTimeToRestoreModel model = await MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig, targetDevOpsPlatform, resourceGroup, numberOfDays, maxNumberOfItems);

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
