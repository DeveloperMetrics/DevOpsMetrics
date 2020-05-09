using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class LeadTimeForChangesDA
    {
        public async Task<List<LeadTimeForChangesModel>> GetAzureDevOpsLeadTimesForChanges(bool getSampleData, string patToken, string organization, string project, string repositoryId, string masterBranch, string buildId)
        {
            List<LeadTimeForChangesModel> items = new List<LeadTimeForChangesModel>();
            if (getSampleData == false)
            {
                List<AzureDevOpsBuild> initialBuilds = new List<AzureDevOpsBuild>();
                BuildsDA buildsDA = new BuildsDA();
                initialBuilds = await buildsDA.GetAzureDevOpsBuilds(patToken, organization, project, masterBranch, buildId);

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
                foreach (string branch in branches)
                {
                    List<AzureDevOpsBuild> branchBuilds = builds.Where(a => a.sourceBranch == branch).ToList();
                    string pullRequestId = branch.Replace("refs/pull/", "").Replace("/merge", "");
                    PullRequestDA pullRequestDA = new PullRequestDA();
                    List<AzureDevOpsPRCommit> pullRequestCommits = await pullRequestDA.GetAzureDevOpsPullRequestCommits(patToken, organization, project, repositoryId, pullRequestId);
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
                    LeadTimeForChangesModel leadTime = new LeadTimeForChangesModel
                    {
                        PullRequestId = pullRequestId,
                        Branch = branch,
                        BuildCount = branchBuilds.Count,
                        Commits = commits,
                        StartDateTime = minTime,
                        EndDateTime = maxTime
                    };

                    items.Add(leadTime);
                }
            }
            else
            {

            }

            return items;
        }

        public async Task<List<LeadTimeForChangesModel>> GetGitHubLeadTimesForChanges(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string masterBranch, string workflowId)
        {
            List<GitHubActionsRun> initialRuns = new List<GitHubActionsRun>();
            BuildsDA buildsDA = new BuildsDA();
            initialRuns = await buildsDA.GetGitHubActionRuns(getSampleData, clientId, clientSecret, owner, repo, masterBranch, workflowId);

            //Filter out all branches that aren't a master build
            List<GitHubActionsRun> runs = new List<GitHubActionsRun>();
            List<string> branches = new List<string>();
            foreach (GitHubActionsRun item in initialRuns)
            {
                if (item.status == "completed" && item.head_branch != masterBranch)//&& item.head_branch == "refs/pull/445/merge")
                {
                    runs.Add(item);
                    //Load all of the branches
                    if (branches.Contains(item.head_branch) == false)
                    {
                        branches.Add(item.head_branch);
                    }
                }
            }

            //Process the lead time for changes
            List<LeadTimeForChangesModel> items = new List<LeadTimeForChangesModel>();
            foreach (string branch in branches)
            {
                List<GitHubActionsRun> branchBuilds = runs.Where(a => a.head_branch == branch).ToList();
                //This is messy. In Azure DevOps we could get the build trigger/pull request id. In GitHub we cannot. 
                //Instead we get the pull request id by searching pull requests by branch
                PullRequestDA pullRequestDA = new PullRequestDA();
                string pullRequestId = await pullRequestDA.GetGitHubPullRequestIdByBranchName(clientId, clientSecret, owner, repo, branch);
                List<GitHubPRCommit> pullRequestCommits = await pullRequestDA.GetGitHubPullRequestCommits(clientId, clientSecret, owner, repo, pullRequestId);
                List<Commit> commits = new List<Commit>();
                foreach (GitHubPRCommit item in pullRequestCommits)
                {
                    commits.Add(new Commit
                    {
                        commitId = item.sha,
                        name = item.committer.name,
                        date = item.committer.date
                    });
                }

                DateTime minTime = DateTime.MaxValue;
                DateTime maxTime = DateTime.MinValue;
                foreach (GitHubPRCommit pullRequestCommit in pullRequestCommits)
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
                foreach (GitHubActionsRun branchBuild in branchBuilds)
                {
                    if (minTime > branchBuild.updated_at)
                    {
                        minTime = branchBuild.updated_at;
                    }
                    if (maxTime < branchBuild.updated_at)
                    {
                        maxTime = branchBuild.updated_at;
                    }
                }

                LeadTimeForChangesModel leadTime = new LeadTimeForChangesModel
                {
                    PullRequestId = pullRequestId,
                    Branch = branch,
                    BuildCount = branchBuilds.Count,
                    Commits = commits,
                    StartDateTime = minTime,
                    EndDateTime = maxTime
                };

                items.Add(leadTime);
            }

            return items;
        }

    }
}
