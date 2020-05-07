using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class GitHubLeadTimeForChangesDA
    {
        public async Task<List<LeadTimeForChangesModel>> GetLeadTimesForChanges(string owner, string repo, string masterBranch, string workflowId)
        {
            List<GitHubActionsRun> initialRuns = new List<GitHubActionsRun>();
            GitHubDeploymentFrequencyDA deployments = new GitHubDeploymentFrequencyDA();
            initialRuns = await deployments.GetDeployments(owner, repo, masterBranch, workflowId);

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
                string pullRequestId = await GetPullRequestByBranchName(owner, repo, branch);
                List<GitHubPRCommit> pullRequestCommits = await GetPullRequestDetails(owner, repo, pullRequestId);
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

        private async Task<string> GetPullRequestByBranchName(string owner, string repo, string branch)
        {
            //https://developer.github.com/v3/pulls/#list-pull-requests
            //GET /repos/:owner/:repo/pulls
            string url = $"/repos/{owner}/{repo}/pulls?state=all&head={branch}";
            string response = await MessageUtility.SendGitHubMessage(url, "https://api.github.com/");

            List<GitHubPR> prs = new List<GitHubPR>();
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = buildListObject;
                prs = JsonConvert.DeserializeObject<List<GitHubPR>>(value.ToString());
            }
            string prId = "";
            foreach (GitHubPR item in prs)
            {
                if (item.head.@ref == branch)
                {
                    prId = item.number;
                }
            }
            return prId;
        }

        private async Task<List<GitHubPRCommit>> GetPullRequestDetails(string owner, string repo, string pull_number)
        {
            //https://developer.github.com/v3/pulls/#list-commits-on-a-pull-request
            //GET /repos/:owner/:repo/pulls/:pull_number/commits
            string url = $"/repos/{owner}/{repo}/pulls/{pull_number}/commits";
            string response = await MessageUtility.SendGitHubMessage(url, "https://api.github.com/");

            List<GitHubPRCommit> commits = new List<GitHubPRCommit>();
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                Newtonsoft.Json.Linq.JArray value = buildListObject;
                commits = JsonConvert.DeserializeObject<List<GitHubPRCommit>>(value.ToString());
            }
            return commits;
        }

    }
}
