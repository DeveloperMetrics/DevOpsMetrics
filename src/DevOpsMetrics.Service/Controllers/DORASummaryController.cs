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
            string project,
            string repo,
            string branch,
            string workflowName,
            string workflowId,
            string resourceGroup,
            int numberOfDays,
            int maxNumberOfItems,
            ILogger log = null,
            bool useCache = true,
            bool isGitHub = true)
        {
            AzureTableStorageDA azureTableStorageDA = new();
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            string clientId = null;
            string clientSecret = null;
            string patToken = null;
            if (isGitHub == true)
            {  //Get the client id and secret from the settings
                string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
                string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
                clientId = Configuration[clientIdName];
                clientSecret = Configuration[clientSecretName];
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception($"clientId '{clientId}' or clientSecret '{clientSecret}' not found in key vault");
                }
            }
            else
            {
                string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(owner, project, repo);
                patToken = Configuration[patTokenName];
                if (string.IsNullOrEmpty(patToken))
                {
                    throw new Exception($"patToken '{patTokenName}' not found in key vault");
                }
            }

            ProcessingResult result = new();
            try
            {
                //TODO: fix this - should be using a common interface, not this null hack
                string message = "";
                if (isGitHub == true)
                {
                    message = $"Processing GitHub owner {owner}, repo {repo}";
                }
                else
                {
                    message = $"Processing Azure DevOps organization {owner}, project {project}, repo {repo}";
                }
                if (log == null)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    log.LogInformation(message);
                }
                if (isGitHub == true)
                {
                    result.BuildsUpdated = await azureTableStorageDA.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
                        owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
                    result.PRsUpdated = await azureTableStorageDA.UpdateGitHubActionPullRequestsInStorage(clientId, clientSecret, tableStorageConfig,
                        owner, repo, branch, numberOfDays, maxNumberOfItems);
                    message = $"Processed GitHub owner {owner}, repo {repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                }
                else
                {
                    result.BuildsUpdated = await azureTableStorageDA.UpdateAzureDevOpsBuildsInStorage(patToken, tableStorageConfig,
                        owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
                    result.PRsUpdated = await azureTableStorageDA.UpdateAzureDevOpsPullRequestsInStorage(patToken, tableStorageConfig,
                        owner, repo, branch, numberOfDays, maxNumberOfItems);
                    message = $"Processed Azure DevOps organization {owner}, project {project}, repo {repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                }
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
                DeploymentFrequencyModel deploymentFrequencyModel = new();
                LeadTimeForChangesModel leadTimeForChangesModel = new();
                if (isGitHub == true)
                {
                    deploymentFrequencyModel = await DeploymentFrequencyDA.GetGitHubDeploymentFrequency(false, clientId, clientSecret, tableStorageConfig,
                        owner, repo, branch, workflowName, workflowId,
                        numberOfDays, maxNumberOfItems, useCache);

                    leadTimeForChangesModel = await LeadTimeForChangesDA.GetGitHubLeadTimesForChanges(false, clientId, clientSecret, tableStorageConfig,
                       owner, repo, branch, workflowName, workflowId,
                       numberOfDays, maxNumberOfItems, useCache);
                }
                else
                {
                    deploymentFrequencyModel = await DeploymentFrequencyDA.GetAzureDevOpsDeploymentFrequency(false, patToken, tableStorageConfig,
                        owner, project, branch, workflowName,
                        numberOfDays, maxNumberOfItems, useCache);

                    leadTimeForChangesModel = await LeadTimeForChangesDA.GetAzureDevOpsLeadTimesForChanges(false, patToken, tableStorageConfig,
                       owner, project, repo, branch, workflowName,
                       numberOfDays, maxNumberOfItems, useCache);
                }
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
                await AzureTableStorageDA.UpdateDORASummaryItem(tableStorageConfig, owner, project, repo, DORASummary);

                //await settingsController.UpdateGitHubProjectLog(ghSetting.Owner, ghSetting.Repo, result.BuildsUpdated, result.PRsUpdated, "", "", null, null);
                ProjectLog projectLog = null;
                if (isGitHub == true)
                {
                    projectLog = new(
                        PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo),
                        result.BuildsUpdated, result.PRsUpdated,
                        "", "", null, null);
                }
                else
                {
                    projectLog = new(
                        PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(owner, project, repo),
                        result.BuildsUpdated, result.PRsUpdated,
                        "", "", null, null);
                }
                if (projectLog != null)
                {
                    await azureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, projectLog);
                }
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
                ProjectLog projectLog = null;
                if (isGitHub == true)
                {
                    projectLog = new(
                        PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo),
                        result.BuildsUpdated, result.PRsUpdated,
                        owner + "_" + repo + "_" + branch + "_" + workflowName + "_" + workflowId + "_" + numberOfDays + "_" + maxNumberOfItems,
                        owner + "_" + repo + "_" + branch + "_" + numberOfDays + "_" + maxNumberOfItems,
                        ex.ToString(), error);
                }
                else
                {
                    projectLog = new(
                        PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(owner, project, repo),
                        result.BuildsUpdated, result.PRsUpdated,
                        owner + "_" + project + "_" + repo + "_" + branch + "_" + workflowName + "_" + workflowId + "_" + numberOfDays + "_" + maxNumberOfItems,
                        owner + "_" + project + "_" + repo + "_" + branch + "_" + numberOfDays + "_" + maxNumberOfItems,
                        ex.ToString(), error);
                }
                if (projectLog != null)
                {
                    await azureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, projectLog);
                }
            }

            return result;
        }

    }
}