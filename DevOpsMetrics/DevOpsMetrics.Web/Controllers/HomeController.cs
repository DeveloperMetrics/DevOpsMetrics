using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevOpsMetrics.Web.Models;
using Microsoft.Extensions.Configuration;
using DevOpsMetrics.Service.Models;

namespace DevOpsMetrics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            int numberOfDeployments = 20;
            int numberOfDays = 7;
            bool showDemoData = false;
            List<PartialViewDeploymentModel> items = new List<PartialViewDeploymentModel>();
            PartialViewDeploymentModel newItem = null;

            //Azure DevOps 1
            string deploymentName = "SamLearnsAzure.CI";
            string patToken = _configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildId = "83"; //"3673"; //SamLearnsAzure.CI
            newItem = await CreateAzureDevOpsBuild(showDemoData, deploymentName, patToken, organization, project, azBranch, buildId, numberOfDeployments, numberOfDays);
            if (newItem != null)
            {
                items.Add(newItem);
            }

            //Azure DevOps 2
            string deploymentName2 = "PartsUnlimited.CI";
            string patToken2 = _configuration["AppSettings:PatToken"];
            string organization2 = "samsmithnz";
            string project2 = "PartsUnlimited";
            string azBranch2 = "refs/heads/master";
            string buildId2 = "75"; //"3673"; //SamLearnsAzure.CI
            newItem = await CreateAzureDevOpsBuild(showDemoData, deploymentName2, patToken2, organization2, project2, azBranch2, buildId2, numberOfDeployments, numberOfDays);
            if (newItem != null)
            {
                items.Add(newItem);
            }

            //GitHub 1
            deploymentName = "SamsFeatureFlags.CI";
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string ghbranch = "master";
            string workflowId = "108084";
            newItem = await CreateGitHubActionsRun(showDemoData, deploymentName, owner, repo, ghbranch, workflowId, numberOfDeployments, numberOfDays);
            if (newItem != null)
            {
                items.Add(newItem);
            }

            //GitHub 2
            deploymentName = "DevOpsMetrics.CI";
            string owner2 = "samsmithnz";
            string repo2 = "DevOpsMetrics";
            string ghbranch2 = "AddingWebsite";
            string workflowId2 = "1162561";
            newItem = await CreateGitHubActionsRun(showDemoData, deploymentName, owner2, repo2, ghbranch2, workflowId2, numberOfDeployments, numberOfDays);
            if (newItem != null)
            {
                items.Add(newItem);
            }

            IndexDeploymentModel indexModel = new IndexDeploymentModel();
            indexModel.Items = items;
            return View(indexModel);
        }

        private async Task<PartialViewDeploymentModel> CreateAzureDevOpsBuild(bool showDemoData, string deploymentName, string patToken, string organization, string project, string azBranch, string buildId, int numberOfDeployments,int numberOfDays)
        {
            ServiceApiClient service = new ServiceApiClient(_configuration);
            List<AzureDevOpsBuild> azList = await service.GetAZDeployments(showDemoData, patToken, organization, project, azBranch, buildId);
            DeploymentFrequencyModel azDeploymentFrequency = await service.GetAZDeploymentFrequency(showDemoData, patToken, organization, project, azBranch, buildId, numberOfDays);

            PartialViewDeploymentModel item = new PartialViewDeploymentModel();
            item.DeploymentName = deploymentName;
            item.AZList = azList;
            item.AZDeploymentFrequency = azDeploymentFrequency;

            //Limit Azure DevOps to latest results
            if (azList.Count >= numberOfDeployments)
            {
                item.AZList = new List<AzureDevOpsBuild>();
                //Only show the last ten builds
                for (int i = azList.Count - numberOfDeployments; i < azList.Count; i++)
                {
                    item.AZList.Add(azList[i]);
                }
            }
            item.AZDeploymentFrequency = azDeploymentFrequency;
            item.AZList = ProcessAzureDevOpsBuilds(item.AZList);

            return item;
        }
        private async Task<PartialViewDeploymentModel> CreateGitHubActionsRun(bool showDemoData, string deploymentName, string owner, string repo, string ghbranch, string workflowId, int numberOfDeployments, int numberOfDays)
        {
            ServiceApiClient service = new ServiceApiClient(_configuration);
            List<GitHubActionsRun> ghList = await service.GetGHDeployments(showDemoData, owner, repo, ghbranch, workflowId);
            DeploymentFrequencyModel ghDeploymentFrequency = await service.GetGHDeploymentFrequency(showDemoData, owner, repo, ghbranch, workflowId, numberOfDays);
           
            PartialViewDeploymentModel item = new PartialViewDeploymentModel();
            item.DeploymentName = deploymentName;
            item.GHDeploymentFrequency = ghDeploymentFrequency;
            item.GHList = ghList;

            //Limit Github to latest 10 results
            if (ghList.Count >= numberOfDeployments)
            {
                item.GHList = new List<GitHubActionsRun>();
                //Only show the last ten builds
                for (int i = ghList.Count - numberOfDeployments; i < ghList.Count; i++)
                {
                    item.GHList.Add(ghList[i]);
                }
            }
            item.GHDeploymentFrequency = ghDeploymentFrequency;
            item.GHList = ProcessGitHubBuilds(item.GHList);

            return item;
        }

        private List<AzureDevOpsBuild> ProcessAzureDevOpsBuilds(List<AzureDevOpsBuild> azList)
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
                item.buildDurationPercent = ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
            }
            return azList;
        }

        private List<GitHubActionsRun> ProcessGitHubBuilds(List<GitHubActionsRun> ghList)
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
                item.buildDurationPercent = ScaleNumberToRange(interiumResult, 0, 100, 20, 100);
            }
            return ghList;
        }

        //We scale the number, so that the lowest number is visible on the charts
        private int ScaleNumberToRange(float number, float currentMin, float currentMax, float targetMin, float targetMax)
        {
            //https://stats.stackexchange.com/questions/281162/scale-a-number-between-a-range/281164
            int result = (int)(((number - currentMin) / (currentMax - currentMin) * (targetMax - targetMin)) + targetMin);
            return result;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
