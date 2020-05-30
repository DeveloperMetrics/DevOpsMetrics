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
        public async Task AzChangeFailureRateDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            bool isAzureDevOps = true;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            ChangeFailureRateModel model = await da.GetChangeFailureRate(getSampleData, tableStorageAuth,
               isAzureDevOps, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.IsAzureDevOps == true);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
        }

        [TestMethod]
        public async Task GHDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            string owner = "samsmithnz";
            string repo = "DevOpsMetrics";
            string branch = "master";
            string workflowName = "DevOpsMetrics CI/CD";
            string workflowId = "1162561";
            bool isAzureDevOps = false;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            ChangeFailureRateModel model = await da.GetChangeFailureRate(getSampleData, tableStorageAuth,
               isAzureDevOps, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model != null);
            Assert.IsTrue(model.IsAzureDevOps == isAzureDevOps);
            Assert.IsTrue(model.DeploymentName != "");
            Assert.IsTrue(model.ChangeFailureRateMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.ChangeFailureRateMetricDescription));
            Assert.AreNotEqual("Elite", model.ChangeFailureRateMetricDescription);
        }

    }
}
