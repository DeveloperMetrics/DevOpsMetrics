using DevOpsMetrics.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class AzureDevOpsLeadTimeForChangesDA
    {
        public async Task<List<LeadTimeForChangesModel>> GetLeadTimesForChanges(string patToken, string organization, string project, string masterBranch, string buildId)
        {
            List<AzureDevOpsBuild> initialBuilds = new List<AzureDevOpsBuild>();
            AzureDevOpsDeploymentFrequencyDA deployments = new AzureDevOpsDeploymentFrequencyDA();
            initialBuilds = await deployments.GetDeployments(patToken, organization, project, masterBranch, buildId);

            //Filter out all branches that aren't a master build
            List<AzureDevOpsBuild> builds = new List<AzureDevOpsBuild>();
            List<string> branches = new List<string>();
            foreach (AzureDevOpsBuild item in initialBuilds)
            {
                if (item.status == "completed" && item.sourceBranch != masterBranch && item.sourceBranch == "refs/pull/445/merge")
                {
                    builds.Add(item);
                    //Load all of the branches
                    if (branches.Contains(item.sourceBranch) == false)
                    {
                        branches.Add(item.sourceBranch);
                    }
                }
            }

            //Process the lead time for changes
            List<LeadTimeForChangesModel> items = new List<LeadTimeForChangesModel>();
            foreach (string branch in branches)
            {
                List<AzureDevOpsBuild> branchBuilds = builds.Where(a => a.sourceBranch == branch).ToList();
                List<AzureDevOpsPRCommit> pullRequestCommits = await GetPullRequestDetails(patToken, organization, project, branch.Replace("refs/pull/", "").Replace("/merge", ""));
                List<Commit> commits = new List<Commit>();
                foreach (AzureDevOpsPRCommit item in pullRequestCommits)
                {
                    commits.Add(new Commit
                    {
                        commitId = item.commitId,
                        name = item.committer.name,
                        date = item.committer.date
                    });
                }
                
                LeadTimeForChangesModel leadTime = new LeadTimeForChangesModel
                {
                    branch = branch,
                    duration = new TimeSpan(),
                    BuildCount = branchBuilds.Count,
                    Commits = commits
                };

                DateTime minTime = DateTime.MaxValue;
                DateTime maxTime = DateTime.MinValue;
                foreach (AzureDevOpsPRCommit pullRequestCommit in pullRequestCommits)
                {
                    if (minTime > pullRequestCommit.committer.date)
                    {
                        minTime = pullRequestCommit.committer.date;
                    }
                    if (maxTime < pullRequestCommit.committer.date)
                    {
                        maxTime = pullRequestCommit.committer.date;
                    }
                }
                foreach (AzureDevOpsBuild branchBuild in branchBuilds)
                {
                    if (minTime > branchBuild.finishTime)
                    {
                        minTime = branchBuild.finishTime;
                    }
                    if (maxTime < branchBuild.finishTime)
                    {
                        maxTime = branchBuild.finishTime;
                    }
                }
                leadTime.duration = (maxTime - minTime);
                items.Add(leadTime);
            }

            return items;
        }

        private async Task<List<AzureDevOpsPRCommit>> GetPullRequestDetails(string patToken, string organization, string project, string pullRequestId)
        {
            string repositoryId = "SamLearnsAzure";
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/git/pull%20request%20commits/get%20pull%20request%20commits?view=azure-devops-rest-5.1
            string url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/commits?api-version=5.1";
            string response = await MessageUtility.SendAzureDevOpsMessage(patToken, url);
            List<AzureDevOpsPRCommit> commits = new List<AzureDevOpsPRCommit>();
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = buildListObject.value;
                commits = JsonConvert.DeserializeObject<List<AzureDevOpsPRCommit>>(value.ToString());
            }
            return commits;
        }

    }
}
