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
        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimesForChanges(bool getSampleData, string patToken, TableStorageAuth tableStorageAuth,
                string organization, string project, string repositoryId, string masterBranch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ListUtility<PullRequestModel> utility = new ListUtility<PullRequestModel>();
            LeadTimeForChanges leadTimeForChanges = new LeadTimeForChanges();
            List<PullRequestModel> pullRequests = new List<PullRequestModel>();
            if (getSampleData == false)
            {
                List<AzureDevOpsBuild> initialBuilds = new List<AzureDevOpsBuild>();
                BuildsDA buildsDA = new BuildsDA();
                initialBuilds = await buildsDA.GetAzureDevOpsBuilds(patToken, tableStorageAuth, organization, project, masterBranch, buildName, buildId, useCache);

                //Process all builds, filtering by master and feature branchs
                List<AzureDevOpsBuild> masterBranchBuilds = new List<AzureDevOpsBuild>();
                List<AzureDevOpsBuild> featureBranchBuilds = new List<AzureDevOpsBuild>();
                List<string> branches = new List<string>();
                foreach (AzureDevOpsBuild item in initialBuilds)
                {
                    if (item.status == "completed" && item.queueTime > DateTime.Now.AddDays(-numberOfDays))
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
                            if (branches.Contains(item.branch) == false)
                            {
                                branches.Add(item.branch);
                            }
                        }
                    }
                }

                //Process the lead time for changes
                List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>();
                foreach (string branch in branches)
                {
                    List<AzureDevOpsBuild> branchBuilds = featureBranchBuilds.Where(a => a.sourceBranch == branch).ToList();
                    PullRequestDA pullRequestDA = new PullRequestDA();
                    AzureDevOpsPR pr = await pullRequestDA.GetAzureDevOpsPullRequest(patToken, tableStorageAuth, organization, project, repositoryId, branch, useCache);
                    if (pr != null)
                    {
                        List<AzureDevOpsPRCommit> pullRequestCommits = await pullRequestDA.GetAzureDevOpsPullRequestCommits(patToken, tableStorageAuth, organization, project, repositoryId, pr.PullRequestId, useCache);
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
                            PullRequestId = pr.PullRequestId,
                            Branch = branch,
                            BuildCount = branchBuilds.Count,
                            Commits = commits,
                            StartDateTime = minTime,
                            EndDateTime = maxTime,
                            Status = pr.status,
                            Url = $"https://dev.azure.com/{organization}/{project}/_git/{repositoryId}/pullrequest/{pr.PullRequestId}"
                        };

                        leadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(minTime, pullRequest.Duration));
                        pullRequests.Add(pullRequest);
                    }
                }

                //Calculate the lead time for changes value, in hours
                float leadTime = leadTimeForChanges.ProcessLeadTimeForChanges(leadTimeForChangesList, project, numberOfDays);

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
                foreach (AzureDevOpsBuild item in masterBranchBuilds)
                {
                    totalHours += (item.finishTime - item.queueTime).TotalHours;
                }
                float averageBuildHours = 0;
                if (masterBranchBuilds.Count > 0)
                {
                    averageBuildHours = (float)totalHours / (float)masterBranchBuilds.Count;
                }

                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = project,
                    TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                    AverageBuildHours = averageBuildHours,
                    AveragePullRequestHours = leadTime,
                    LeadTimeForChangesMetric = leadTime + averageBuildHours,
                    LeadTimeForChangesMetricDescription = leadTimeForChanges.GetLeadTimeForChangesRating(leadTime),
                    PullRequests = uiPullRequests,
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = uiPullRequests.Count,
                    TotalItems = pullRequests.Count
                };

                return model;
            }
            else
            {
                List<PullRequestModel> samplePullRequests = utility.GetLastNItems(CreatePullRequestsSample(DevOpsPlatform.AzureDevOps), maxNumberOfItems);
                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = project,
                    TargetDevOpsPlatform = DevOpsPlatform.AzureDevOps,
                    AverageBuildHours = 1f,
                    AveragePullRequestHours = 12f,
                    LeadTimeForChangesMetric = 12f + 1f,
                    LeadTimeForChangesMetricDescription = "Elite",
                    PullRequests = samplePullRequests,
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = samplePullRequests.Count,
                    TotalItems = samplePullRequests.Count
                };

                return model;
            }
        }

        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimesForChanges(bool getSampleData, string clientId, string clientSecret, TableStorageAuth tableStorageAuth,
                string owner, string repo, string masterBranch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ListUtility<PullRequestModel> utility = new ListUtility<PullRequestModel>();
            LeadTimeForChanges leadTimeForChanges = new LeadTimeForChanges();
            List<PullRequestModel> pullRequests = new List<PullRequestModel>();
            if (getSampleData == false)
            {
                List<GitHubActionsRun> initialRuns = new List<GitHubActionsRun>();
                BuildsDA buildsDA = new BuildsDA();
                initialRuns = await buildsDA.GetGitHubActionRuns(getSampleData, clientId, clientSecret, tableStorageAuth, owner, repo, masterBranch, workflowName, workflowId, useCache);

                //Process all builds, filtering by master and feature branchs
                List<GitHubActionsRun> masterBranchRuns = new List<GitHubActionsRun>();
                List<GitHubActionsRun> featureBranchRuns = new List<GitHubActionsRun>();
                List<string> branches = new List<string>();
                foreach (GitHubActionsRun item in initialRuns)
                {
                    if (item.status == "completed" && item.created_at > DateTime.Now.AddDays(-numberOfDays))
                    {
                        if (item.head_branch == masterBranch)
                        {
                            //Save the master branch
                            masterBranchRuns.Add(item);
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
                    PullRequestDA pullRequestDA = new PullRequestDA();
                    GitHubPR pr = await pullRequestDA.GetGitHubPullRequest(clientId, clientSecret, tableStorageAuth, owner, repo, branch, useCache);
                    if (pr != null)
                    {
                        List<GitHubPRCommit> pullRequestCommits = await pullRequestDA.GetGitHubPullRequestCommits(clientId, clientSecret, tableStorageAuth, owner, repo, pr.number, useCache);
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
                        else
                        {
                            pullRequest.Status = pullRequest.Status;
                        }

                        leadTimeForChangesList.Add(new KeyValuePair<DateTime, TimeSpan>(minTime, pullRequest.Duration));
                        pullRequests.Add(pullRequest);
                    }
                }

                //Calculate the lead time for changes value, in hours
                float leadTime = leadTimeForChanges.ProcessLeadTimeForChanges(leadTimeForChangesList, repo, numberOfDays);

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
                foreach (GitHubActionsRun item in masterBranchRuns)
                {
                    totalHours += (item.updated_at - item.created_at).TotalHours;
                }
                float averageBuildHours = 0;
                if (masterBranchRuns.Count > 0)
                {
                    averageBuildHours = (float)totalHours / (float)masterBranchRuns.Count;
                }

                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = repo,
                    TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                    AverageBuildHours = averageBuildHours,
                    AveragePullRequestHours = leadTime,
                    LeadTimeForChangesMetric = leadTime + averageBuildHours,
                    LeadTimeForChangesMetricDescription = leadTimeForChanges.GetLeadTimeForChangesRating(leadTime),
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
                LeadTimeForChangesModel model = new LeadTimeForChangesModel
                {
                    ProjectName = repo,
                    TargetDevOpsPlatform = DevOpsPlatform.GitHub,
                    AverageBuildHours = 1f,
                    AveragePullRequestHours = 20.33f,
                    LeadTimeForChangesMetric = 20.33f + 1f,
                    LeadTimeForChangesMetricDescription = "Elite",
                    PullRequests = samplePullRequests,
                    NumberOfDays = numberOfDays,
                    MaxNumberOfItems = samplePullRequests.Count,
                    TotalItems = samplePullRequests.Count
                };

                return model;
            }
        }

        private List<PullRequestModel> CreatePullRequestsSample(DevOpsPlatform targetDevOpsPlatform)
        {
            List<PullRequestModel> prs = new List<PullRequestModel>();

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

            PullRequestModel pr1 = new PullRequestModel
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
            PullRequestModel pr2 = new PullRequestModel
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

            PullRequestModel pr3 = new PullRequestModel
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
