using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.GitHub;
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
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
                .AddEnvironmentVariables()
                .Build();

            //Get settings
            ServiceApiClient api = new ServiceApiClient(configuration);
            //List<AzureDevOpsSettings> azSettings = await api.GetAzureDevOpsSettings();
            List<GitHubSettings> ghSettings = await api.GetGitHubSettings();

            //Loop through each setting to update the runs, pull requests and pull request commits
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            int totalResults = 0;
            //foreach (AzureDevOpsSettings item in azSettings)
            //{
            //    log.LogInformation($"Processing Azure DevOps organization {item.Organization}, project {item.Project}");
            //    int buildsUpdated = await api.UpdateAzureDevOpsBuilds(configuration["Appsettings:AzureDevOpsPatToken"], item.Organization, item.Project, item.Branch, item.BuildName, item.BuildId, numberOfDays, maxNumberOfItems);
            //    int prsUpdated = await api.UpdateAzureDevOpsPullRequests(configuration["Appsettings:AzureDevOpsPatToken"], item.Organization, item.Project, item.Repository, numberOfDays, maxNumberOfItems);
            //    log.LogInformation($"Processed Azure DevOps organization {item.Organization}, project {item.Project}. {buildsUpdated} builds and {prsUpdated} prs/commits updated, ");
            //    totalResults += buildsUpdated + prsUpdated;
            //}
            foreach (GitHubSettings item in ghSettings)
            {
                log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}");
                int buildsUpdated = await api.UpdateGitHubActionRuns(configuration["Appsettings:GitHubClientId"], configuration["Appsettings:GitHubClientSecret"], item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {buildsUpdated} builds updated");
                int prsUpdated = await api.UpdateGitHubActionPullRequests(configuration["Appsettings:GitHubClientId"], configuration["Appsettings:GitHubClientSecret"], item.Owner, item.Repo, item.Branch, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {prsUpdated} pull requests updated");
                log.LogInformation($"Processed GitHub owner {item.Owner}, repo {item.Repo}. {buildsUpdated} builds and {prsUpdated} prs/commits updated");
                totalResults += buildsUpdated + prsUpdated;
            }
            log.LogInformation($"C# Timer trigger function complete at: {DateTime.Now} after updating {totalResults} records");
        }

    }
}
