//using DevOpsMetrics.Service.Models;
//using DevOpsMetrics.Web.Models;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace DevOpsMetrics.Web.Services
//{
//    public class LeadTimeForChangesService
//    {
//        public static async Task<DeploymentPartialViewModel> CreateAzureDevOpsLeadTimeForChanges(bool showDemoData, string deploymentName, string patToken, string organization, string project, string azBranch, string buildId, int numberOfDeployments, int numberOfDays, IConfiguration configuration)
//        {
//            ServiceApiClient service = new ServiceApiClient(configuration);
//            List<LeadTimeForChangesModel> azList = await service.GetAzureDevOpsLeadTimeForChanges(showDemoData, patToken, organization, project, azBranch, buildId);

//            //DeploymentPartialViewModel item = new DeploymentPartialViewModel
//            //{
//            //    DeploymentName = deploymentName,
//            //    AzureDevOpsList = azList,
//            //    AzureDevOpsDeploymentFrequency = azDeploymentFrequency
//            //};

//            ////Limit Azure DevOps to latest results
//            //if (azList.Count >= numberOfDeployments)
//            //{
//            //    item.AzureDevOpsList = new List<AzureDevOpsBuild>();
//            //    //Only show the last ten builds
//            //    for (int i = azList.Count - numberOfDeployments; i < azList.Count; i++)
//            //    {
//            //        item.AzureDevOpsList.Add(azList[i]);
//            //    }
//            //}
//            //item.AzureDevOpsDeploymentFrequency = azDeploymentFrequency;
//            //item.AzureDevOpsList = DeploymentFrequencyService.ProcessAzureDevOpsBuilds(item.AzureDevOpsList);

//            return azList;
//        }

//        public static async Task<DeploymentPartialViewModel> CreateGitHubActionsRun(bool showDemoData, string deploymentName, string owner, string repo, string ghbranch, string workflowId, int numberOfDeployments, int numberOfDays, IConfiguration configuration)
//        {
//            ServiceApiClient service = new ServiceApiClient(configuration);
//            List<GitHubActionsRun> ghList = await service.GetGitHubDeployments(showDemoData, owner, repo, ghbranch, workflowId);
//            DeploymentFrequencyModel ghDeploymentFrequency = await service.GetGitHubDeploymentFrequency(showDemoData, owner, repo, ghbranch, workflowId, numberOfDays);

//            DeploymentPartialViewModel item = new DeploymentPartialViewModel
//            {
//                DeploymentName = deploymentName,
//                GitHubDeploymentFrequency = ghDeploymentFrequency,
//                GitHubList = ghList
//            };

//            //Limit GitHub to latest 10 results
//            if (ghList.Count >= numberOfDeployments)
//            {
//                item.GitHubList = new List<GitHubActionsRun>();
//                //Only show the last ten builds
//                for (int i = ghList.Count - numberOfDeployments; i < ghList.Count; i++)
//                {
//                    item.GitHubList.Add(ghList[i]);
//                }
//            }
//            item.GitHubDeploymentFrequency = ghDeploymentFrequency;
//            item.GitHubList = DeploymentFrequencyService.ProcessGitHubRuns(item.GitHubList);

//            return item;
//        }

//        private static List<AzureDevOpsBuild> ProcessAzureDevOpsBuilds(List<AzureDevOpsBuild> azList)
//        {
//            float maxBuildDuration = 0f;
//            foreach (AzureDevOpsBuild item in azList)
//            {
//                if (item.buildDuration > maxBuildDuration)
//                {
//                    maxBuildDuration = item.buildDuration;
//                }
//            }
//            foreach (AzureDevOpsBuild item in azList)
//            {
//                float interiumResult = ((item.buildDuration / maxBuildDuration) * 100f);
//                item.buildDurationPercent = Utility.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
//            }
//            return azList;
//        }

//        private static List<GitHubActionsRun> ProcessGitHubRuns(List<GitHubActionsRun> ghList)
//        {
//            float maxBuildDuration = 0f;
//            foreach (GitHubActionsRun item in ghList)
//            {
//                if (item.buildDuration > maxBuildDuration)
//                {
//                    maxBuildDuration = item.buildDuration;
//                }
//            }
//            foreach (GitHubActionsRun item in ghList)
//            {
//                float interiumResult = ((item.buildDuration / maxBuildDuration) * 100f);
//                item.buildDurationPercent = Utility.ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
//            }
//            return ghList;
//        }

//    }
//}
