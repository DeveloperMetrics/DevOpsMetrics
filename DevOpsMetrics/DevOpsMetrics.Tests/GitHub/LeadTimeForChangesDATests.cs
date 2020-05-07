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
    public class LeadTimeForChangesDATests
    {
        [TestMethod]
        public async Task GHLeadTimeForChangesDAIntegrationTest()
        {
            //Arrange
            string owner = "samsmithnz";
            string repo = "devopsmetrics";
            string masterBranch = "master";
            string workflowId = "1162561";

            //Act
            GitHubLeadTimeForChangesDA da = new GitHubLeadTimeForChangesDA();
            List<LeadTimeForChangesModel> list = await da.GetLeadTimesForChanges(owner, repo, masterBranch, workflowId);

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
