using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.Common;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;

namespace DevOpsMetrics.Core.DataAccess
{
    public class LeadTimeForChangesDA
    {
        public static async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimesForChanges(bool getSampleData, string patToken, TableStorageConfiguration tableStorageConfig,
                string organization, string project, string repository, string mainBranch, string buildName, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ListUtility<PullRequestModel> utility = new();
            LeadTimeForChanges leadTimeForChanges = new();
            List<PullRequestModel> pullRequests = new();
            if (getSampleData == false)
            {
                List<AzureDevOpsBuild> initialBuilds = new();
                BuildsDA buildsDA = new();
                initialBuilds = await BuildsDA.GetAzureDevOpsBuilds(patToken, tableStorageConfig, organization, project, buildName, useCache);

                //Process all builds, filtering by main and feature branchs
                List<AzureDevOpsBuild> mainBranchBuilds = new();
                List<AzureDevOpsBuild> featureBranchBuilds = new();
                List<string> branches = new();
                foreach (AzureDevOpsBuild item in initialBuilds)
                {
                    if (item.status == "completed" && item.queueTime > DateTime.Now.AddDays(-numberOfDays))
                    {
                        if (item.sourceBranch == mainBranch)
                        {
                            //Save the main branch
                            mainBranchBuilds.Add(item);
                        }
                        else
                        {
                            //Save the feature branches
                            featureBranchBuilds.Add(item);
                            //Record all unique branches
                            if (branches.Contains(item.branch) == false)
                            {
                                branches.Add(item.branch);
                            }
                        }
                    }
                }

                //Process the lead time for changes
                List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new();
                foreach (string branch in branches)
                {
                    List<AzureDevOpsBuild> branchBuilds = featureBranchBuilds.Where(a => a.sourceBranch == branch).ToList();
                    PullRequestsDA pullRequestDA = new();
                    AzureDevOpsPR pr = await PullRequestsDA.GetAzureDevOpsPullRequest(patToken, tableStorageConfig, organization, project, repository, branch, useCache);
                    if (pr != null)
                    {
                        List<AzureDevOpsPRCommit> pullRequestCommits = await PullRequestsDA.GetAzureDevOpsPullRequestCommits(patToken, tableStorageConfig, organization, project, repository, pr.PullRequestId, useCache);
                        List<Commit> commits = new();
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
                        PullRequestModel pullRequest = new()
                        {
                            PullRequestId = pr.PullRequestId,
                            Branch = branch,
                            BuildCount = branchBuilds.Count,
                            Commits = commits,
                            StartDateTime = minTime,
                            EndDateTime = maxTime,
                            Status = pr.status,
                            Url = $"https://dev.azure.com/{organization}/{project}/_git/{repository}/pullrequest/{pr.PullRequestId}"
                        };

                        leadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(minTime, pullRequest.Duration));
                        pullRequests.Add(pullRequest);
                    }
                }

                //Calculate the lead time for changes value, in hours
                float leadTime = leadTimeForChanges.ProcessLeadTimeForChanges(leadTimeForChangesList, numberOfDays);

                List<PullRequestModel> uiPullRequests = utility.GetLastNItems(pullRequests, maxNumberOfItems);
                float maxPullRequestDuration = 0f;
                foreach (PullRequestModel item in uiPullRequests)
                {
                    if (item.Duration.TotalMinutes > maxPullRequestDuration)
                    {
                        maxPullRequestDuration = (float)item.Duration.TotalMinutes;
                    }
                }
                foreach (PullRequestModel item in uiPullRequests)
                {
                    float interiumResult = (((float)item.Duration.TotalMinutes / maxPullRequestDuration) * 100f);
                    item.DurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                }
                double totalHours = 0;
                foreach (AzureDevOpsBuild item in mainBranchBuilds)
                {
                    totalHours += (item.finishTime - item.queueTime).TotalHours;
                }
                float averageBuildHours = 0;
                if (mainBranchBuilds.Count > 0)
                {
                    averageBuildHours = (float)totalHours / (float)mainBranchBuilds.Count;
                }

                LeadTimeForChangesModel model = new()
                {
                    ProjectName = project,
                    TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                    AverageBuildHours = averageBuildHours,
                    AveragePullRequestHours = leadTime,
                    LeadTimeForChangesMetric = leadTime + averageBuildHours,
                    LeadTimeForChangesMetricDescription = LeadTimeForChanges.GetLeadTimeForChangesRating(leadTime),
                    PullRequests = uiPullRequests,
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = uiPullRequests.Count,
                    TotalItems = pullRequests.Count
                };

                return model;
            }
            else
            {
                //Get sample data
                List<PullRequestModel> samplePullRequests = utility.GetLastNItems(CreatePullRequestsSample(DevOpsPlatform.AzureDevOps), maxNumberOfItems);
                LeadTimeForChangesModel model = new()
                {
                    ProjectName = project,
                    TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                    AverageBuildHours = 1f,
                    AveragePullRequestHours = 12f,
                    LeadTimeForChangesMetric = 12f + 1f,
                    LeadTimeForChangesMetricDescription = "High",
                    PullRequests = samplePullRequests,
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = samplePullRequests.Count,
                    TotalItems = samplePullRequests.Count
                };

                return model;
            }
        }

