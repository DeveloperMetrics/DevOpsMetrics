using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class MeanTimeToRestoreDATests
    {
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<DeploymentFrequencyDATests>();
            Configuration = config.Build();
        }

        [TestMethod]
        public async Task MeanTimeToRestoreDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string resourceGroup = "DevOpsMetricsTestRG";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            MeanTimeToRestoreDA da = new MeanTimeToRestoreDA();
            MeanTimeToRestoreModel model = await da.GetAzureMeanTimeToRestore(getSampleData, tableStorageAuth, resourceGroup, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.IsAzureDevOps == true);
            Assert.AreEqual(resourceGroup, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count > 0);
            Assert.IsTrue(model.MTTRAverageDurationInMinutes > 0);
        }


        [TestMethod]
        public async Task MeanTimeToRestoreDALiveIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string resourceGroup = "SamLearnsAzureProd";
            bool isAzureDevOps = true;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            MeanTimeToRestoreDA da = new MeanTimeToRestoreDA();
            MeanTimeToRestoreModel model = await da.GetAzureMeanTimeToRestore(getSampleData, tableStorageAuth, resourceGroup, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            //Assert.IsTrue(model.IsAzureDevOps == isAzureDevOps);
            Assert.AreEqual(resourceGroup, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count > 0);
            Assert.IsTrue(model.MTTRAverageDurationInMinutes > 0);
        }
    }
}
