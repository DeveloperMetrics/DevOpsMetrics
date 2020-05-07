using DevOpsMetrics.Service.Models;
using System.Collections.Generic;

namespace DevOpsMetrics.Web.Services
{
    public class DeploymentFrequencyService
    {
        public static List<AzureDevOpsBuild> ProcessAzureDevOpsBuilds(List<AzureDevOpsBuild> azList)
        {
            float maxBuildDuration = 0f;
            foreach (AzureDevOpsBuild item in azList)
            {
                if (item.buildDuration > maxBuildDuration)
                {
                    maxBuildDuration = item.buildDuration;
                }
            }
            foreach (AzureDevOpsBuild item in azList)
            {
                float interiumResult = ((item.buildDuration / maxBuildDuration) * 100f);
                item.buildDurationPercent = Utility.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
            }
            return azList;
        }

        public static List<GitHubActionsRun> ProcessGitHubRuns(List<GitHubActionsRun> ghList)
        {
            float maxBuildDuration = 0f;
            foreach (GitHubActionsRun item in ghList)
            {
                if (item.buildDuration > maxBuildDuration)
                {
                    maxBuildDuration = item.buildDuration;
                }
            }
            foreach (GitHubActionsRun item in ghList)
            {
                float interiumResult = ((item.buildDuration / maxBuildDuration) * 100f);
                item.buildDurationPercent = Utility.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
            }
            return ghList;
        }

    }
}