        public static async Task<LeadTimeForChangesModel> GetGitHubLeadTimesForChanges(bool getSampleData, string clientId, string clientSecret, TableStorageConfiguration tableStorageConfig,
                string owner, string repo, string mainBranch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ListUtility<PullRequestModel> utility = new();
            LeadTimeForChanges leadTimeForChanges = new();
            List<PullRequestModel> pullRequests = new();
            if (getSampleData == false)
            {
                List<GitHubActionsRun> initialRuns = new();
                BuildsDA buildsDA = new();
                initialRuns = await BuildsDA.GetGitHubActionRuns(clientId, clientSecret, tableStorageConfig, owner, repo, workflowName, workflowId, useCache);

                //Process all builds, filtering by main and feature branchs
                List<GitHubActionsRun> mainBranchRuns = new();
                List<GitHubActionsRun> featureBranchRuns = new();
                List<string> branches = new();
                foreach (GitHubActionsRun item in initialRuns)
                {
                    if (item.status == "completed" && item.created_at > DateTime.Now.AddDays(-numberOfDays))
                    {
                        if (item.head_branch == mainBranch)
                        {
                            //Save the main branch
                            mainBranchRuns.Add(item);
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
                List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new();
                foreach (string branch in branches)
                {
                    List<GitHubActionsRun> branchBuilds = featureBranchRuns.Where(a => a.head_branch == branch).ToList();
                    PullRequestsDA pullRequestDA = new();
                    GitHubPR pr = await PullRequestsDA.GetGitHubPullRequest(clientId, clientSecret, tableStorageConfig, owner, repo, branch, useCache);
                    if (pr != null)
                    {
                        List<GitHubPRCommit> pullRequestCommits = await PullRequestsDA.GetGitHubPullRequestCommits(clientId, clientSecret, tableStorageConfig, owner, repo, pr.number, useCache);
                        List<Commit> commits = new();
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

                        PullRequestModel pullRequest = new()
                        {
                            PullRequestId = pr.number,
                            Branch = branch,
                            BuildCount = branchBuilds.Count,
                            Commits = commits,
                            StartDateTime = minTime,
                            EndDateTime = maxTime,
                            Status = pr.state,
                            Url = $"https://github.com/{owner}/{repo}/pull/{pr.number}"
                        };
                        //Convert the pull request status to the standard UI status
                        if (pullRequest.Status == "closed")
                        {
                            pullRequest.Status = "completed";
                        }
                        else if (pullRequest.Status == "open")
                        {
                            pullRequest.Status = "inProgress";
                        }

                        leadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(minTime, pullRequest.Duration));
                        pullRequests.Add(pullRequest);
                    }
                }

                //Calculate the lead time for changes value, in hours
                float leadTime = leadTimeForChanges.ProcessLeadTimeForChanges(leadTimeForChangesList, numberOfDays);

                List<PullRequestModel> uiPullRequests = utility.GetLastNItems(pullRequests, maxNumberOfItems);
                float maxPullRequestDuration = 0f;
                foreach (PullRequestModel item in uiPullRequests)
                {
                    if (item.Duration.TotalMinutes > maxPullRequestDuration)
                    {
                        maxPullRequestDuration = (float)item.Duration.TotalMinutes;
                    }
                }
                foreach (PullRequestModel item in uiPullRequests)
                {
                    float interiumResult = (((float)item.Duration.TotalMinutes / maxPullRequestDuration) * 100f);
                    item.DurationPercent = Scaling.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
                }
                double totalHours = 0;
                foreach (GitHubActionsRun item in mainBranchRuns)
                {
                    totalHours += (item.updated_at - item.created_at).TotalHours;
                }
                float averageBuildHours = 0;
                if (mainBranchRuns.Count > 0)
                {
                    averageBuildHours = (float)totalHours / (float)mainBranchRuns.Count;
                }

                LeadTimeForChangesModel model = new()
                {
                    ProjectName = repo,
                    TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                    AverageBuildHours = averageBuildHours,
                    AveragePullRequestHours = leadTime,
                    LeadTimeForChangesMetric = leadTime + averageBuildHours,
                    LeadTimeForChangesMetricDescription = LeadTimeForChanges.GetLeadTimeForChangesRating(leadTime + averageBuildHours),
                    PullRequests = utility.GetLastNItems(pullRequests, maxNumberOfItems),
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = uiPullRequests.Count,
                    TotalItems = pullRequests.Count
                };

                return model;
            }
            else
            {
                List<PullRequestModel> samplePullRequests = utility.GetLastNItems(CreatePullRequestsSample(DevOpsPlatform.GitHub), maxNumberOfItems);
                LeadTimeForChangesModel model = new()
                {
                    ProjectName = repo,
                    TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                    AverageBuildHours = 1f,
                    AveragePullRequestHours = 20.33f,
                    LeadTimeForChangesMetric = 20.33f + 1f,
                    LeadTimeForChangesMetricDescription = "High",
                    PullRequests = samplePullRequests,
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = samplePullRequests.Count,
                    TotalItems = samplePullRequests.Count
                };

                return model;
            }
        }

        //Return a sample dataset to help with testing
        private static List<PullRequestModel> CreatePullRequestsSample(DevOpsPlatform targetDevOpsPlatform)
        {
            List<PullRequestModel> prs = new();

            string url;
            if (targetDevOpsPlatform == DevOpsPlatform.AzureDevOps)
            {
                url = $"https://dev.azure.com/testOrganization/testProject/_git/testRepo/pullrequest/123";
            }
            else if (targetDevOpsPlatform == DevOpsPlatform.GitHub)
            {
                url = $"https://github.com/testOwner/testRepo/pull/123";
            }
            else
            {
                url = "";
            }

            PullRequestModel pr1 = new()
            {
                PullRequestId = "123",
                Branch = "branch1",
                BuildCount = 1,
                Commits = CreateCommitsSample(1),
                DurationPercent = 33,
                StartDateTime = DateTime.Now.AddDays(-7),
                EndDateTime = DateTime.Now.AddDays(-7).AddHours(1),
                Url = url
            };
            prs.Add(pr1);
            prs.Add(pr1);
            prs.Add(pr1);
            prs.Add(pr1);
            prs.Add(pr1);
            PullRequestModel pr2 = new()
            {
                PullRequestId = "124",
                Branch = "branch2",
                BuildCount = 3,
                Commits = CreateCommitsSample(3),
                DurationPercent = 100,
                StartDateTime = DateTime.Now.AddDays(-7),
                EndDateTime = DateTime.Now.AddDays(-5),
                Url = url
            };
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);
            prs.Add(pr2);

            PullRequestModel pr3 = new()
            {
                PullRequestId = "126",
                Branch = "branch3",
                BuildCount = 2,
                Commits = CreateCommitsSample(2),
                DurationPercent = 66,
                StartDateTime = DateTime.Now.AddDays(-7),
                EndDateTime = DateTime.Now.AddDays(-6),
                Url = url
            };
            prs.Add(pr3);
            prs.Add(pr3);
            prs.Add(pr3);
            prs.Add(pr3);
            prs.Add(pr3);
            prs.Add(pr3);

            return prs;
        }

        private static List<Commit> CreateCommitsSample(int numberOfCommits)
        {
            List<Commit> commits = new();

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
