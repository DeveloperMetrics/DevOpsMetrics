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
        public async Task<List<DORASummaryItem>> GetDORASummaryItems(string owner)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            List<DORASummaryItem> model = await DORASummaryDA.GetDORASummaryItems(tableStorageConfig, owner);
            return model;
        }

        // Get DORA Summary Item
        [HttpGet("GetDORASummaryItem")]
        public async Task<DORASummaryItem> GetDORASummaryItem(string owner, string project, string repo)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            DORASummaryItem model = await DORASummaryDA.GetDORASummaryItem(tableStorageConfig, owner, project, repo);
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
            Microsoft.Extensions.Logging.ILogger log = null,
            bool useCache = true,
            bool isGitHub = true,
            bool useParallelProcessing = true)
        {
            //Start timer
            DateTime startTime = DateTime.Now;
            AzureTableStorageDA azureTableStorageDA = new();
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            string clientId = null;
            string clientSecret = null;
            string patToken = null;
            //Instead of throwing exceptions when there are no secrets, add the error message to the overall processing message
            string errorMessage = null;
            if (isGitHub == true)
            {
                //Get the client id and secret from the settings
                string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
                string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
                clientId = Configuration[clientIdName];
                clientSecret = Configuration[clientSecretName];
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    errorMessage = $"clientId '{clientId}' or clientSecret '{clientSecret}' not found in key vault";
                    throw new Exception(errorMessage);
                }
            }
            else
            {
                string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(owner, project, repo);
                patToken = Configuration[patTokenName];
                if (string.IsNullOrEmpty(patToken))
                {
                    errorMessage = $"patToken '{patTokenName}' not found in key vault";
                    throw new Exception(errorMessage);
                }
            }

            ProcessingResult result = new();
            DeploymentFrequencyModel deploymentFrequencyModel = new();
            LeadTimeForChangesModel leadTimeForChangesModel = new();
            MeanTimeToRestoreModel meanTimeToRestoreModel = new();
            ChangeFailureRateModel changeFailureRateModel = new();
            try
            {
                string message = "";
                if (isGitHub == true)
                {
                    message = $"Processing GitHub owner {owner}, repo {repo}";
                }
                else
                {
                    message = $"Processing Azure DevOps organization {owner}, project {project}, repo {repo}";
                }
                //TODO: fix this - should be using a common interface, not this null hack
                if (log == null)
                {
                    Console.WriteLine(errorMessage);
                    Console.WriteLine(message);
                }
                else
                {
                    log.LogInformation(errorMessage);
                    log.LogInformation(message);
                }
                if (useParallelProcessing)
                {
                    //Call the builds and prs in parallel
                    Task<int> buildTask;
                    Task<int> prTask;
                    if (isGitHub == true)
                    {
                        //result.BuildsUpdated = await 
                        buildTask = azureTableStorageDA.UpdateGitHubActionRunsInStorage(clientId, clientSecret, tableStorageConfig,
                           owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
                        //result.PRsUpdated = await 
                        prTask = azureTableStorageDA.UpdateGitHubActionPullRequestsInStorage(clientId, clientSecret, tableStorageConfig,
                           owner, repo, branch, numberOfDays, maxNumberOfItems);
                        message = $"Processed GitHub owner {owner}, repo {repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                    }
                    else
                    {
                        //result.BuildsUpdated = await 
                        buildTask = azureTableStorageDA.UpdateAzureDevOpsBuildsInStorage(patToken, tableStorageConfig,
                            owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
                        //result.PRsUpdated = await
                        prTask = azureTableStorageDA.UpdateAzureDevOpsPullRequestsInStorage(patToken, tableStorageConfig,
                            owner, repo, branch, numberOfDays, maxNumberOfItems);
                        message = $"Processed Azure DevOps organization {owner}, project {project}, repo {repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                    }
                    await Task.WhenAll(buildTask, prTask);
                    result.BuildsUpdated = await buildTask;
                    result.PRsUpdated = await prTask;
                }
                else
                {
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
                if (useParallelProcessing)
                {
                    Task<DeploymentFrequencyModel> deploymentFrequencyTask;
                    Task<LeadTimeForChangesModel> leadTimeForChangesTask;
                    Task<MeanTimeToRestoreModel> meanTimeToRestoreTask;
                    Task<ChangeFailureRateModel> changeFailureRateTask;

                    //Get the deployment frequency and lead time for changes in parallel
                    if (isGitHub == true)
                    {
                        deploymentFrequencyTask = DeploymentFrequencyDA.GetGitHubDeploymentFrequency(false, clientId, clientSecret, tableStorageConfig,
                            owner, repo, branch, workflowName, workflowId,
                            numberOfDays, maxNumberOfItems, useCache);

                        leadTimeForChangesTask = LeadTimeForChangesDA.GetGitHubLeadTimesForChanges(false, clientId, clientSecret, tableStorageConfig,
                           owner, repo, branch, workflowName, workflowId,
                           numberOfDays, maxNumberOfItems, useCache);
                    }
                    else
                    {
                        deploymentFrequencyTask = DeploymentFrequencyDA.GetAzureDevOpsDeploymentFrequency(false, patToken, tableStorageConfig,
                            owner, project, branch, workflowName,
                            numberOfDays, maxNumberOfItems, useCache);

                        leadTimeForChangesTask = LeadTimeForChangesDA.GetAzureDevOpsLeadTimesForChanges(false, patToken, tableStorageConfig,
                           owner, project, repo, branch, workflowName,
                           numberOfDays, maxNumberOfItems, useCache);
                    }
                    //Get the mean time to restore and change failure rate in parallel
                    if (resourceGroup != null)
                    {
                        meanTimeToRestoreTask = MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(false, tableStorageConfig,
                            DevOpsPlatform.GitHub,
                            resourceGroup,
                            numberOfDays, maxNumberOfItems);
                    }
                    else
                    {
                        meanTimeToRestoreTask = null;
                        meanTimeToRestoreModel.MTTRAverageDurationInHours = 0;
                        meanTimeToRestoreModel.MTTRAverageDurationDescription = MeanTimeToRestore.GetMeanTimeToRestoreRating(0);
                    }
                    changeFailureRateTask = ChangeFailureRateDA.GetChangeFailureRate(false, tableStorageConfig,
                       DevOpsPlatform.GitHub,
                       owner, repo, branch, workflowName,
                       numberOfDays, maxNumberOfItems);

                    //Process the tasks in parallel
                    if (meanTimeToRestoreTask != null)
                    {
                        await Task.WhenAll(deploymentFrequencyTask, leadTimeForChangesTask, meanTimeToRestoreTask, changeFailureRateTask);
                    }
                    else
                    {
                        await Task.WhenAll(deploymentFrequencyTask, leadTimeForChangesTask, changeFailureRateTask);
                    }
                    deploymentFrequencyModel = await deploymentFrequencyTask;
                    leadTimeForChangesModel = await leadTimeForChangesTask;
                    if (meanTimeToRestoreTask != null)
                    {
                        meanTimeToRestoreModel = await meanTimeToRestoreTask;
                    }
                    changeFailureRateModel = await changeFailureRateTask;
                }
                else
                {
                    if (isGitHub == true)
                    {
                        //Get the deployment frequency and lead time for changes
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
                    //Get the mean time to restore and change failure rate
                    if (resourceGroup != null)
                    {
                        meanTimeToRestoreModel = await MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(false, tableStorageConfig,
                            DevOpsPlatform.GitHub,
                            resourceGroup,
                            numberOfDays, maxNumberOfItems);
                    }
                    else
                    {
                        meanTimeToRestoreModel.MTTRAverageDurationInHours = 0;
                        meanTimeToRestoreModel.MTTRAverageDurationDescription = MeanTimeToRestore.GetMeanTimeToRestoreRating(0);
                    }
                    changeFailureRateModel = await ChangeFailureRateDA.GetChangeFailureRate(false, tableStorageConfig,
                       DevOpsPlatform.GitHub,
                       owner, repo, branch, workflowName,
                       numberOfDays, maxNumberOfItems);
                }

                //Get the total time since startTime
                string processingLogMessage = $"Processed summary for {owner}, repo {repo} in {(DateTime.Now - startTime).TotalSeconds} seconds";
                if (errorMessage != null)
                {
                    processingLogMessage += $", error: {errorMessage}";
                }

                //Summarize the results into a new object
                DORASummaryItem DORASummary = new()
                {
                    Owner = owner,
                    Project = project,
                    Repo = repo,
                    NumberOfDays = numberOfDays,
                    DeploymentFrequency = deploymentFrequencyModel.DeploymentsPerDayMetric,
                    DeploymentFrequencyBadgeURL = deploymentFrequencyModel.BadgeURL,
                    DeploymentFrequencyBadgeWithMetricURL = deploymentFrequencyModel.BadgeWithMetricURL,
                    LeadTimeForChanges = leadTimeForChangesModel.LeadTimeForChangesMetric,
                    LeadTimeForChangesBadgeURL = leadTimeForChangesModel.BadgeURL,
                    LeadTimeForChangesBadgeWithMetricURL = leadTimeForChangesModel.BadgeWithMetricURL,
                    MeanTimeToRestore = meanTimeToRestoreModel.MTTRAverageDurationInHours,
                    MeanTimeToRestoreBadgeURL = meanTimeToRestoreModel.BadgeURL,
                    MeanTimeToRestoreBadgeWithMetricURL = meanTimeToRestoreModel.BadgeWithMetricURL,
                    ChangeFailureRate = changeFailureRateModel.ChangeFailureRateMetric,
                    ChangeFailureRateBadgeURL = changeFailureRateModel.BadgeURL,
                    ChangeFailureRateBadgeWithMetricURL = changeFailureRateModel.BadgeWithMetricURL,
                    LastUpdatedMessage = processingLogMessage,
                    LastUpdated = DateTime.Now
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
                string error = $"Exception while processing GitHub owner {owner}, repo {repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated " + ex.ToString();
                if (log == null)
                {
                    Console.WriteLine(error);
                }
                else
                {
                    log.LogInformation(error);
                }
                ProjectLog projectLog;
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
                //Update the DORA object with the error message
                try
                {
                    //Get the total time since startTime
                    string processingLogMessage = $"Error processing summary for {owner}, repo {repo} in {(DateTime.Now - startTime).TotalSeconds} seconds";

                    //Summarize the results into a new object
                    DORASummaryItem DORASummary = new()
                    {
                        Owner = owner,
                        Project = project,
                        Repo = repo,
                        NumberOfDays = numberOfDays,
                        DeploymentFrequency = deploymentFrequencyModel.DeploymentsPerDayMetric,
                        DeploymentFrequencyBadgeURL = deploymentFrequencyModel.BadgeURL,
                        DeploymentFrequencyBadgeWithMetricURL = deploymentFrequencyModel.BadgeWithMetricURL,
                        LeadTimeForChanges = leadTimeForChangesModel.LeadTimeForChangesMetric,
                        LeadTimeForChangesBadgeURL = leadTimeForChangesModel.BadgeURL,
                        LeadTimeForChangesBadgeWithMetricURL = leadTimeForChangesModel.BadgeWithMetricURL,
                        MeanTimeToRestore = meanTimeToRestoreModel.MTTRAverageDurationInHours,
                        MeanTimeToRestoreBadgeURL = meanTimeToRestoreModel.BadgeURL,
                        MeanTimeToRestoreBadgeWithMetricURL = meanTimeToRestoreModel.BadgeWithMetricURL,
                        ChangeFailureRate = changeFailureRateModel.ChangeFailureRateMetric,
                        ChangeFailureRateBadgeURL = changeFailureRateModel.BadgeURL,
                        ChangeFailureRateBadgeWithMetricURL = changeFailureRateModel.BadgeWithMetricURL,
                        LastUpdatedMessage = processingLogMessage,
                        LastUpdated = DateTime.Now
                    };
                    //Serialize the summary into an Azure storage table
                    await AzureTableStorageDA.UpdateDORASummaryItem(tableStorageConfig, owner, project, repo, DORASummary);
                }
                catch
                {
                    //Do nothing, we handled the error above
                }

                if (projectLog != null)
                {
                    await azureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, projectLog);
                }
                //throw new Exception(error, ex);
            }

            return result;
        }

    }
}