using DevOpsMetrics.Service.Models.Common;
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
        private readonly IConfiguration Configuration;
        private readonly ILogger<HomeController> Logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            int maxNumberOfItems = 20;
            int numberOfDays = 60;
            bool getSampleData = false;
            ServiceApiClient serviceAPIClient = new ServiceApiClient(Configuration);
            List<LeadTimeForChangesModel> items = new List<LeadTimeForChangesModel>();

            //Azure DevOps 1
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            LeadTimeForChangesModel newItem1 = await serviceAPIClient.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken, organization, project, repositoryId, azBranch, buildId, numberOfDays, maxNumberOfItems);
            if (newItem1 != null)
            {
                items.Add(newItem1);
            }

            //Azure DevOps 2
            string patToken2 = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization2 = "samsmithnz";
            string project2 = "PartsUnlimited";
            string repositoryId2 = "PartsUnlimited";
            string azBranch2 = "refs/heads/master";
            string buildId2 = "75"; //SamLearnsAzure.CI
            LeadTimeForChangesModel newItem2 = await serviceAPIClient.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken2, organization2, project2, repositoryId2, azBranch2, buildId2, numberOfDays, maxNumberOfItems);
            if (newItem2 != null)
            {
                items.Add(newItem2);
            }

            //GitHub 1
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string ghbranch = "master";
            //string workflowName = "SamsFeatureFlags.CI";
            string workflowId = "108084";
            LeadTimeForChangesModel newItem3 = await serviceAPIClient.GetGitHubLeadTimeForChanges(getSampleData, clientId, clientSecret, owner, repo, ghbranch, workflowId, numberOfDays, maxNumberOfItems);
            if (newItem3 != null)
            {
                items.Add(newItem3);
            }

            //GitHub 2
            string clientId2 = Configuration["AppSettings:GitHubClientId"];
            string clientSecret2 = Configuration["AppSettings:GitHubClientSecret"];
            string owner2 = "samsmithnz";
            string repo2 = "DevOpsMetrics";
            string ghbranch2 = "master";
            //string workflowName2 = "DevOpsMetrics.CI";
            string workflowId2 = "1162561";
            LeadTimeForChangesModel newItem4 = await serviceAPIClient.GetGitHubLeadTimeForChanges(getSampleData, clientId2, clientSecret2, owner2, repo2, ghbranch2, workflowId2, numberOfDays, maxNumberOfItems);
            if (newItem4 != null)
            {
                items.Add(newItem4);
            }

            return View(items);
        }

        public async Task<IActionResult> DeploymentFrequency()
        {
            //TODO: Move variables to a configuration file or database
            int maxNumberOfItems = 20;
            int numberOfDays = 30;
            bool getSampleData = false;
            ServiceApiClient serviceApiClient = new ServiceApiClient(Configuration);
            List<DeploymentFrequencyModel> items = new List<DeploymentFrequencyModel>();

            //Azure DevOps 1
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildName = "SamLearnsAzure.CI";
            string buildId = "83"; //"3673"; //SamLearnsAzure.CI
            DeploymentFrequencyModel newItem1 = await serviceApiClient.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, organization, project, azBranch, buildName, buildId, numberOfDays, maxNumberOfItems);
            if (newItem1 != null)
            {
                items.Add(newItem1);
            }

            //Azure DevOps 2
            string patToken2 = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization2 = "samsmithnz";
            string project2 = "PartsUnlimited";
            string azBranch2 = "refs/heads/master";
            string buildName2 = "PartsUnlimited.CI";
            string buildId2 = "75"; //SamLearnsAzure.CI
            DeploymentFrequencyModel newItem2 = await serviceApiClient.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken2, organization2, project2, azBranch2, buildName2, buildId2, numberOfDays, maxNumberOfItems);
            if (newItem2 != null)
            {
                items.Add(newItem2);
            }

            //GitHub 1
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string ghbranch = "master";
            string workflowName = "SamsFeatureFlags.CI";
            string workflowId = "108084";
            DeploymentFrequencyModel newItem3 = await serviceApiClient.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, owner, repo, ghbranch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
            if (newItem3 != null)
            {
                items.Add(newItem3);
            }

            //GitHub 2
            string clientId2 = Configuration["AppSettings:GitHubClientId"];
            string clientSecret2 = Configuration["AppSettings:GitHubClientSecret"];
            string owner2 = "samsmithnz";
            string repo2 = "DevOpsMetrics";
            string ghbranch2 = "master";
            string workflowName2 = "DevOpsMetrics.CI";
            string workflowId2 = "1162561";
            DeploymentFrequencyModel newItem4 = await serviceApiClient.GetGitHubDeploymentFrequency(getSampleData, clientId2, clientSecret2, owner2, repo2, ghbranch2, workflowName2, workflowId2, numberOfDays, maxNumberOfItems);
            if (newItem4 != null)
            {
                items.Add(newItem4);
            }

            return View(items);
        }

        public async Task<IActionResult> LeadTimeForChanges()
        {
            int maxNumberOfItems = 20;
            int numberOfDays = 60;
            bool getSampleData = false;
            ServiceApiClient serviceAPIClient = new ServiceApiClient(Configuration);
            List<LeadTimeForChangesModel> items = new List<LeadTimeForChangesModel>();

            //Azure DevOps 1
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI
            LeadTimeForChangesModel newItem1 = await serviceAPIClient.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken, organization, project, repositoryId, azBranch, buildId, numberOfDays, maxNumberOfItems);
            if (newItem1 != null)
            {
                items.Add(newItem1);
            }


            //Azure DevOps 2
            string patToken2 = Configuration["AppSettings:AzureDevOpsPatToken"];
            string organization2 = "samsmithnz";
            string project2 = "PartsUnlimited";
            string repositoryId2 = "PartsUnlimited";
            string azBranch2 = "refs/heads/master";
            string buildId2 = "75"; //SamLearnsAzure.CI
            LeadTimeForChangesModel newItem2 = await serviceAPIClient.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken2, organization2, project2, repositoryId2, azBranch2, buildId2, numberOfDays, maxNumberOfItems);
            if (newItem2 != null)
            {
                items.Add(newItem2);
            }

            //GitHub 1
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string ghbranch = "master";
            //string workflowName = "SamsFeatureFlags.CI";
            string workflowId = "108084";
            LeadTimeForChangesModel newItem3 = await serviceAPIClient.GetGitHubLeadTimeForChanges(getSampleData, clientId, clientSecret, owner, repo, ghbranch, workflowId, numberOfDays, maxNumberOfItems);
            if (newItem3 != null)
            {
                items.Add(newItem3);
            }

            //GitHub 2
            string clientId2 = Configuration["AppSettings:GitHubClientId"];
            string clientSecret2 = Configuration["AppSettings:GitHubClientSecret"];
            string owner2 = "samsmithnz";
            string repo2 = "DevOpsMetrics";
            string ghbranch2 = "master";
            //string workflowName2 = "DevOpsMetrics.CI";
            string workflowId2 = "1162561";
            LeadTimeForChangesModel newItem4 = await serviceAPIClient.GetGitHubLeadTimeForChanges(getSampleData, clientId2, clientSecret2, owner2, repo2, ghbranch2, workflowId2, numberOfDays, maxNumberOfItems);
            if (newItem4 != null)
            {
                items.Add(newItem4);
            }

            return View(items);
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
