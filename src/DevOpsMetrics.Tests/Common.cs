using DevOpsMetrics.Core.Models.Common;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class Common
    {
        public static TableStorageConfiguration GenerateTableAuthorization(IConfiguration configuration)
        {
            TableStorageConfiguration tableStorageConfig = new()
            {
                StorageAccountConnectionString = configuration["AppSettings:AzureStorageAccountConfigurationString"],
                TableAzureDevOpsBuilds = configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"],
                TableAzureDevOpsPRs = configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"],
                TableAzureDevOpsPRCommits = configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"],
                TableAzureDevOpsSettings = configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsSettings"],
                TableGitHubRuns = configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"],
                TableGitHubPRs = configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"],
                TableGitHubPRCommits = configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"],
                TableGitHubSettings = configuration["AppSettings:AzureStorageAccountContainerGitHubSettings"],
                TableMTTR = configuration["AppSettings:AzureStorageAccountContainerMTTR"],
                TableChangeFailureRate = configuration["AppSettings:AzureStorageAccountContainerChangeFailureRate"],
                TableLog = configuration["AppSettings:AzureStorageAccountContainerTableLog"]
            };
            return tableStorageConfig;
        }
    }
}
