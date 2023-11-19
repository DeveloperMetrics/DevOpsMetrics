using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Azure.Core;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Core;
using Azure.Identity;

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
            string tenantId = Configuration["AppSettings:TenantId"];
            //AzureServiceTokenProvider azureServiceTokenProvider = new();
            //KeyVaultClient keyVaultClient = new(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //builder.AddAzureKeyVault(keyVaultURL, keyVaultId, keyVaultSecret);

            if (keyVaultURL != null && keyVaultId != null && keyVaultSecret != null && tenantId != null)
            {
                TokenCredential tokenCredential = new ClientSecretCredential(tenantId, keyVaultId, keyVaultSecret);
                builder.AddAzureKeyVault(new(keyVaultURL), tokenCredential);
            }
            else
            {
                throw new System.Exception("Missing configuration for Azure Key Vault");
            }
            Configuration = builder.Build();
            ServiceApiClient serviceApiClient = new(Configuration);

            //Get settings
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            AzureTableStorageDA azureTableStorageDA = new();
            List<AzureDevOpsSettings> azSettings = await serviceApiClient.GetAzureDevOpsSettings();
            List<GitHubSettings> ghSettings = await serviceApiClient.GetGitHubSettings();

            //Loop through each setting to update the runs, pull requests and pull request commits
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            int totalResults = 0;
            foreach (AzureDevOpsSettings azSetting in azSettings)
            {
                log.LogInformation($"Processing Azure DevOps organization {azSetting.Organization}, project {azSetting.Project}");
                ProcessingResult ghResult = await serviceApiClient.UpdateDORASummaryItem(
                    azSetting.Organization, azSetting.Project, azSetting.Repository,
                    azSetting.Branch, azSetting.BuildName, azSetting.BuildId,
                    azSetting.ProductionResourceGroup,
                    numberOfDays, maxNumberOfItems, false);
                totalResults = ghResult.TotalResults;
            }

            foreach (GitHubSettings ghSetting in ghSettings)
            {
                log.LogInformation($"Processing GitHub owner {ghSetting.Owner}, repo {ghSetting.Repo}");
                ProcessingResult ghResult = await serviceApiClient.UpdateDORASummaryItem(
                    ghSetting.Owner, "", ghSetting.Repo, ghSetting.Branch,
                    ghSetting.WorkflowName, ghSetting.WorkflowId,
                    ghSetting.ProductionResourceGroup,
                    numberOfDays, maxNumberOfItems, true);
                totalResults = ghResult.TotalResults;
            }
            log.LogInformation($"C# Timer trigger function complete at: {DateTime.Now} after updating {totalResults} records");
        }

    }
}
