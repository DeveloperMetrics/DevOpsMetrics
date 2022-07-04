using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DORASummaryController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly IAzureTableStorageDA AzureTableStorageDA;

        public DORASummaryController(IConfiguration configuration, IAzureTableStorageDA azureTableStorageDA)
        {
            Configuration = configuration;
            AzureTableStorageDA = azureTableStorageDA;
        }

        // Get DORA Summary Items
        //[HttpGet("GetDORASummaryItems")]
        //public async Task<DORASummaryItem> GetDORASummaryItems(bool getSampleData,
        //    string organization, string project, string repository, string branch, string buildName,
        //    int numberOfDays, int maxNumberOfItems, bool useCache)
        //{
        //    DORASummaryItem model = new();

        //    AzureTableStorageDA.GetAzureDevOpsSettingsFromStorage(, "DevOpsDORASummaryItem")
        //        TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);

        //        //Get the PAT token from the key vault
        //        string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(organization, project, repository);
        //        patTokenName = SecretsProcessing.CleanKey(patTokenName);
        //        string patToken = Configuration[patTokenName];
        //        if (string.IsNullOrEmpty(patToken) == true)
        //        {
        //            throw new Exception($"patToken '{patTokenName}' not found in key vault");
        //        }

        //        DeploymentFrequencyDA da = new();
        //        model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, tableStorageConfig, organization, project, branch, buildName, numberOfDays, maxNumberOfItems, useCache);

        //    return model;
        //}

       

    }
}