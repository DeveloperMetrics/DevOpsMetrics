using DevOpsMetrics.Service.Models.Common;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class Common
    {
        public static TableStorageAuth GenerateTableAuthorization(IConfiguration Configuration)
        {
            TableStorageAuth tableStorageAuth = new TableStorageAuth
            {
                AccountName = Configuration["AppSettings:AzureStorageAccountName"],
                AccountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"],
                TableAzureDevOpsBuilds = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"],
                TableAzureDevOpsPRs = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"],
                TableAzureDevOpsPRCommits = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"],
                TableAzureDevOpsSettings = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsSettings"],
                TableGitHubRuns = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"],
                TableGitHubPRs = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"],
                TableGitHubPRCommits = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"],
                TableGitHubSettings = Configuration["AppSettings:AzureStorageAccountContainerGitHubSettings"],
                TableMMTRRaw = Configuration["AppSettings:AzureStorageAccountContainerMMTRRaw"],
                TableMMTRSummary = Configuration["AppSettings:AzureStorageAccountContainerMMTRSummary"],
            };
            return tableStorageAuth;
        }
    }
}
