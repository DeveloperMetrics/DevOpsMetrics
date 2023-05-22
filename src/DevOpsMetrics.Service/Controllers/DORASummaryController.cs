using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOpsMetrics.Core;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DORASummaryController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DORASummaryController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Get DORA Summary Items
        [HttpGet("GetDORASummaryItems")]
        public List<DORASummaryItem> GetDORASummaryItems(string owner)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            List<DORASummaryItem> model = DORASummaryDA.GetDORASummaryItems(tableStorageConfig, owner);
            return model;
        }

        // Get DORA Summary Item
        [HttpGet("GetDORASummaryItem")]
        public DORASummaryItem GetDORASummaryItem(string owner, string repository)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            DORASummaryItem model = DORASummaryDA.GetDORASummaryItem(tableStorageConfig, owner, repository);
            return model;
        }

        [HttpGet("UpdateDORASummaryItem")]
        public async Task<ProcessingResult> UpdateDORASummaryItem(
          string owner,
          string repo,
          string branch,
          string workflowName,
          string workflowId,
          string resourceGroup,
          int numberOfDays,
          int maxNumberOfItems,
          ILogger log = null,
          bool useCache = true)
        {
            AzureTableStorageDA azureTableStorageDA = new();
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            //Get the client id and secret from the settings
            string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
            string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
            string clientId = Configuration[clientIdName];
            string clientSecret = Configuration[clientSecretName];
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception($"clientId '{clientId}' or clientSecret '{clientSecret}' not found in key vault");
            }

            ProcessingResult result = new();
            try
            {
                //TODO: fix this - should be using a common interface, not this null hack
                string message = $"Processing GitHub owner {owner}, repo {repo}";
                if (log == null)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    log.LogInformation(message);
                }
                result.BuildsUpdated = await azureTableStorageDA.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
                    owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {buildsUpdated} builds updated");
                result.PRsUpdated = await azureTableStorageDA.UpdateGitHubActionPullRequestsInStorage(clientId, clientSecret, tableStorageConfig,
                        owner, repo, branch, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {prsUpdated} pull requests updated");
                message = $"Processed GitHub owner {owner}, repo {repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
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
                //await AzureTableStorageDA.UpdateDORASummaryItem(tableStorageConfig, owner, repo, DORASummary);

                //await settingsController.UpdateGitHubProjectLog(ghSetting.Owner, ghSetting.Repo, result.BuildsUpdated, result.PRsUpdated, "", "", null, null);
                ProjectLog projectLog = new(
                    PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo),
                    result.BuildsUpdated, result.PRsUpdated, "", "", null, null);
                await azureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, projectLog);
            }
            catch (Exception ex)
            {
                string error = $"Exception while processing GitHub owner {owner}, repo {repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                if (log == null)
                {
                    Console.WriteLine(error);
                }
                else
                {
                    log.LogInformation(error);
                }
                //await settingsController.UpdateGitHubProjectLog(ghSetting.Owner, ghSetting.Repo, result.BuildsUpdated, result.PRsUpdated,
                //    ghSetting.Owner + "_" + ghSetting.Repo + "_" + ghSetting.Branch + "_" + ghSetting.WorkflowName + "_" + ghSetting.WorkflowId + "_" + numberOfDays + "_" + maxNumberOfItems,
                //    ghSetting.Owner + "_" + ghSetting.Repo + "_" + ghSetting.Branch + "_" + numberOfDays + "_" + maxNumberOfItems,
                //    ex.ToString(), error);
                ProjectLog projectLog = new(
                    PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo),
                    result.BuildsUpdated, result.PRsUpdated,
                    owner + "_" + repo + "_" + branch + "_" + workflowName + "_" + workflowId + "_" + numberOfDays + "_" + maxNumberOfItems,
                    owner + "_" + repo + "_" + branch + "_" + numberOfDays + "_" + maxNumberOfItems,
                    ex.ToString(), error);
                await azureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, projectLog);
            }

            return result;
        }

    }
}