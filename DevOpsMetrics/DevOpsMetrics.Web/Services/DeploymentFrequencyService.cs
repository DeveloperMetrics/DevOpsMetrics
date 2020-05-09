using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using DevOpsMetrics.Web.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOpsMetrics.Web.Services
{
    public class DeploymentFrequencyService
    {
        //public static async Task<DeploymentPartialViewModel> CreateAzureDevOpsBuild(bool getSampleData, string deploymentName, string patToken, string organization, string project, string azBranch, string buildId, int numberOfDeployments, int numberOfDays, IConfiguration configuration)
        //{
        //    ServiceApiClient service = new ServiceApiClient(configuration);
        //    List<AzureDevOpsBuild> azList = await service.GetAzureDevOpsDeployments(getSampleData, patToken, organization, project, azBranch, buildId);
        //    DeploymentFrequencyModel azDeploymentFrequency = await service.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, azBranch, buildId, numberOfDays);

        //    DeploymentPartialViewModel item = new DeploymentPartialViewModel
        //    {
        //        DeploymentName = deploymentName,
        //        AzureDevOpsList = azList,
        //        AzureDevOpsDeploymentFrequency = azDeploymentFrequency
        //    };

        //    //Limit Azure DevOps to latest results
        //    if (azList.Count >= numberOfDeployments)
        //    {
        //        item.AzureDevOpsList = new List<AzureDevOpsBuild>();
        //        //Only show the last n builds
        //        for (int i = azList.Count - numberOfDeployments; i < azList.Count; i++)
        //        {
        //            item.AzureDevOpsList.Add(azList[i]);
        //        }
        //    }
        //    item.AzureDevOpsDeploymentFrequency = azDeploymentFrequency;
        //    item.AzureDevOpsList = DeploymentFrequencyService.ProcessAzureDevOpsBuilds(item.AzureDevOpsList);

        //    return item;
        //}

        //public static async Task<DeploymentPartialViewModel> CreateGitHubActionsRun(bool getSampleData, string deploymentName, string owner, string repo, string ghbranch, string workflowName, string workflowId, int numberOfDeployments, int numberOfDays, IConfiguration configuration)
        //{
        //    ServiceApiClient service = new ServiceApiClient(configuration);
        //    List<GitHubActionsRun> ghList = await service.GetGitHubDeployments(getSampleData, owner, repo, ghbranch, workflowName, workflowId);
        //    DeploymentFrequencyModel ghDeploymentFrequency = await service.GetGitHubDeploymentFrequency(getSampleData, owner, repo, ghbranch, workflowName, workflowId, numberOfDays);

        //    DeploymentPartialViewModel item = new DeploymentPartialViewModel
        //    {
        //        DeploymentName = deploymentName,
        //        GitHubDeploymentFrequency = ghDeploymentFrequency,
        //        GitHubList = ghList
        //    };

        //    //Limit GitHub to latest 10 results
        //    if (ghList.Count >= numberOfDeployments)
        //    {
        //        item.GitHubList = new List<GitHubActionsRun>();
        //        //Only show the last ten builds
        //        for (int i = ghList.Count - numberOfDeployments; i < ghList.Count; i++)
        //        {
        //            item.GitHubList.Add(ghList[i]);
        //        }
        //    }
        //    item.GitHubDeploymentFrequency = ghDeploymentFrequency;
        //    item.GitHubList = DeploymentFrequencyService.ProcessGitHubRuns(item.GitHubList);

        //    return item;
        //}

     

    }
}
