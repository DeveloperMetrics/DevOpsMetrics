using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using DevOpsMetrics.Web.Models;
using DevOpsMetrics.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration Configuration;

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {

            //Get a list of settings
            ServiceApiClient serviceApiClient = new ServiceApiClient(Configuration);
            List<AzureDevOpsSettings> azureDevOpsSettings = await serviceApiClient.GetAzureDevOpsSettings();
            List<GitHubSettings> githubSettings = await serviceApiClient.GetGitHubSettings();

            //Return the resultant list
            (List<AzureDevOpsSettings>, List<GitHubSettings>) result = (azureDevOpsSettings, githubSettings);
            return View(result);
        }

        public async Task<IActionResult> Project(string rowKey)
        {
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            int maxNumberOfItems = 20;
            int numberOfDays = 30;
            bool getSampleData = false;
            bool useCache = true;
            AzureDevOpsSettings azureDevOpsSetting = null;
            GitHubSettings githubSetting = null;
            ProjectViewModel model = new ProjectViewModel();

            //Find the right project to load
            ServiceApiClient serviceApiClient = new ServiceApiClient(Configuration);
            List<AzureDevOpsSettings> azureDevOpsSettings = await serviceApiClient.GetAzureDevOpsSettings();
            List<GitHubSettings> githubSettings = await serviceApiClient.GetGitHubSettings();

            //Get Azure DevOps project details
            foreach (AzureDevOpsSettings item in azureDevOpsSettings)
            {
                if (item.RowKey == rowKey)
                {
                    azureDevOpsSetting = item;

                    DeploymentFrequencyModel deploymentFrequencyModel = await serviceApiClient.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken,
                        item.Organization, item.Project, item.Branch, item.BuildName, item.BuildId,
                        numberOfDays, maxNumberOfItems, useCache);
                    LeadTimeForChangesModel leadTimeForChangesModel = await serviceApiClient.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken,
                        item.Organization, item.Project, item.Repository, item.Branch, item.BuildName, item.BuildId,
                        numberOfDays, maxNumberOfItems, useCache);
                    MeanTimeToRestoreModel meanTimeToRestoreModel = await serviceApiClient.GetAzureMeanTimeToRestore(getSampleData,
                        item.ProductionResourceGroup,DevOpsPlatform.AzureDevOps , numberOfDays, maxNumberOfItems, useCache);
                    ChangeFailureRateModel changeFailureRateModel = await serviceApiClient.GetChangeFailureRate(getSampleData,
                        DevOpsPlatform.AzureDevOps, item.Organization, item.Project, item.Branch, item.BuildName, item.BuildId,
                        numberOfDays, maxNumberOfItems, useCache);
                    model = new ProjectViewModel
                    {
                        ProjectName = item.Project,
                        DeploymentFrequency = deploymentFrequencyModel,
                        LeadTimeForChanges = leadTimeForChangesModel,
                        MeanTimeToRestore = meanTimeToRestoreModel,
                        ChangeFailureRate = changeFailureRateModel
                    };
                }
            }
            //Get GitHub project details
            foreach (GitHubSettings item in githubSettings)
            {
                if (item.RowKey == rowKey)
                {
                    githubSetting = item;

                    DeploymentFrequencyModel deploymentFrequencyModel = await serviceApiClient.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret,
                        item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId,
                        numberOfDays, maxNumberOfItems, useCache);
                    LeadTimeForChangesModel leadTimeForChangesModel = await serviceApiClient.GetGitHubLeadTimeForChanges(getSampleData, clientId, clientSecret,
                        item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId,
                        numberOfDays, maxNumberOfItems, useCache);
                    MeanTimeToRestoreModel meanTimeToRestoreModel = await serviceApiClient.GetAzureMeanTimeToRestore(getSampleData,
                        item.ProductionResourceGroup, DevOpsPlatform.GitHub, numberOfDays, maxNumberOfItems, useCache);
                    ChangeFailureRateModel changeFailureRateModel = await serviceApiClient.GetChangeFailureRate(getSampleData,
                        DevOpsPlatform.GitHub, item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId,
                        numberOfDays, maxNumberOfItems, useCache);
                    model = new ProjectViewModel
                    {
                        ProjectName = item.Repo,
                        DeploymentFrequency = deploymentFrequencyModel,
                        LeadTimeForChanges = leadTimeForChangesModel,
                        MeanTimeToRestore = meanTimeToRestoreModel,
                        ChangeFailureRate = changeFailureRateModel
                    };
                }
            }

            return View(model);
        }

        public async Task<IActionResult> DeploymentFrequency()
        {
            //TODO: Move variables to a configuration file or database
            int maxNumberOfItems = 20;
            int numberOfDays = 30;
            bool getSampleData = false;
            bool useCache = true;
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];

            ServiceApiClient serviceApiClient = new ServiceApiClient(Configuration);
            List<DeploymentFrequencyModel> items = new List<DeploymentFrequencyModel>();

            //Get a list of settings
            List<AzureDevOpsSettings> azureDevOpsSettings = await serviceApiClient.GetAzureDevOpsSettings();
            List<GitHubSettings> githubSettings = await serviceApiClient.GetGitHubSettings();

            //Create deployment frequency models from each setting object
            foreach (AzureDevOpsSettings item in azureDevOpsSettings)
            {
                DeploymentFrequencyModel newDeploymentFrequencyModel = await serviceApiClient.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken,
                        item.Organization, item.Project, item.Branch, item.BuildName, item.BuildId,
                        numberOfDays, maxNumberOfItems, useCache);
                newDeploymentFrequencyModel.ItemOrder = item.ItemOrder;
                if (newDeploymentFrequencyModel != null)
                {
                    items.Add(newDeploymentFrequencyModel);
                }
            }
            foreach (GitHubSettings item in githubSettings)
            {
                DeploymentFrequencyModel newDeploymentFrequencyModel = await serviceApiClient.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret,
                        item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId,
                        numberOfDays, maxNumberOfItems, useCache);
                newDeploymentFrequencyModel.ItemOrder = item.ItemOrder;
                if (newDeploymentFrequencyModel != null)
                {
                    items.Add(newDeploymentFrequencyModel);
                }
            }

            //sort the list
            items = items.OrderBy(o => o.ItemOrder).ToList();
            return View(items);
        }

        public async Task<IActionResult> LeadTimeForChanges()
        {
            string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
            string clientId = Configuration["AppSettings:GitHubClientId"];
            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
            int maxNumberOfItems = 20;
            int numberOfDays = 60;
            bool getSampleData = false;
            bool useCache = true;
            ServiceApiClient serviceApiClient = new ServiceApiClient(Configuration);
            List<LeadTimeForChangesModel> items = new List<LeadTimeForChangesModel>();

            //Get a list of settings
            List<AzureDevOpsSettings> azureDevOpsSettings = await serviceApiClient.GetAzureDevOpsSettings();
            List<GitHubSettings> githubSettings = await serviceApiClient.GetGitHubSettings();

            //Create deployment frequency models from each setting object
            foreach (AzureDevOpsSettings item in azureDevOpsSettings)
            {
                LeadTimeForChangesModel newLeadTimeForChangesModel = await serviceApiClient.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken,
                        item.Organization, item.Project, item.Repository, item.Branch, item.BuildName, item.BuildId,
                        numberOfDays, maxNumberOfItems, useCache);
                newLeadTimeForChangesModel.ItemOrder = item.ItemOrder;
                if (newLeadTimeForChangesModel != null)
                {
                    items.Add(newLeadTimeForChangesModel);
                }
            }
            foreach (GitHubSettings item in githubSettings)
            {
                LeadTimeForChangesModel newLeadTimeForChangesModel = await serviceApiClient.GetGitHubLeadTimeForChanges(getSampleData, clientId, clientSecret,
                        item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId,
                        numberOfDays, maxNumberOfItems, useCache);
                newLeadTimeForChangesModel.ItemOrder = item.ItemOrder;
                if (newLeadTimeForChangesModel != null)
                {
                    items.Add(newLeadTimeForChangesModel);
                }
            }

            //sort the list
            items = items.OrderBy(o => o.ItemOrder).ToList();
            return View(items);
        }

        public async Task<IActionResult> MeanTimeToRestore()
        {
            int maxNumberOfItems = 20;
            int numberOfDays = 60;
            bool getSampleData = false;
            bool useCache = true;
            ServiceApiClient serviceApiClient = new ServiceApiClient(Configuration);
            List<MeanTimeToRestoreModel> items = new List<MeanTimeToRestoreModel>();

            //Get a list of settings
            List<AzureDevOpsSettings> azureDevOpsSettings = await serviceApiClient.GetAzureDevOpsSettings();
            List<GitHubSettings> githubSettings = await serviceApiClient.GetGitHubSettings();

            //Create MTTR models from each setting object
            foreach (AzureDevOpsSettings item in azureDevOpsSettings)
            {
                MeanTimeToRestoreModel newMeanTimeToRestoreModel = await serviceApiClient.GetAzureMeanTimeToRestore(getSampleData,
                        item.ProductionResourceGroup, DevOpsPlatform.AzureDevOps, numberOfDays, maxNumberOfItems, useCache);
                newMeanTimeToRestoreModel.ItemOrder = item.ItemOrder;
                if (newMeanTimeToRestoreModel != null)
                {
                    items.Add(newMeanTimeToRestoreModel);
                }
            }
            foreach (GitHubSettings item in githubSettings)
            {
                MeanTimeToRestoreModel newMeanTimeToRestoreModel = await serviceApiClient.GetAzureMeanTimeToRestore(getSampleData,
                        item.ProductionResourceGroup, DevOpsPlatform.GitHub, numberOfDays, maxNumberOfItems, useCache);
                newMeanTimeToRestoreModel.ItemOrder = item.ItemOrder;
                if (newMeanTimeToRestoreModel != null)
                {
                    items.Add(newMeanTimeToRestoreModel);
                }
            }

            //sort the list
            items = items.OrderBy(o => o.ItemOrder).ToList();
            return View(items);
        }

        public async Task<IActionResult> ChangeFailureRate()
        {
            int maxNumberOfItems = 20;
            int numberOfDays = 60;
            bool getSampleData = true;
            bool useCache = true;
            ServiceApiClient serviceApiClient = new ServiceApiClient(Configuration);
            List<ChangeFailureRateModel> items = new List<ChangeFailureRateModel>();

            //Get a list of settings
            List<AzureDevOpsSettings> azureDevOpsSettings = await serviceApiClient.GetAzureDevOpsSettings();
            List<GitHubSettings> githubSettings = await serviceApiClient.GetGitHubSettings();

            //Create MTTR models from each setting object
            foreach (AzureDevOpsSettings item in azureDevOpsSettings)
            {
                ChangeFailureRateModel changeFailureRateModel = await serviceApiClient.GetChangeFailureRate(getSampleData,
                        DevOpsPlatform.AzureDevOps, item.Organization, item.Project, item.Branch, item.BuildName, item.BuildId,
                        numberOfDays, maxNumberOfItems, useCache);
                //changeFailureRateModel.ItemOrder = item.ItemOrder;
                if (changeFailureRateModel != null)
                {
                    items.Add(changeFailureRateModel);
                }
            }
            foreach (GitHubSettings item in githubSettings)
            {
                ChangeFailureRateModel changeFailureRateModel = await serviceApiClient.GetChangeFailureRate(getSampleData,
                        DevOpsPlatform.GitHub, item.Owner, item.Repo, item.Branch, item.WorkflowName, item.WorkflowId, numberOfDays, maxNumberOfItems, useCache);
                //changeFailureRateModel.ItemOrder = item.ItemOrder;
                if (changeFailureRateModel != null)
                {
                    items.Add(changeFailureRateModel);
                }
            }

            //sort the list
            //items = items.OrderBy(o => o.ItemOrder).ToList();
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
