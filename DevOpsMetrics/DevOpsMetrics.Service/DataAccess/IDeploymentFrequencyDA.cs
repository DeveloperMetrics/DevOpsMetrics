using DevOpsMetrics.Service.Models.Common;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public interface IDeploymentFrequencyDA
    {
        Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, TableStorageAuth tableStorageAuth, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache);
        Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, TableStorageAuth tableStorageAuth, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays, int maxNumberOfItems, bool useCache);
    }
}