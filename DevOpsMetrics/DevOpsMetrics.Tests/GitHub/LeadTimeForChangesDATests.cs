using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
            LeadTimeForChangesDA da = new LeadTimeForChangesDA();
            List<LeadTimeForChangesModel> list = await da.GetGitHubLeadTimesForChanges(owner, repo, masterBranch, workflowId);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list[0].Branch != null);
            Assert.IsTrue(list[0].Duration.TotalSeconds > 0);
            Assert.IsTrue(list[0].BuildCount > 0);
            Assert.IsTrue(list[0].Commits.Count > 0);
        }

    }
}
