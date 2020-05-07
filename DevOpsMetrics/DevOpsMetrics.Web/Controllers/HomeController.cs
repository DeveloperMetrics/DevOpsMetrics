using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Web.Models;
using DevOpsMetrics.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
            PartialViewDeploymentModel newItem;

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

            DeploymentViewModel indexModel = new DeploymentViewModel
            {
                Items = items
            };
            return View(indexModel);
        }
 
        public async Task<IActionResult> DeploymentFrequency()
        {
            int numberOfDeployments = 20;
            int numberOfDays = 7;
            bool showDemoData = false;
            List<PartialViewDeploymentModel> items = new List<PartialViewDeploymentModel>();
            PartialViewDeploymentModel newItem;

            //Azure DevOps 1
            //TODO: Move variables
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
            //TODO: Move variables
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
            //TODO: Move variables
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
            //TODO: Move variables
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

            DeploymentViewModel indexModel = new DeploymentViewModel
            {
                Items = items
            };
            return View(indexModel);
        }



        public async Task<PartialViewDeploymentModel> CreateAzureDevOpsBuild(bool showDemoData, string deploymentName, string patToken, string organization, string project, string azBranch, string buildId, int numberOfDeployments, int numberOfDays)
        {
            ServiceApiClient service = new ServiceApiClient(_configuration);
            List<AzureDevOpsBuild> azList = await service.GetAzureDevOpsDeployments(showDemoData, patToken, organization, project, azBranch, buildId);
            DeploymentFrequencyModel azDeploymentFrequency = await service.GetAzureDevOpsDeploymentFrequency(showDemoData, patToken, organization, project, azBranch, buildId, numberOfDays);

            PartialViewDeploymentModel item = new PartialViewDeploymentModel
            {
                DeploymentName = deploymentName,
                AzureDevOpsList = azList,
                AzureDevOpsDeploymentFrequency = azDeploymentFrequency
            };

            //Limit Azure DevOps to latest results
            if (azList.Count >= numberOfDeployments)
            {
                item.AzureDevOpsList = new List<AzureDevOpsBuild>();
                //Only show the last ten builds
                for (int i = azList.Count - numberOfDeployments; i < azList.Count; i++)
                {
                    item.AzureDevOpsList.Add(azList[i]);
                }
            }
            item.AzureDevOpsDeploymentFrequency = azDeploymentFrequency;
            item.AzureDevOpsList = DeploymentFrequencyService.ProcessAzureDevOpsBuilds(item.AzureDevOpsList);

            return item;
        }

        public async Task<PartialViewDeploymentModel> CreateGitHubActionsRun(bool showDemoData, string deploymentName, string owner, string repo, string ghbranch, string workflowId, int numberOfDeployments, int numberOfDays)
        {
            ServiceApiClient service = new ServiceApiClient(_configuration);
            List<GitHubActionsRun> ghList = await service.GetGitHubDeployments(showDemoData, owner, repo, ghbranch, workflowId);
            DeploymentFrequencyModel ghDeploymentFrequency = await service.GetGitHubDeploymentFrequency(showDemoData, owner, repo, ghbranch, workflowId, numberOfDays);

            PartialViewDeploymentModel item = new PartialViewDeploymentModel
            {
                DeploymentName = deploymentName,
                GitHubDeploymentFrequency = ghDeploymentFrequency,
                GitHubList = ghList
            };

            //Limit GitHub to latest 10 results
            if (ghList.Count >= numberOfDeployments)
            {
                item.GitHubList = new List<GitHubActionsRun>();
                //Only show the last ten builds
                for (int i = ghList.Count - numberOfDeployments; i < ghList.Count; i++)
                {
                    item.GitHubList.Add(ghList[i]);
                }
            }
            item.GitHubDeploymentFrequency = ghDeploymentFrequency;
            item.GitHubList = DeploymentFrequencyService.ProcessGitHubRuns(item.GitHubList);

            return item;
        }

        public IActionResult LeadTimeForChanges()
        {
            return View();
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
