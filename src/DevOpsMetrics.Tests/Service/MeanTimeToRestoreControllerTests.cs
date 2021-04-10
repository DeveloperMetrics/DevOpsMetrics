using System;
using System.Net.Http;
using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void AzureMTTRSampleControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string resourceGroupName = "SamLearnsAzureProd";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 7;
            int maxNumberOfItems = 20;
            MeanTimeToRestoreController controller = new MeanTimeToRestoreController(Configuration);

            //Act
            MeanTimeToRestoreModel model = controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems);

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

        [TestCategory("ControllerTest")]
        [TestMethod]
        public void AzureMTTRsAPIControllerIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string resourceGroupName = "SamLearnsAzureProd";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 60;
            int maxNumberOfItems = 20;
            MeanTimeToRestoreController controller = new MeanTimeToRestoreController(Configuration);

            //Act
            MeanTimeToRestoreModel model = controller.GetAzureMeanTimeToRestore(getSampleData, targetDevOpsPlatform, resourceGroupName, numberOfDays, maxNumberOfItems);

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

        [TestCategory("APITest")]
        [TestMethod]
        public void DeploymentsControllerEliteBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "Elite",
                MTTRAverageDurationInHours = 0.12f
            };

            //Act

            //Assert
            Assert.AreEqual("Elite", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Elite-brightgreen", model.BadgeURL);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service%20(0.12%20hours)-Elite-brightgreen", model.BadgeWithMetricURL);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public void DeploymentsControllerHighBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "High"
            };

            //Act

            //Assert
            Assert.AreEqual("High", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-High-green", model.BadgeURL);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public void DeploymentsControllerMediumBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "Medium"
            };

            //Act

            //Assert
            Assert.AreEqual("Medium", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange", model.BadgeURL);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public void DeploymentsControllerLowBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "Low"
            };

            //Act

            //Assert
            Assert.AreEqual("Low", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-Low-red", model.BadgeURL);
        }

        [TestCategory("APITest")]
        [TestMethod]
        public void DeploymentsControllerNoneBadgeTest()
        {
            //Arrange
            MeanTimeToRestoreModel model = new MeanTimeToRestoreModel
            {
                MTTRAverageDurationDescription = "None"
            };

            //Act

            //Assert
            Assert.AreEqual("None", model.MTTRAverageDurationDescription);
            Assert.AreEqual("https://img.shields.io/badge/Time%20to%20restore%20service-None-lightgrey", model.BadgeURL);
        }


    }
}
