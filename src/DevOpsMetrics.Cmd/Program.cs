using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service;
using DevOpsMetrics.Service.Controllers;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.Cmd
{
    internal class Program
    {
        private static string ProjectId = "DeveloperMetrics_DevOpsMetrics";

        static async Task Main()
        {
            DateTime startTime = DateTime.Now;
            Console.WriteLine($"C# Timer trigger function UpdateStorageTables started at: {startTime}");

            //Load settings
            IConfigurationBuilder? builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false)
                 .AddUserSecrets<Program>(true);
            IConfigurationRoot Configuration = builder.Build();
            ILogger log = new Logger<Program>(new LoggerFactory());

            string keyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string keyVaultId = Configuration["AppSettings:KeyVaultClientId"];
            string keyVaultSecret = Configuration["AppSettings:KeyVaultClientSecret"];
            AzureServiceTokenProvider azureServiceTokenProvider = new();
            KeyVaultClient keyVaultClient = new(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            builder.AddAzureKeyVault(keyVaultURL, keyVaultId, keyVaultSecret);
            Configuration = builder.Build();
            ServiceApiClient serviceApiClient = new(Configuration);

            //Get settings
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            AzureTableStorageDA azureTableStorageDA = new();
            SettingsController settingsController = new(Configuration, new AzureTableStorageDA());
            DORASummaryController doraSummaryController = new(Configuration);
            List<AzureDevOpsSettings> azSettings = await settingsController.GetAzureDevOpsSettings();
            List<GitHubSettings> ghSettings = await settingsController.GetGitHubSettings();
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

            //Loop through each setting to update the runs, pull requests and pull request commits
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            int totalResults = 0;
            foreach (AzureDevOpsSettings azSetting in azSettings)
            {
                if (ProjectId == azSetting.RowKey)
                {
                    log.LogInformation($"Processing Azure DevOps organization {azSetting.Organization}, project {azSetting.Project}");
                    ProcessingResult ghResult = await doraSummaryController.UpdateDORASummaryItem(
                        azSetting.Organization, azSetting.Project, azSetting.Repository,
                        azSetting.Branch, azSetting.BuildName, azSetting.BuildId,
                        azSetting.ProductionResourceGroup,
                        numberOfDays, maxNumberOfItems,
                        null, true, false);
                    totalResults = ghResult.TotalResults;
                }
            }

            foreach (GitHubSettings ghSetting in ghSettings)
            {
                if (ProjectId == ghSetting.RowKey)
                {
                    log.LogInformation($"Processing GitHub owner {ghSetting.Owner}, repo {ghSetting.Repo}");
                    ProcessingResult ghResult = await doraSummaryController.UpdateDORASummaryItem(
                        ghSetting.Owner, "", ghSetting.Repo, ghSetting.Branch,
                        ghSetting.WorkflowName, ghSetting.WorkflowId,
                        ghSetting.ProductionResourceGroup,
                        numberOfDays, maxNumberOfItems,
                        null, true, true, true);
                    totalResults = ghResult.TotalResults;
                }
            }
            DateTime endTime = DateTime.Now;
            Console.WriteLine($"C# Timer trigger function complete at: {endTime} after updating {totalResults} records");
            Console.WriteLine($"Processing finished in {(endTime - startTime).TotalSeconds}");
        }
    }
}