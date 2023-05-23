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
    public class DeploymentFrequencyDATests : BaseConfiguration
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task AzDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = base.Configuration["AppSettings:AzureDevOpsPatToken"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string organization = "samsmithnz";
            string project = "AzDoDevOpsMetrics";
            string branch = "refs/heads/main";
            string buildName = "azure-pipelines.yml";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            DeploymentFrequencyModel model = await DeploymentFrequencyDA.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, tableStorageConfig, organization, project, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model.DeploymentsPerDayMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription));
            Assert.AreNotEqual("Unknown", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10f, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("per day", model.DeploymentsToDisplayUnit);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
            Assert.IsTrue(model.IsProjectView == false);
            Assert.IsTrue(model.ItemOrder == 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task GHDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = base.Configuration["AppSettings:GitHubClientId"];
            string clientSecret = base.Configuration["AppSettings:GitHubClientSecret"];
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            string owner = "DeveloperMetrics";
            string repo = "DevOpsMetrics";
            string branch = "main";
            string workflowName = "CI/CD";
            string workflowId = "1162561";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            bool useCache = true;

            //Act
            DeploymentFrequencyModel model = await DeploymentFrequencyDA.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, tableStorageConfig, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);

            //Assert
            Assert.IsTrue(model.DeploymentsPerDayMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription));
            Assert.AreNotEqual("Unknown", model.DeploymentsPerDayMetricDescription);
            Assert.AreEqual(10f, model.DeploymentsToDisplayMetric);
            Assert.AreEqual("per day", model.DeploymentsToDisplayUnit);
            Assert.AreEqual(numberOfDays, model.NumberOfDays);
            Assert.IsTrue(model.MaxNumberOfItems > 0);
            Assert.IsTrue(model.TotalItems > 0);
            Assert.IsTrue(model.IsProjectView == false);
            Assert.IsTrue(model.ItemOrder == 0);
        }

    }
}
