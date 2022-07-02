using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service.Controllers;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.Function
{
    public static class Processing
    {

        public async static Task<ProcessingResult> ProcessGitHubItem(GitHubSettings item,
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
                log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}");
                result.BuildsUpdated = await buildsController.UpdateGitHubActionRuns(item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {buildsUpdated} builds updated");
                result.PRsUpdated = await pullRequestsController.UpdateGitHubActionPullRequests(item.Owner, item.Repo, item.Branch, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {prsUpdated} pull requests updated");
                log.LogInformation($"Processed GitHub owner {item.Owner}, repo {item.Repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated");
                result.TotalResults += result.BuildsUpdated + result.PRsUpdated;
                await settingsController.UpdateGitHubProjectLog(item.Owner, item.Repo, result.BuildsUpdated, result.PRsUpdated, "", "", null, null);
            }
            catch (Exception ex)
            {
                string error = $"Exception while processing GitHub owner {item.Owner}, repo {item.Repo}. {result.BuildsUpdated} builds and {result.PRsUpdated} prs/commits updated";
                log.LogInformation(error);
                await settingsController.UpdateGitHubProjectLog(item.Owner, item.Repo, result.BuildsUpdated, result.PRsUpdated,
                    item.Owner + "_" + item.Repo + "_" + item.Branch + "_" + item.WorkflowName + "_" + item.WorkflowId + "_" + numberOfDays + "_" + maxNumberOfItems,
                    item.Owner + "_" + item.Repo + "_" + item.Branch + "_" + numberOfDays + "_" + maxNumberOfItems,
                    ex.ToString(), error);
            }

            return result;
        }
    }
}
