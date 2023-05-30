using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class MeanTimeToRestoreControllerTests : BaseConfiguration
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task AzureMTTRSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string resourceGroupName = "DevOpsMetrics";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            MeanTimeToRestoreController controller = new(base.Configuration);

            //Act
            MeanTimeToRestoreModel model = await controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(targetDevOpsPlatform, model.TargetDevOpsPlatform);
            Assert.AreEqual(resourceGroupName, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count > 0);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Name == "Name1");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Resource == "Resource1");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Status == "Completed");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Url == "https://mttr.com");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].StartTime > DateTime.MinValue);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].EndTime > DateTime.MinValue);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ResourceGroup == resourceGroupName);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationInHours > 0f);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationPercent > 0f);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ItemOrder == 1);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        [TestMethod]
        public async Task AzureMTTRsAPIControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string resourceGroupName = "DevOpsMetrics";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;
            MeanTimeToRestoreController controller = new(base.Configuration);

            //Act
            MeanTimeToRestoreModel model = await controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(targetDevOpsPlatform, model.TargetDevOpsPlatform);
            Assert.AreEqual(resourceGroupName, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count >= 0);
            if (model.MeanTimeToRestoreEvents.Count > 0)
            {
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Name != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Resource != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Status != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Url != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].StartTime >= DateTime.MinValue);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].EndTime >= DateTime.MinValue);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ResourceGroup != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationInHours > 0f);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationPercent > 0f);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ItemOrder >= 1);
            }
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }

        [TestMethod]
        public async Task AzureMTTRsAPINullIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string resourceGroupName = null;
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;
            MeanTimeToRestoreController controller = new(base.Configuration);

            //Act
            MeanTimeToRestoreModel model = await controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(targetDevOpsPlatform, model.TargetDevOpsPlatform);
            Assert.AreEqual("", model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count >= 0);
            if (model.MeanTimeToRestoreEvents.Count > 0)
            {
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Name != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Resource != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Status != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Url != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].StartTime >= DateTime.MinValue);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].EndTime >= DateTime.MinValue);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ResourceGroup != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationInHours > 0f);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationPercent > 0f);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ItemOrder >= 1);
            }
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }

        [TestMethod]
        public async Task AzureMTTRsAPIEmptyIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string resourceGroupName = "";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;
            MeanTimeToRestoreController controller = new(base.Configuration);

            //Act
            MeanTimeToRestoreModel model = await controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(targetDevOpsPlatform, model.TargetDevOpsPlatform);
            Assert.AreEqual(resourceGroupName, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count >= 0);
            if (model.MeanTimeToRestoreEvents.Count > 0)
            {
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Name != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Resource != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Status != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Url != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].StartTime >= DateTime.MinValue);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].EndTime >= DateTime.MinValue);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ResourceGroup != "");
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationInHours > 0f);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationPercent > 0f);
                Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ItemOrder >= 1);
            }
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems >= 0);
            Assert.IsTrue(model.TotalItems >= 0);
        }

    }
}
