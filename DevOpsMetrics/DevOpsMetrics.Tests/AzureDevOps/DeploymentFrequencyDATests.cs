using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.AzureDevOps
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
        public async Task AzDeploymentsDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI

            //Act
            AzureDevOpsDeploymentFrequencyDA da = new AzureDevOpsDeploymentFrequencyDA();
            List<AzureDevOpsBuild> list = await da.GetDeployments(patToken, organization, project, branch, buildId);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list[0].status != null);
        }

        [TestMethod]
        public async Task AzDeploymentFrequencyDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            int numberOfDays = 7;

            //Act
            AzureDevOpsDeploymentFrequencyDA da = new AzureDevOpsDeploymentFrequencyDA();
            float deploymentFrequencyResult = await da.GetDeploymentFrequency(patToken, organization, project, branch, buildId, numberOfDays);

            //Assert
            Assert.IsTrue(deploymentFrequencyResult > 0);
        }

    }
}
