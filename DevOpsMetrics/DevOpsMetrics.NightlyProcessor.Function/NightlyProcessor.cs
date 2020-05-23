using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.NightlyProcessor.Function
{
    public static class NightlyProcessor
    {
        private const string everyTwoHours = "0 */2 * * *";

        [FunctionName("UpdateStorageTables")]
        public static async Task Run([TimerTrigger(everyTwoHours, RunOnStartup = true)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function started at: {DateTime.Now}");

            //Load settings
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
                .AddEnvironmentVariables()
                .Build();

            //Get settings
            ServiceApiClient api = new ServiceApiClient(configuration);
            List<AzureDevOpsSettings> azSettings = await api.GetAzureDevOpsSettings();
            List<GitHubSettings> ghSettings = await api.GetGitHubSettings();

            log.LogInformation($"Checking AzureStorageAccountAccessKey environment variable: {configuration["AppSettings:AzureStorageAccountAccessKey"]}");


            //Loop through each setting to update the runs, pull requests and pull request commits
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            int totalResults = 0;
            foreach (AzureDevOpsSettings item in azSettings)
            {
                log.LogInformation($"Processing organization {item.Organization}, project {item.Project}");
                totalResults += await api.UpdateAzureDevOpsBuilds(configuration["Appsettings:AzureDevOpsPatToken"], item.Organization, item.Project, item.Branch, item.BuildName, item.BuildId, numberOfDays, maxNumberOfItems);
                totalResults += await api.UpdateAzureDevOpsPullRequests(configuration["Appsettings:AzureDevOpsPatToken"], item.Organization, item.Project, item.Branch, item.BuildName, item.BuildId, numberOfDays, maxNumberOfItems);
            }
            foreach (GitHubSettings item in ghSettings)
            {
                log.LogInformation($"Processing owner {item.Owner}, repo {item.Repo}");
                totalResults += await api.UpdateGitHubActionRuns(configuration["Appsettings:GitHubClientId"], configuration["Appsettings:GitHubClientSecret"], item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId, numberOfDays, maxNumberOfItems);
                totalResults += await api.UpdateGitHubActionPullRequests(configuration["Appsettings:GitHubClientId"], configuration["Appsettings:GitHubClientSecret"], item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId, numberOfDays, maxNumberOfItems);
            }
            log.LogInformation($"C# Timer trigger function complete at: {DateTime.Now} after updating {totalResults} records");
        }

    }
}
