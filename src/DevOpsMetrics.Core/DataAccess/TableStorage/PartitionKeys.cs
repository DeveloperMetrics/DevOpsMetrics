namespace DevOpsMetrics.Core.DataAccess.TableStorage
{
    public static class PartitionKeys
    {
        public static string CreateAzureDevOpsSettingsPartitionKey(string organization, string project, string repository)
        {
            return organization + "_" + project + "_" + repository;
        }

        public static string CreateBuildWorkflowPartitionKey(string organization_owner, string project_repo, string buildName_workflowName)
        {
            return organization_owner + "_" + project_repo + "_" + buildName_workflowName;
        }

        public static string CreateAzureDevOpsPRPartitionKey(string organization, string project)
        {
            return organization + "_" + project;
        }

        public static string CreateAzureDevOpsPRCommitPartitionKey(string organization, string project, string pullRequestId)
        {
            return organization + "_" + project + "_" + pullRequestId;
        }

        public static string CreateGitHubSettingsPartitionKey(string owner, string repo)
        {
            return owner + "_" + repo;
        }

        public static string CreateGitHubPRPartitionKey(string owner, string repo)
        {
            return owner + "_" + repo;
        }

        public static string CreateGitHubPRCommitPartitionKey(string owner, string repo, string pullRequestId)
        {
            return owner + "_" + repo + "_" + pullRequestId;
        }
    }
}
