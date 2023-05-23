//using System.Collections.Generic;
//using System.Threading.Tasks;
//using DevOpsMetrics.Core.DataAccess.TableStorage;
//using DevOpsMetrics.Core.Models.Common;
//using DevOpsMetrics.Core.Models.GitHub;
//using DevOpsMetrics.Service;
//using DevOpsMetrics.Service.Controllers;
//using Microsoft.Extensions.Logging;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace DevOpsMetrics.Tests.Service
//{
//    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
//    [TestCategory("IntegrationTest")]
//    [TestClass]
//    public class NightlyProcessTests : BaseConfiguration
//    {
//        [TestMethod]
//        public async Task GitHubProcessingTest()
//        {
//            //Arrange
//            string clientId = Configuration["AppSettings:GitHubClientId"];
//            string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
//            ProcessingResult result = null;
//            AzureTableStorageDA da = new();
//            BuildsController buildsController = new(Configuration, da);
//            PullRequestsController pullRequestsController = new(Configuration, da);
//            SettingsController settingsController = new(Configuration, da);
//            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
//                .SetMinimumLevel(LogLevel.Trace)
//                .AddConsole());
//            ILogger log = loggerFactory.CreateLogger<NightlyProcessTests>();
//            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

//            //Act
//            List<GitHubSettings> results = da.GetGitHubSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings, null);
//            foreach (GitHubSettings item in results)
//            {
//                if (item.Repo == "AzurePipelinesToGitHubActionsConverter")
//                {
//                    result = await Processing.ProcessGitHubItem(item, clientId, clientSecret, tableStorageConfig,
//                        30, 20,
//                        buildsController, pullRequestsController, settingsController,
//                        log, 0);
//                }
//            }

//            //Assert
//            Assert.IsNotNull(result);
//        }



//        //[TestMethod]
//        //public async Task GHUpdateAzurePipelinesToGitHubActionsConverterWebSettingDAIntegrationTest()
//        //{
//        //    //Arrange
//        //    TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
//        //    string owner = "samsmithnz";
//        //    string repo = "AzurePipelinesToGitHubActionsConverterWeb";
//        //    string branch = "main";
//        //    string workflowName = "Pipelines to Actions website CI/CD";
//        //    string workflowId = "43084";
//        //    string resourceGroupName = "PipelinesToActions";
//        //    int itemOrder = 3;
//        //    bool showSetting = true;

//        //    //Act
//        //    AzureTableStorageDA da = new();
//        //    bool result = await da.UpdateGitHubSettingInStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings,
//        //            owner, repo, branch, workflowName, workflowId, resourceGroupName, itemOrder, showSetting);

//        //    //Assert
//        //    Assert.IsTrue(result);
//        //}


//        //[TestMethod]
//        //public async Task GHUpdateAllDevOpsMetricsIntegrationTest()
//        //{
//        //    //Arrange
//        //    string clientId = Configuration["AppSettings:GitHubClientId"];
//        //    string clientSecret = Configuration["AppSettings:GitHubClientSecret"];
//        //    int numberOfDays = 30;
//        //    int maxNumberOfItems = 20;
//        //    int totalResults = 0;
//        //    using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
//        //        .SetMinimumLevel(LogLevel.Trace)
//        //        .AddConsole());
//        //    ILogger log = loggerFactory.CreateLogger<NightlyProcessTests>();
//        //    TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

//        //    //Act
//        //    AzureTableStorageDA azureTableStorageDA = new();
//        //    BuildsController buildsController = new(Configuration, azureTableStorageDA);
//        //    PullRequestsController pullRequestsController = new(Configuration, azureTableStorageDA);
//        //    SettingsController settingsController = new(Configuration, azureTableStorageDA);
//        //    List<GitHubSettings> ghSettings = settingsController.GetGitHubSettings();

//        //    foreach (GitHubSettings item in ghSettings)
//        //    {
//        //        ProcessingResult ghResult = await Processing.ProcessGitHubItem(item,
//        //            clientId, clientSecret, tableStorageConfig,
//        //            numberOfDays, maxNumberOfItems,
//        //            buildsController, pullRequestsController, settingsController, log, totalResults);
//        //        totalResults = ghResult.TotalResults;
//        //    }

//        //    //Assert
//        //    Assert.IsTrue(totalResults >= 0);
//        //}

//    }
//}
