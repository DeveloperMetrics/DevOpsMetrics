using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.Function
{
    public static class Processing
    {

        public async static Task<ProcessingResult> ProcessGitHubItem(GitHubSettings item,
            int numberOfDays,
            int maxNumberOfItems,
            ServiceApiClient api,
            ILogger log, 
            int totalResults)
        {
            ProcessingResult result = new ProcessingResult
            {
                TotalResults = totalResults
            };
            try
            {
                log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}");
                result.BuildsUpdated = await api.UpdateGitHubActionRuns(item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {buildsUpdated} builds updated");
                result.PRsUpdated = await api.UpdateGitHubActionPullRequests(item.Owner, item.Repo, item.Branch, numberOfDays, maxNumberOfItems);
                //log.LogInformation($"Processing GitHub owner {item.Owner}, repo {item.Repo}: {prsUpdated} pull requests updated");
                log.LogInformation($"Processed GitHub owner {item.Owner}, repo {item.Repo}. {result.BuildsUpdated.Item1} builds and {result.PRsUpdated.Item1} prs/commits updated");
                result.TotalResults += result.BuildsUpdated.Item1 + result.PRsUpdated.Item1;
                await api.UpdateGitHubProjectLog(item.Owner, item.Repo, result.BuildsUpdated.Item1, result.PRsUpdated.Item1, result.BuildsUpdated.Item2, result.PRsUpdated.Item2, null, null);
            }
            catch (Exception ex)
            {
                string error = $"Exception while processing GitHub owner {item.Owner}, repo {item.Repo}. {result.BuildsUpdated.Item1} builds and {result.PRsUpdated.Item1} prs/commits updated";
                log.LogInformation(error);
                await api.UpdateGitHubProjectLog(item.Owner, item.Repo, result.BuildsUpdated.Item1, result.PRsUpdated.Item1, result.BuildsUpdated.Item2, result.PRsUpdated.Item2, ex.Message, error);
            }

            return result;
        }
    }
}
