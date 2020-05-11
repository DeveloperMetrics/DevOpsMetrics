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
    public class DeploymentFrequencyDATests
    {
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            Configuration = config.Build();
        }

        [TestMethod]
        public async Task AzDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model.DeploymentsPerDayMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription));
            Assert.AreNotEqual("Unknown", model.DeploymentsPerDayMetricDescription);
        }

        [TestMethod]
        public async Task GHDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            bool getSampleData = true;
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowName = "samsfeatureflags CI/CD";
            string workflowId = "108084";
            int numberOfDays = 30;
            int maxNumberOfItems = 20;

            //Act
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);

            //Assert
            Assert.IsTrue(model.DeploymentsPerDayMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription));
            Assert.AreNotEqual("Unknown", model.DeploymentsPerDayMetricDescription);
        }

    }
}
