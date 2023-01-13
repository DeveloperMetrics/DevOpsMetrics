using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service.Controllers;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.Function
{
    public static class Processing
    {

        public async static Task<ProcessingResult> ProcessGitHubItem(GitHubSettings ghSetting,
            string clientId,
            string clientSecret,
            TableStorageConfiguration tableStorageConfig,
            int numberOfDays,
            int maxNumberOfItems,
            BuildsController buildsController,
            PullRequestsController pullRequestsController,
            SettingsController settingsController,
            ILogger log,
            int totalResults)
        {
            ProcessingResult result = new()
            {
                TotalResults = totalResults
            };
            try
            {
                //TODO: fix this - should be using a common interface, not this null hack
                string message = $"Processing GitHub owner {ghSetting.Owner}, repo {ghSetting.Repo}";
                if (log == null)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    log.LogInformation(message);
                }
                result.BuildsUpdated = await buildsController.UpdateGitHubActionRuns(ghSetting.Owner, ghSetting.Repo, ghSetting.Branch, ghSetting.WorkflowName, ghSetting.WorkflowId, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {buildsUpdated} builds updated");
                result.PRsUpdated = await pullRequestsController.UpdateGitHubActionPullRequests(ghSetting.Owner, ghSetting.Repo, ghSetting.Branch, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {prsUpdated} pull requests updated");
                message = $"Processed GitHub owner {ghSetting.Owner}, repo {ghSetting.Repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                if (log == null)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    log.LogInformation(message);
                }
                result.TotalResults += result.BuildsUpdated + result.PRsUpdated;

                //Process summary results for last 90 days, creating badges for the four metrics
                await UpdateSummaryMetrics(clientId, clientSecret, tableStorageConfig,
                    ghSetting.Owner, ghSetting.Repo, ghSetting.Branch,
                    ghSetting.WorkflowName, ghSetting.WorkflowId,
                    ghSetting.ProductionResourceGroup,
                    numberOfDays, maxNumberOfItems);

                await settingsController.UpdateGitHubProjectLog(ghSetting.Owner, ghSetting.Repo, result.BuildsUpdated, result.PRsUpdated, "", "", null, null);
            }
            catch (Exception ex)
            {
                string error = $"Exception while processing GitHub owner {ghSetting.Owner}, repo {ghSetting.Repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                if (log == null)
                {
                    Console.WriteLine(error);
                }
                else
                {
                    log.LogInformation(error);
                }
                await settingsController.UpdateGitHubProjectLog(ghSetting.Owner, ghSetting.Repo, result.BuildsUpdated, result.PRsUpdated,
                    ghSetting.Owner + "_" + ghSetting.Repo + "_" + ghSetting.Branch + "_" + ghSetting.WorkflowName + "_" + ghSetting.WorkflowId + "_" + numberOfDays + "_" + maxNumberOfItems,
                    ghSetting.Owner + "_" + ghSetting.Repo + "_" + ghSetting.Branch + "_" + numberOfDays + "_" + maxNumberOfItems,
                    ex.ToString(), error);
            }

            return result;
        }

        private static async Task<bool> UpdateSummaryMetrics(string clientId, string clientSecret,
            TableStorageConfiguration tableStorageConfig,
            string owner, string repo,
            string branch, string workflowName, string workflowId,
            string resourceGroup,
            int numberOfDays, int maxNumberOfItems,
            bool useCache = true)
        {
            //Get the DORA metrics for the last 90 days
            DeploymentFrequencyModel deploymentFrequencyModel = await DeploymentFrequencyDA.GetGitHubDeploymentFrequency(false, clientId, clientSecret, tableStorageConfig,
                owner, repo, branch, workflowName, workflowId,
                numberOfDays, maxNumberOfItems, useCache);

            LeadTimeForChangesModel leadTimeForChangesModel = await LeadTimeForChangesDA.GetGitHubLeadTimesForChanges(false, clientId, clientSecret, tableStorageConfig,
                owner, repo, branch, workflowName, workflowId,
                numberOfDays, maxNumberOfItems, useCache);

            MeanTimeToRestoreModel meanTimeToRestoreModel = new();
            if (resourceGroup != null)
            {
                meanTimeToRestoreModel = MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(false, tableStorageConfig,
                DevOpsPlatform.GitHub,
                resourceGroup,
                numberOfDays, maxNumberOfItems);
            }
            else
            {
                meanTimeToRestoreModel.MTTRAverageDurationInHours = 0;
                meanTimeToRestoreModel.MTTRAverageDurationDescription = MeanTimeToRestore.GetMeanTimeToRestoreRating(0);
            }

            ChangeFailureRateModel changeFailureRateModel = ChangeFailureRateDA.GetChangeFailureRate(false, tableStorageConfig,
                DevOpsPlatform.GitHub,
                owner, repo, branch, workflowName,
                numberOfDays, maxNumberOfItems);

            //Summarize the results into a new object
            DORASummaryItem DORASummary = new()
            {
                Owner = owner,
                Repo = repo,
                DeploymentFrequencyBadgeURL = deploymentFrequencyModel.BadgeURL,
                DeploymentFrequencyBadgeWithMetricURL = deploymentFrequencyModel.BadgeWithMetricURL,
                LeadTimeForChangesBadgeURL = leadTimeForChangesModel.BadgeURL,
                LeadTimeForChangesBadgeWithMetricURL = leadTimeForChangesModel.BadgeWithMetricURL,
                MeanTimeToRestoreBadgeURL = meanTimeToRestoreModel.BadgeURL,
                MeanTimeToRestoreBadgeWithMetricURL = meanTimeToRestoreModel.BadgeWithMetricURL,
                ChangeFailureRateBadgeURL = changeFailureRateModel.BadgeURL,
                ChangeFailureRateBadgeWithMetricURL = changeFailureRateModel.BadgeWithMetricURL
            };

            //Serialize the summary into an Azure storage table
            await AzureTableStorageDA.UpdateDORASummaryItem(tableStorageConfig, owner, repo, DORASummary);

            return true;
        }
    }
}
