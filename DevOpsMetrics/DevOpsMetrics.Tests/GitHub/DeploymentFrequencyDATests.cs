using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.GitHub
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class DeploymentFrequencyDATests
    {
        [TestMethod]
        public async Task GHDeploymentsDAIntegrationTest()
        {
            //Arrange
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string ghbranch = "master";
            string workflowId = "108084";

            //Act
            BuildsDA da = new BuildsDA();
            List<GitHubActionsRun> list = await da.GetGitHubActionRuns(clientId, clientSecret, owner, repo, ghbranch, workflowId);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list[0].status != null);
            Assert.IsTrue(list[0].buildDuration >= 0f);
            Assert.IsTrue(list.Count > 1);
            Assert.IsTrue(list[0].created_at < list[1].created_at);
        }

        [TestMethod]
        public async Task GHDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            string clientId = "";
            string clientSecret = "";
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowId = "108084";
            int numberOfDays = 7;

            //Act
            DeploymentFrequencyDA da = new DeploymentFrequencyDA();
            DeploymentFrequencyModel model = await da.GetGitHubDeploymentFrequency(clientId, clientSecret, owner, repo, branch, workflowId, numberOfDays);

            //Assert
            Assert.IsTrue(model.DeploymentsPerDayMetric > 0f);
            Assert.AreEqual(false, string.IsNullOrEmpty(model.DeploymentsPerDayMetricDescription));
            Assert.AreNotEqual("Unknown", model.DeploymentsPerDayMetricDescription);
        }
    }
}
