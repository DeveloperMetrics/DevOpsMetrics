using DevOpsMetrics.Core.Models.Common;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service
{
    public class Common
    {
        public static TableStorageConfiguration GenerateTableStorageConfiguration(IConfiguration Configuration)
        {
            TableStorageConfiguration tableStorageConfig = new()
            {
                StorageAccountConnectionString = Configuration["AppSettings:AzureStorageAccountConfigurationString"]
            };

            if (Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"] != null)
            {
                tableStorageConfig.TableAzureDevOpsBuilds = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"] != null)
            {
                tableStorageConfig.TableAzureDevOpsPRs = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"] != null)
            {
                tableStorageConfig.TableAzureDevOpsPRCommits = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsSettings"] != null)
            {
                tableStorageConfig.TableAzureDevOpsSettings = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsSettings"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"] != null)
            {
                tableStorageConfig.TableGitHubRuns = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"] != null)
            {
                tableStorageConfig.TableGitHubPRs = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"] != null)
            {
                tableStorageConfig.TableGitHubPRCommits = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerGitHubSettings"] != null)
            {
                tableStorageConfig.TableGitHubSettings = Configuration["AppSettings:AzureStorageAccountContainerGitHubSettings"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerMTTR"] != null)
            {
                tableStorageConfig.TableMTTR = Configuration["AppSettings:AzureStorageAccountContainerMTTR"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerChangeFailureRate"] != null)
            {
                tableStorageConfig.TableChangeFailureRate = Configuration["AppSettings:AzureStorageAccountContainerChangeFailureRate"];
            }
            if (Configuration["AppSettings:AzureStorageAccountDORASummaryItem"] != null)
            {
                tableStorageConfig.TableDORASummaryItem = Configuration["AppSettings:AzureStorageAccountDORASummaryItem"];
            }
            if (Configuration["AppSettings:AzureStorageAccountContainerTableLog"] != null)
            {
                tableStorageConfig.TableLog = Configuration["AppSettings:AzureStorageAccountContainerTableLog"];
            }
            return tableStorageConfig;

        }
    }
}
