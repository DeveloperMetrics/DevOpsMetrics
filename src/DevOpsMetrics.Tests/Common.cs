using DevOpsMetrics.Core.Models.Common;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class Common
    {
        public static TableStorageConfiguration GenerateTableAuthorization(IConfiguration Configuration)
        {
            TableStorageConfiguration tableStorageConfig = new TableStorageConfiguration
            {
                //StorageAccountName = Configuration["AppSettings:AzureStorageAccountName"],
                //StorageAccountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"],
                StorageAccountConnectionString = Configuration["AppSettings:AzureStorageAccountConfigurationString"],
                TableAzureDevOpsBuilds = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"],
                TableAzureDevOpsPRs = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"],
                TableAzureDevOpsPRCommits = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"],
                TableAzureDevOpsSettings = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsSettings"],
                TableGitHubRuns = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"],
                TableGitHubPRs = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"],
                TableGitHubPRCommits = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"],
                TableGitHubSettings = Configuration["AppSettings:AzureStorageAccountContainerGitHubSettings"],
                TableMTTR = Configuration["AppSettings:AzureStorageAccountContainerMTTR"],
                TableChangeFailureRate = Configuration["AppSettings:AzureStorageAccountContainerChangeFailureRate"],
            };
            return tableStorageConfig;
        }
    }
}
