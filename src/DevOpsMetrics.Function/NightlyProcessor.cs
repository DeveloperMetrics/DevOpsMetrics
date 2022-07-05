using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service;
using DevOpsMetrics.Service.Controllers;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.Function
{
    public static class NightlyProcessor
    {
        [FunctionName("UpdateStorageTables")]
        public static async Task Run(
            [TimerTrigger("0 */2 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function UpdateStorageTables started at: {DateTime.Now}");

            //Load settings
            IConfigurationBuilder builder = new ConfigurationBuilder();
            IConfiguration Configuration = builder
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables()
                .Build();

            string keyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string keyVaultId = Configuration["AppSettings:KeyVaultClientId"];
            string keyVaultSecret = Configuration["AppSettings:KeyVaultClientSecret"];
            AzureServiceTokenProvider azureServiceTokenProvider = new();
            KeyVaultClient keyVaultClient = new(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            builder.AddAzureKeyVault(keyVaultURL, keyVaultId, keyVaultSecret);
            Configuration = builder.Build();

            //Get settings
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            AzureTableStorageDA azureTableStorageDA = new();
            BuildsController buildsController = new(Configuration, azureTableStorageDA);
            PullRequestsController pullRequestsController = new(Configuration, azureTableStorageDA);
            SettingsController settingsController = new(Configuration, azureTableStorageDA);
            List<AzureDevOpsSettings> azSettings = settingsController.GetAzureDevOpsSettings();
            List<GitHubSettings> ghSettings = settingsController.GetGitHubSettings();
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

            //Loop through each setting to update the runs, pull requests and pull request commits
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            int totalResults = 0;
            foreach (AzureDevOpsSettings item in azSettings)
            {
                //    (int, string) buildsUpdated = (0, null);
                //    (int, string) prsUpdated = (0, null);
                //    try
                //    {
                log.LogInformation($"Processing Azure DevOps organization {item.Organization}, project {item.Project}");
                //        buildsUpdated = await api.UpdateAzureDevOpsBuilds(item.Organization, item.Project, item.Repository, item.Branch, item.BuildName, item.BuildId, numberOfDays, maxNumberOfItems);
                //        prsUpdated = await api.UpdateAzureDevOpsPullRequests(item.Organization, item.Project, item.Repository, numberOfDays, maxNumberOfItems);
                //        log.LogInformation($"Processed Azure DevOps organization {item.Organization}, project {item.Project}. {buildsUpdated.Item1} builds and {prsUpdated.Item1} prs/commits updated");
                //        totalResults += buildsUpdated.Item1 + prsUpdated.Item1;
                //        await api.UpdateAzureDevOpsProjectLog(item.Organization, item.Project, item.Repository, buildsUpdated.Item1, prsUpdated.Item1, buildsUpdated.Item2, prsUpdated.Item2, null, null);
                //    }
                //    catch (Exception ex)
                //    {
                //        string error = $"Exception while processing Azure DevOps organization {item.Organization}, project {item.Project}. {buildsUpdated.Item1} builds and {prsUpdated.Item1} prs/commits updated";
                //        log.LogInformation(error);
                //        await api.UpdateAzureDevOpsProjectLog(item.Organization, item.Project, item.Repository, buildsUpdated.Item1, prsUpdated.Item1, buildsUpdated.Item2, prsUpdated.Item2, ex.Message, error);
                //    }
            }

            foreach (GitHubSettings ghSetting in ghSettings)
            {
                ProcessingResult ghResult = await Processing.ProcessGitHubItem(ghSetting,
                    clientId, clientSecret, tableStorageConfig,
                    numberOfDays, maxNumberOfItems,
                    buildsController, pullRequestsController, settingsController,
                    log, totalResults);
                totalResults = ghResult.TotalResults;
            }
            log.LogInformation($"C# Timer trigger function complete at: {DateTime.Now} after updating {totalResults} records");
        }

    }
}
