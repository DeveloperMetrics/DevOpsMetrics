using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.AzureDevOps
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class LeadTimeForChangesDATests
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
        public async Task AzLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string masterBranch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI

            //Act
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            List<LeadTimeForChangesModel> list = await da.GetAzureDevOpsLeadTimesForChanges(patToken, organization, project, masterBranch, buildId);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list[0].branch != null);
            Assert.IsTrue(list[0].duration.TotalSeconds > 0);
            Assert.IsTrue(list[0].BuildCount > 0);
            Assert.IsTrue(list[0].Commits.Count > 0);
        }

    }
}
