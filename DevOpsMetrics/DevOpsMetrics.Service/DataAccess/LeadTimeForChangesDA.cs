using DevOpsMetrics.Core;
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
        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimesForChanges(bool getSampleData, string patToken, string organization, string project, string repositoryId, string masterBranch, string buildId, int numberOfDays, int maxNumberOfItems)
        {
            LeadTimeForChanges leadTimeForChanges = new LeadTimeForChanges();
            List<PullRequestModel> pullRequests = new List<PullRequestModel>();
            if (getSampleData == false)
            {
                List<AzureDevOpsBuild> initialBuilds = new List<AzureDevOpsBuild>();
                BuildsDA buildsDA = new BuildsDA();
                initialBuilds = await buildsDA.GetAzureDevOpsBuilds(patToken, organization, project, masterBranch, buildId);

                //Process all builds, filtering by master and feature branchs
                List<AzureDevOpsBuild> masterBranchBuilds = new List<AzureDevOpsBuild>();
                List<AzureDevOpsBuild> featureBranchBuilds = new List<AzureDevOpsBuild>();
                List<string> branches = new List<string>();
                foreach (AzureDevOpsBuild item in initialBuilds)
                {
                    if (item.status == "completed")
                    {
                        if (item.sourceBranch == masterBranch)
                        {
                            //Save the master branch
                            masterBranchBuilds.Add(item);
                        }
                        else
                        {
                            //Save the feature branches
                            featureBranchBuilds.Add(item);
                            //Record all unique branches
                            if (branches.Contains(item.sourceBranch) == false)
                            {
                                branches.Add(item.sourceBranch);
                            }
                        }
                    }
                }

                //Process the lead time for changes
                List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>();
                foreach (string branch in branches)
                {
                    List<AzureDevOpsBuild> branchBuilds = featureBranchBuilds.Where(a => a.sourceBranch == branch).ToList();
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
                    PullRequestModel pullRequest = new PullRequestModel
                    {
                        PullRequestId = pullRequestId,
                        Branch = branch,
                        BuildCount = branchBuilds.Count,
                        Commits = commits,
                        StartDateTime = minTime,
                        EndDateTime = maxTime,
                        Url = $"https://dev.azure.com/{organization}/{project}/_git/{repositoryId}/pullrequest/{pullRequestId}"
                    };
                    leadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(minTime, pullRequest.Duration));
                    pullRequests.Add(pullRequest);
                }

                float maxPullRequestDuration = 0f;
                foreach (PullRequestModel item in pullRequests)
                {
                    if (item.Duration.TotalMinutes > maxPullRequestDuration)
                    {
                        maxPullRequestDuration = (float)item.Duration.TotalMinutes;
                    }
                }
                foreach (PullRequestModel item in pullRequests)
                {
                    float interiumResult = (((float)item.Duration.TotalMinutes / maxPullRequestDuration) * 100f);
                    item.DurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                }

                //Filter out builds on the master branch older than the number of days
                masterBranchBuilds = masterBranchBuilds.Where(x => x.queueTime > DateTime.Now.AddDays(-numberOfDays)).ToList();
                double totalHours = 0;
                foreach (AzureDevOpsBuild item in masterBranchBuilds)
                {
                    totalHours += (item.finishTime - item.queueTime).TotalHours;
                }
                float averageBuildHours = 0;
                if (masterBranchBuilds.Count > 0)
                {
                    averageBuildHours = (float)totalHours / (float)masterBranchBuilds.Count;
                }

                //Calculate the lead time for changes value, in hours
                float leadTime = leadTimeForChanges.ProcessLeadTimeForChanges(leadTimeForChangesList, project, numberOfDays);

                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = project,
                    IsAzureDevOps = true,
                    AverageBuildHours = averageBuildHours,
                    AveragePullRequestHours = leadTime,
                    LeadTimeForChangesMetric = leadTime + averageBuildHours,
                    LeadTimeForChangesMetricDescription = leadTimeForChanges.GetLeadTimeForChangesRating(leadTime),
                    PullRequests = pullRequests,
                    NumberOfDays = numberOfDays
                };

                return model;
            }
            else
            {
                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = project,
                    IsAzureDevOps = true,
                    AverageBuildHours = 1f,
                    AveragePullRequestHours = 12f,
                    LeadTimeForChangesMetric = 12f + 1f,
                    LeadTimeForChangesMetricDescription = "Elite",
                    PullRequests = CreatePullRequestsSample(true),
                    NumberOfDays = numberOfDays
                };

                return model;
            }
        }

        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimesForChanges(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string masterBranch, string workflowId, int numberOfDays, int maxNumberOfItems)
        {
            LeadTimeForChanges leadTimeForChanges = new LeadTimeForChanges();
            List<PullRequestModel> pullRequests = new List<PullRequestModel>();
            if (getSampleData == false)
            {
                List<GitHubActionsRun> initialRuns = new List<GitHubActionsRun>();
                BuildsDA buildsDA = new BuildsDA();
                initialRuns = await buildsDA.GetGitHubActionRuns(getSampleData, clientId, clientSecret, owner, repo, masterBranch, workflowId);

                //Process all builds, filtering by master and feature branchs
                List<GitHubActionsRun> masterBranchRuns = new List<GitHubActionsRun>();
                List<GitHubActionsRun> featureBranchRuns = new List<GitHubActionsRun>();
                List<string> branches = new List<string>();
                foreach (GitHubActionsRun item in initialRuns)
                {
                    if (item.status == "completed")
                    {
                        if (item.head_branch == masterBranch)
                        {
                            //Save the master branch
                            //           masterBranchRuns.Add(item);
                        }
                        else
                        {
                            //Save the feature branches
                            featureBranchRuns.Add(item);
                            //Record all unique branches       
                            if (branches.Contains(item.head_branch) == false)
                            {
                                branches.Add(item.head_branch);
                            }
                        }
                    }
                }

                //Process the lead time for changes
                List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>();
                foreach (string branch in branches)
                {
                    List<GitHubActionsRun> branchBuilds = featureBranchRuns.Where(a => a.head_branch == branch).ToList();
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
                            name = item.commit.committer.name,
                            date = item.commit.committer.date
                        });
                    }

                    DateTime minTime = DateTime.MaxValue;
                    DateTime maxTime = DateTime.MinValue;
                    foreach (GitHubPRCommit pullRequestCommit in pullRequestCommits)
                    {
                        if (minTime > pullRequestCommit.commit.committer.date)
                        {
                            minTime = pullRequestCommit.commit.committer.date;
                        }
                        if (maxTime < pullRequestCommit.commit.committer.date)
                        {
                            maxTime = pullRequestCommit.commit.committer.date;
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

                    PullRequestModel pullRequest = new PullRequestModel
                    {
                        PullRequestId = pullRequestId,
                        Branch = branch,
                        BuildCount = branchBuilds.Count,
                        Commits = commits,
                        StartDateTime = minTime,
                        EndDateTime = maxTime,
                        Url = $"https://github.com/{owner}/{repo}/pull/{pullRequestId}"
                    };

                    leadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(minTime, pullRequest.Duration));
                    pullRequests.Add(pullRequest);
                }

                float maxPullRequestDuration = 0f;
                foreach (PullRequestModel item in pullRequests)
                {
                    if (item.Duration.TotalMinutes > maxPullRequestDuration)
                    {
                        maxPullRequestDuration = (float)item.Duration.TotalMinutes;
                    }
                }
                foreach (PullRequestModel item in pullRequests)
                {
                    float interiumResult = (((float)item.Duration.TotalMinutes / maxPullRequestDuration) * 100f);
                    item.DurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                }

                //Filter out builds on the master branch older than the number of days
                masterBranchRuns = masterBranchRuns.Where(x => x.created_at > DateTime.Now.AddDays(-numberOfDays)).ToList();
                double totalHours = 0;
                foreach (GitHubActionsRun item in masterBranchRuns)
                {
                    totalHours += (item.updated_at - item.created_at).TotalHours;
                }
                float averageBuildHours = 0;
                if (masterBranchRuns.Count > 0)
                {
                    averageBuildHours = (float)totalHours / (float)masterBranchRuns.Count;
                }

                //Calculate the lead time for changes value, in hours
                float leadTime = leadTimeForChanges.ProcessLeadTimeForChanges(leadTimeForChangesList, repo, numberOfDays);

                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = repo,
                    IsAzureDevOps = false,
                    AverageBuildHours = averageBuildHours,
                    AveragePullRequestHours=leadTime,
                    LeadTimeForChangesMetric = leadTime + averageBuildHours,
                    LeadTimeForChangesMetricDescription = leadTimeForChanges.GetLeadTimeForChangesRating(leadTime),
                    PullRequests = pullRequests,
                    NumberOfDays = numberOfDays
                };

                return model;
            }
            else
            {
                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = repo,
                    IsAzureDevOps = false,
                    AverageBuildHours = 1f,
                    AveragePullRequestHours = 20.33f,
                    LeadTimeForChangesMetric = 20.33f + 1f,
                    LeadTimeForChangesMetricDescription = "Elite",
                    PullRequests = CreatePullRequestsSample(false),
                    NumberOfDays = numberOfDays
                };

                return model;
            }
        }

        private List<PullRequestModel> CreatePullRequestsSample(bool isAzureDevOps)
        {
            List<PullRequestModel> prs = new List<PullRequestModel>();

            string url = "";
            if (isAzureDevOps)
            {
                url = $"https://dev.azure.com/testOrganization/testProject/_git/testRepo/pullrequest/123";
            }
            else
            {
                url = $"https://github.com/testOwner/testRepo/pull/123";
            }

            prs.Add(
                new PullRequestModel
                {
                    PullRequestId = "123",
                    Branch = "branch1",
                    BuildCount = 1,
                    Commits = CreateCommitsSample(1),
                    DurationPercent = 33,
                    StartDateTime = DateTime.Now.AddDays(-7),
                    EndDateTime = DateTime.Now.AddDays(-7).AddHours(1),
                    Url = url
                });
            prs.Add(
                new PullRequestModel
                {
                    PullRequestId = "124",
                    Branch = "branch2",
                    BuildCount = 3,
                    Commits = CreateCommitsSample(3),
                    DurationPercent = 100,
                    StartDateTime = DateTime.Now.AddDays(-7),
                    EndDateTime = DateTime.Now.AddDays(-5),
                    Url = url
                });
            prs.Add(
                new PullRequestModel
                {
                    PullRequestId = "126",
                    Branch = "branch3",
                    BuildCount = 2,
                    Commits = CreateCommitsSample(2),
                    DurationPercent = 66,
                    StartDateTime = DateTime.Now.AddDays(-7),
                    EndDateTime = DateTime.Now.AddDays(-6),
                    Url = url
                });

            return prs;
        }

        private List<Commit> CreateCommitsSample(int numberOfCommits)
        {
            List<Commit> commits = new List<Commit>();

            if (numberOfCommits > 0)
            {
                commits.Add(
                    new Commit
                    {
                        commitId = "abc",
                        name = "name1",
                        date = DateTime.Now.AddDays(-7)
                    });
            }
            if (numberOfCommits > 1)
            {
                commits.Add(
                new Commit
                {
                    commitId = "def",
                    name = "name2",
                    date = DateTime.Now.AddDays(-6)
                });
            }
            if (numberOfCommits > 2)
            {
                commits.Add(
                new Commit
                {
                    commitId = "ghi",
                    name = "name3",
                    date = DateTime.Now.AddDays(-5)
                });
            }

            return commits;
        }
    }
}
