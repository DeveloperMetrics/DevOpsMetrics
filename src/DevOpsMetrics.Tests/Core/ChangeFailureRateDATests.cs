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
    public class ChangeFailureRateDATests : BaseConfiguration
    {

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task AzChangeFailureRateDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.AzureDevOps;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            ChangeFailureRateModel model = await ChangeFailureRateDA.GetChangeFailureRate(getSampleData, tableStorageConfig,
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
        //    tableStorageConfig tableStorageConfig = Common.GenerateTableStorageConfiguration(_configuration);
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
        //    ChangeFailureRateDA da = new();
        //    ChangeFailureRateModel model = await da.GetChangeFailureRate(getSampleData, tableStorageConfig,
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

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task GHChangeFailureRateDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            string owner = "DeveloperMetrics";
            string repo = "DevOpsMetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            DevOpsPlatform targetDevOpsPlatform = DevOpsPlatform.GitHub;
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            ChangeFailureRateModel model = await ChangeFailureRateDA.GetChangeFailureRate(getSampleData, tableStorageConfig,
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
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string buildName = "azure-pipelines.yml";
            int percent = 50;
            int numberOfDays = 1;

            //Act
            bool result = await ChangeFailureRateDA.UpdateChangeFailureRate(tableStorageConfig,
               organization, project, buildName, percent, numberOfDays);

            //Assert
            Assert.IsTrue(result);

        }

    }
}
