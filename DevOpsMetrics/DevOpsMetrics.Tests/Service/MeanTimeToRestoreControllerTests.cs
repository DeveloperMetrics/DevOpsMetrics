using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class MeanTimeToRestoreControllerTests
    {
        private TestServer _server;
        public HttpClient Client;
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<DeploymentFrequencyControllerTests>();
            Configuration = config.Build();

            //Setup the test server
            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseConfiguration(Configuration)
                .UseStartup<DevOpsMetrics.Service.Startup>());
            Client = _server.CreateClient();
            //Client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task AzureMTTRSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string resourceGroupName = "SamLearnsAzureProd";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            bool useCache = false;
            MeanTimeToRestoreController controller = new MeanTimeToRestoreController(Configuration);

            //Act
            MeanTimeToRestoreModel model = await controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems, useCache);

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
        }

        [TestCategory("ControllerTest")]
        [TestMethod]
        public async Task AzureMTTRsAPIControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string resourceGroupName = "SamLearnsAzureProd";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;
            MeanTimeToRestoreController controller = new MeanTimeToRestoreController(Configuration);

            //Act
            MeanTimeToRestoreModel model = await controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(targetDevOpsPlatform, model.TargetDevOpsPlatform);
            Assert.AreEqual(resourceGroupName, model.ResourceGroup);
            Assert.IsTrue(model.MeanTimeToRestoreEvents.Count > 0);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Name != "");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Resource != "");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Status != "");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].Url != "");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].StartTime >= DateTime.MinValue);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].EndTime >= DateTime.MinValue);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ResourceGroup != "");
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationInHours > 0f);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].MTTRDurationPercent > 0f);
            Assert.IsTrue(model.MeanTimeToRestoreEvents[0].ItemOrder == 1);
        }


    }
}
