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
    public class ChangeFailureRateDATests
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
        public void AzChangeFailureRateDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            ChangeFailureRateModel model = da.GetChangeFailureRate(getSampleData, tableStorageAuth,
               targetDevOpsPlatform, organization, project, branch, buildName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == DevOpsPlatform.AzureDevOps);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.IsTrue(model.ChangeFailureRateBuildList.Count <= 20f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        //[TestMethod]
        //public async Task AzChangeFailureRateDAIntegrationTest2()
        //{
        //    //Arrange
        //    bool getSampleData = false;
        //    TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
        //    string organization = "samsmithnz";
        //    string project = "PartsUnlimited";
        //    string branch = "refs/heads/master";
        //    string buildName = "PartsUnlimited.CI";
        //    string buildId = "75"; //PartsUnlimited.CI
        //    DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
        //    int numberOfDays = 30;
        //    int maxNumberOfItems = 20;
        //    bool useCache = true;

        //    //Act
        //    ChangeFailureRateDA da = new ChangeFailureRateDA();
        //    ChangeFailureRateModel model = await da.GetChangeFailureRate(getSampleData, tableStorageAuth,
        //       targetDevOpsPlatform, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

        //    //Assert
        //    Assert.IsTrue(model != null);
        //    Assert.IsTrue(model.TargetDevOpsPlatform == DevOpsPlatform.AzureDevOps);
        //    Assert.IsTrue(model.DeploymentName != "");
        //    Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
        //    Assert.IsTrue(model.ChangeFailureRateBuildList.Count <= 20f);
        //    Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
        //    Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
        //}

        [TestMethod]
        public void GHChangeFailureRateDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            ChangeFailureRateModel model = da.GetChangeFailureRate(getSampleData, tableStorageAuth,
               targetDevOpsPlatform, owner, repo, branch, workflowName, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.TargetDevOpsPlatform == targetDevOpsPlatform);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.IsTrue(model.ChangeFailureRateBuildList.Count <= 20f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
        }

        [TestMethod]
        public async Task UpdateChangeFailureRateDAIntegrationTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string buildName = "SamLearnsAzure.CI";
            int percent = 50;
            int numberOfDays = 1;

            //Act
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            bool result = await da.UpdateChangeFailureRate(tableStorageAuth,
               organization, project, buildName, percent, numberOfDays);

            //Assert
            Assert.IsTrue(result == true);

        }

    }
}
