using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.GitHub;
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
            List<AzureDevOpsSettings> azSettings = await api.GetAzureDevOpsSettings();
            List<GitHubSettings> ghSettings = await api.GetGitHubSettings();

            //Loop through each setting to update the runs, pull requests and pull request commits
            int numberOfDays = 30;
            int maxNumberOfItems = 20;
            int totalResults = 0;
            foreach (AzureDevOpsSettings item in azSettings)
            {
                int buildsUpdated = 0;
                int prsUpdated = 0;
                try
                {
                    log.LogInformation($"Processing Azure DevOps organization {item.Organization}, project {item.Project}");
                    buildsUpdated = await api.UpdateAzureDevOpsBuilds(item.Organization, item.Project, item.Repository, item.Branch, item.BuildName, item.BuildId, numberOfDays, maxNumberOfItems);
                    prsUpdated = await api.UpdateAzureDevOpsPullRequests(item.Organization, item.Project, item.Repository, numberOfDays, maxNumberOfItems);
                    log.LogInformation($"Processed Azure DevOps organization {item.Organization}, project {item.Project}. {buildsUpdated} builds and {prsUpdated} prs/commits updated");
                    totalResults += buildsUpdated + prsUpdated;
                    await api.UpdateAzureDevOpsProjectLog(item.Organization, item.Project, item.Repository, buildsUpdated, prsUpdated, null, null);
                }
                catch (Exception ex)
                {
                    string error = $"Exception while processing Azure DevOps organization {item.Organization}, project {item.Project}. {buildsUpdated} builds and {prsUpdated} prs/commits updated";
                    log.LogInformation(error);
                    await api.UpdateAzureDevOpsProjectLog(item.Organization, item.Project, item.Repository, buildsUpdated, prsUpdated, ex.Message, error);
                }
            }
            foreach (GitHubSettings item in ghSettings)
            {
                int buildsUpdated = 0;
                int prsUpdated = 0;
                try
                {
                    log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}");
                    buildsUpdated = await api.UpdateGitHubActionRuns( item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId, numberOfDays, maxNumberOfItems);
                    //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {buildsUpdated} builds updated");
                    prsUpdated = await api.UpdateGitHubActionPullRequests( item.Owner, item.Repo, item.Branch, numberOfDays, maxNumberOfItems);
                    //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {prsUpdated} pull requests updated");
                    log.LogInformation($"Processed GitHub owner {item.Owner}, repo {item.Repo}. {buildsUpdated} builds and {prsUpdated} prs/commits updated");
                    totalResults += buildsUpdated + prsUpdated;
                    await api.UpdateGitHubProjectLog(item.Owner, item.Repo, buildsUpdated, prsUpdated, null, null);
                }
                catch (Exception ex)
                {
                    string error = $"Exception while processing GitHub owner {item.Owner}, repo {item.Repo}. {buildsUpdated} builds and {prsUpdated} prs/commits updated";
                    log.LogInformation(error);
                    await api.UpdateGitHubProjectLog(item.Owner, item.Repo, buildsUpdated, prsUpdated, ex.Message, error);
                }

            }
            log.LogInformation($"C# Timer trigger function complete at: {DateTime.Now} after updating {totalResults} records");
        }

    }
}
