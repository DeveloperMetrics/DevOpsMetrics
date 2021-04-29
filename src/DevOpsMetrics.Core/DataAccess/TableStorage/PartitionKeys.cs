namespace DevOpsMetrics.Core.DataAccess.TableStorage
{
    public static class PartitionKeys
    {
        public static string CreateAzureDevOpsSettingsPartitionKey(string organization, string project, string repository)
        {
            return organization + "_" + project + "_" + repository;
        }

        public static string CreateAzureDevOpsSettingsPartitionKeyPatToken(string organization, string project, string repository)
        {
            //add a azdo/Azure DevOps prefix, remove _/underscores and append a suffix for the pat token
            string result = "azdo" + CreateAzureDevOpsSettingsPartitionKey(organization, project, repository) + "patToken";
            return result.Replace("_", "");
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

        public static string CreateGitHubSettingsPartitionKeyClientId(string owner, string repo)
        {
            //add a gh/github prefix, remove _/underscores and append a suffix for the client id
            string result = "gh" + CreateGitHubSettingsPartitionKey(owner, repo) + "clientId";
            return result.Replace("_", "");
        }

        public static string CreateGitHubSettingsPartitionKeyClientSecret(string owner, string repo)
        {
            //add a gh/github prefix, remove _/underscores and append a suffix for the client secret
            string result = "gh" + CreateGitHubSettingsPartitionKey(owner, repo) + "clientSecret";
            return result.Replace("_", "");
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
