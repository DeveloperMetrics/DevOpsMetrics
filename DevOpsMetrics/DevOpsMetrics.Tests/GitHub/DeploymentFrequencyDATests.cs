using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowId = "108084";

            //Act
            GitHubDeploymentFrequencyDA da = new GitHubDeploymentFrequencyDA();
            List<GitHubActionsRun> list = await da.GetDeployments(owner, repo, branch, workflowId);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public async Task GHDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string branch = "master";
            string workflowId = "108084";
            int numberOfDays = 7;

            //Act
            GitHubDeploymentFrequencyDA da = new GitHubDeploymentFrequencyDA();
            float deploymentFrequencyResult = await da.GetDeploymentFrequency(owner, repo, branch, workflowId, numberOfDays);

            //Assert
            Assert.IsTrue(deploymentFrequencyResult > 0);
        }
    }
}
