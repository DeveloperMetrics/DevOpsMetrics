using DevOpsMetrics.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class LeadTimeForChangesDA
    {
        public async Task<List<LeadTimeForChangesModel>> GetAzureDevOpsLeadTimesForChanges(string patToken, string organization, string project, string masterBranch, string buildId)
        {
            List<AzureDevOpsBuild> initialBuilds = new List<AzureDevOpsBuild>();
            DeploymentFrequencyDA deployments = new DeploymentFrequencyDA();
            initialBuilds = await deployments.GetAzureDevOpsDeployments(patToken, organization, project, masterBranch, buildId);

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
                List<AzureDevOpsPRCommit> pullRequestCommits = await PullRequestDA.GetAzureDevOpsPullRequestDetails(patToken, organization, project, branch.Replace("refs/pull/", "").Replace("/merge", ""));
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

        public async Task<List<LeadTimeForChangesModel>> GetGitHubLeadTimesForChanges(string owner, string repo, string masterBranch, string workflowId)
        {
            List<GitHubActionsRun> initialRuns = new List<GitHubActionsRun>();
            DeploymentFrequencyDA deployments = new DeploymentFrequencyDA();
            initialRuns = await deployments.GetGitHubDeployments(owner, repo, masterBranch, workflowId);

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
                string pullRequestId = await PullRequestDA.GetGitHubPullRequestByBranchName(owner, repo, branch);
                List<GitHubPRCommit> pullRequestCommits = await PullRequestDA.GetGitHubPullRequestDetails(owner, repo, pullRequestId);
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

                LeadTimeForChangesModel leadTime = new LeadTimeForChangesModel
                {
                    branch = branch,
                    duration = new TimeSpan(),
                    BuildCount = branchBuilds.Count,
                    Commits = commits
                };

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
                leadTime.duration = (maxTime - minTime);
                items.Add(leadTime);
            }

            return items;
        }

    }
}
