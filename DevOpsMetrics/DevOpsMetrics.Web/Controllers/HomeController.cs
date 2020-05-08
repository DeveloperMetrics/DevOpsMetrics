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
            bool showDemoData = true;
            ServiceApiClient serviceAPIClient = new ServiceApiClient(_configuration);
            List<LeadTimeForChangesPartialViewModel> items = new List<LeadTimeForChangesPartialViewModel>();
            LeadTimeForChangesPartialViewModel newItem;

            //Azure DevOps 1
            string patToken = _configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildId = "83"; //"3673"; //SamLearnsAzure.CI
            List<LeadTimeForChangesModel> azleadTimes1 = await serviceAPIClient.GetAzureDevOpsLeadTimeForChanges(showDemoData, patToken, organization, project, azBranch, buildId);
            newItem = new LeadTimeForChangesPartialViewModel
            {
                ProjectName = project,
                AzureDevOpsList = azleadTimes1
            };
            items.Add(newItem);

            //GitHub 2
            string owner2 = "samsmithnz";
            string repo2 = "DevOpsMetrics";
            string ghbranch2 = "AddingWebsite";
            string workflowId2 = "1162561";
            List<LeadTimeForChangesModel> ghleadTimes2 = await serviceAPIClient.GetGitHubLeadTimeForChanges(showDemoData, owner2, repo2, ghbranch2, workflowId2);
            newItem = new LeadTimeForChangesPartialViewModel
            {
                ProjectName = project,
                GitHubList = ghleadTimes2
            };
            items.Add(newItem);

            return View(items);
        }

        public async Task<IActionResult> DeploymentFrequency()
        {
            int numberOfDeployments = 20;
            int numberOfDays = 7;
            bool showDemoData = false;
            List<DeploymentPartialViewModel> items = new List<DeploymentPartialViewModel>();
            DeploymentPartialViewModel newItem;

            //Azure DevOps 1
            //TODO: Move variables
            string deploymentName = "SamLearnsAzure.CI";
            string patToken = _configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildId = "83"; //"3673"; //SamLearnsAzure.CI
            newItem = await DeploymentFrequencyService.CreateAzureDevOpsBuild(showDemoData, deploymentName, patToken, organization, project, azBranch, buildId, numberOfDeployments, numberOfDays, _configuration);
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
            newItem = await DeploymentFrequencyService.CreateAzureDevOpsBuild(showDemoData, deploymentName2, patToken2, organization2, project2, azBranch2, buildId2, numberOfDeployments, numberOfDays, _configuration);
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
            newItem = await DeploymentFrequencyService.CreateGitHubActionsRun(showDemoData, deploymentName, owner, repo, ghbranch, workflowId, numberOfDeployments, numberOfDays, _configuration);
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
            newItem = await DeploymentFrequencyService.CreateGitHubActionsRun(showDemoData, deploymentName, owner2, repo2, ghbranch2, workflowId2, numberOfDeployments, numberOfDays, _configuration);
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
