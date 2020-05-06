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
            //Azure DevOps
            string patToken = _configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildId = "83"; //"3673"; //SamLearnsAzure.CI

            //GitHub
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string ghbranch = "master";
            string workflowId = "108084";
            //string repo = "DevOpsMetrics"; 
            //string ghbranch = "AddingWebsite"; 
            //string workflowId = "1162561";

            int numberOfDays = 7;
            bool getDemoData = false;

            ServiceApiClient service = new ServiceApiClient(_configuration);
            List<AzureDevOpsBuild> azList = await service.GetAZDeployments(getDemoData, patToken, organization, project, azBranch, buildId);
            DeploymentFrequencyModel azDeploymentFrequency = await service.GetAZDeploymentFrequency(getDemoData, patToken, organization, project, azBranch, buildId, numberOfDays);
            List<GitHubActionsRun> ghList = await service.GetGHDeployments(getDemoData, owner, repo, ghbranch, workflowId);
            DeploymentFrequencyModel ghDeploymentFrequency = await service.GetGHDeploymentFrequency(getDemoData, owner, repo, ghbranch, workflowId, numberOfDays);

            IndexDeploymentModel indexModel = new IndexDeploymentModel();

            int numberOfBuilds = 20;
            int numberOfRuns = 20;

            //Limit Azure DevOps to latest 10 results
            if (azList.Count < numberOfBuilds)
            {
                indexModel.AZList = azList;
            }
            else
            {
                indexModel.AZList = new List<AzureDevOpsBuild>();
                //Only show the last ten builds
                for (int i = azList.Count - numberOfBuilds; i < azList.Count; i++)
                {
                    indexModel.AZList.Add(azList[i]);
                }
            }
            indexModel.AZDeploymentFrequency = azDeploymentFrequency;
            indexModel.AZList = ProcessAzureDevOpsBuilds(indexModel.AZList);

            //Limit Github to latest 10 results
            if (ghList.Count < numberOfRuns)
            {
                indexModel.GHList = ghList;
            }
            else
            {
                indexModel.GHList = new List<GitHubActionsRun>();
                //Only show the last ten builds
                for (int i = ghList.Count - numberOfRuns; i < ghList.Count; i++)
                {
                    indexModel.GHList.Add(ghList[i]);
                }
            }
            indexModel.GHDeploymentFrequency = ghDeploymentFrequency;
            indexModel.GHList = ProcessGitHubBuilds(indexModel.GHList);

            return View(indexModel);
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
