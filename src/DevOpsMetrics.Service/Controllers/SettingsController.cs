using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using DevOpsMetrics.Service.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly IAzureTableStorageDA AzureTableStorageDA;

        public SettingsController(IConfiguration configuration, IAzureTableStorageDA azureTableStorageDA)
        {
            Configuration = configuration;
            AzureTableStorageDA = azureTableStorageDA;
        }

        [HttpGet("GetAzureDevOpsSettings")]
        public async Task<List<AzureDevOpsSettings>> GetAzureDevOpsSettings(string rowKey = null)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            List<AzureDevOpsSettings> settings = await AzureTableStorageDA.GetAzureDevOpsSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings, rowKey);
            return settings;
        }

        [HttpGet("GetGitHubSettings")]
        public async Task<List<GitHubSettings>> GetGitHubSettings(string rowKey = null)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            List<GitHubSettings> settings = await AzureTableStorageDA.GetGitHubSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings, rowKey);
            return settings;
        }

        [HttpGet("UpdateAzureDevOpsSetting")]
        public async Task<bool> UpdateAzureDevOpsSetting(string patToken,
                string organization, string project, string repository,
                string branch, string buildName, string buildId, string resourceGroup,
                int itemOrder, bool showSetting)
        {
            //Save the PAT token to the key vault
            string patTokenName = PartitionKeys.CreateAzureDevOpsSettingsPartitionKeyPatToken(organization, project, repository);
            patTokenName = SecretsProcessing.CleanKey(patTokenName);
            if (patTokenName.Length > 12)
            {
                await CreateKeyVaultSecret(patTokenName, patToken);
            }

            //Save everything else to table storage
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateAzureDevOpsSettingInStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings,
                     organization, project, repository, branch, buildName, buildId, resourceGroup, itemOrder, showSetting);
        }

        [HttpGet("UpdateGitHubSetting")]
        public async Task<bool> UpdateGitHubSetting(string clientId, string clientSecret,
                string owner, string repo,
                string branch, string workflowName, string workflowId, string resourceGroup,
                int itemOrder, bool showSetting)
        {
            //Save the Client Id and Client Secret to the key vault
            string clientIdName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientId(owner, repo);
            clientIdName = SecretsProcessing.CleanKey(clientIdName);
            if (clientIdName.Length > 10)
            {
                await CreateKeyVaultSecret(clientIdName, clientId);
            }
            string clientSecretName = PartitionKeys.CreateGitHubSettingsPartitionKeyClientSecret(owner, repo);
            clientSecretName = SecretsProcessing.CleanKey(clientSecretName);
            if (clientSecretName.Length > 14)
            {
                await CreateKeyVaultSecret(clientSecretName, clientSecret);
            }

            //Save everything else to table storage
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateGitHubSettingInStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroup, itemOrder, showSetting);
        }

        [HttpPost("UpdateDevOpsMonitoringEvent")]
        public async Task<bool> UpdateDevOpsMonitoringEvent([FromBody] MonitoringEvent monitoringEvent)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateDevOpsMonitoringEventInStorage(tableStorageConfig, monitoringEvent);
        }

        [HttpGet("GetAzureDevOpsProjectLog")]
        public async Task<List<ProjectLog>> GetAzureDevOpsProjectLog(string organization, string project, string repository)
        {
            string partitionKey = PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.GetProjectLogsFromStorage(tableStorageConfig, partitionKey);
        }

        [HttpGet("UpdateAzureDevOpsProjectLog")]
        public async Task<bool> UpdateAzureDevOpsProjectLog(string organization, string project, string repository,
            int buildsUpdated, int prsUpdated, string buildUrl, string prUrl,
            string exceptionMessage, string exceptionStackTrace)
        {
            ProjectLog log = new(
                PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository),
                buildsUpdated, prsUpdated, HttpUtility.UrlDecode(buildUrl), HttpUtility.UrlDecode(prUrl), exceptionMessage, exceptionStackTrace);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, log);
        }

        [HttpGet("GetGitHubProjectLog")]
        public async Task<List<ProjectLog>> GetGitHubProjectLog(string owner, string repo)
        {
            string partitionKey = PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.GetProjectLogsFromStorage(tableStorageConfig, partitionKey);
        }

        [HttpGet("UpdateGitHubProjectLog")]
        public async Task<bool> UpdateGitHubProjectLog(string owner, string repo,
            int buildsUpdated, int prsUpdated, string buildUrl, string prUrl,
            string exceptionMessage, string exceptionStackTrace)
        {
            ProjectLog log = new(
                PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo),
                buildsUpdated, prsUpdated, HttpUtility.UrlDecode(buildUrl), HttpUtility.UrlDecode(prUrl), exceptionMessage, exceptionStackTrace);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, log);
        }

        private async Task<KeyVaultSecret> CreateKeyVaultSecret(string secretName, string secretValue)
        {
            string keyVaultURI = Configuration["AppSettings:KeyVaultURL"];
            SecretClient secretClient = new(new Uri(keyVaultURI), new DefaultAzureCredential());
            return await secretClient.SetSecretAsync(secretName, secretValue);
        }

    }
}