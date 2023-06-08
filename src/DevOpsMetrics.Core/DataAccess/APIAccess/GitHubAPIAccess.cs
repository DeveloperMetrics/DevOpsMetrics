using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.DataAccess.APIAccess
{
    public class GitHubAPIAccess
    {

        //Call the GitHub Rest API to get a JSON array of runs
        public static async Task<JArray> GetGitHubActionRunsJArray(string clientId, string clientSecret, string owner, string repo, string workflowId)
        {
            JArray list = null;
            string url = $"https://api.github.com/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs?per_page=100";
            string response = await GetGitHubMessage(url, clientId, clientSecret);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(response);
                list = jsonObj.workflow_runs;
            }
            return list;
        }

        //Call the GitHub Rest API to get a JSON array of pull requests
        public static async Task<JArray> GetGitHubPullRequestsJArray(string clientId, string clientSecret, string owner, string repo, string branch)
        {
            JArray list = null;
            //https://developer.GitHub.com/v3/pulls/#list-pull-requests
            //GET /repos/:owner/:repo/pulls
            string url = $"https://api.github.com/repos/{owner}/{repo}/pulls?state=all&head={branch}&per_page=100";
            string response = await GetGitHubMessage(url, clientId, clientSecret);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                list = buildListObject;
            }
            return list;
        }

        //Call the GitHub Rest API to get a JSON array of pull request commits
        public static async Task<JArray> GetGitHubPullRequestCommitsJArray(string clientId, string clientSecret, string owner, string repo, string pull_number)
        {
            JArray list = null;
            //https://developer.GitHub.com/v3/pulls/#list-commits-on-a-pull-request
            //GET /repos/:owner/:repo/pulls/:pull_number/commits
            string url = $"https://api.github.com/repos/{owner}/{repo}/pulls/{pull_number}/commits?per_page=100";
            string response = await GetGitHubMessage(url, clientId, clientSecret);
            if (string.IsNullOrEmpty(response) == false)
            {
                dynamic buildListObject = JsonConvert.DeserializeObject(response);
                list = buildListObject;
            }
            return list;
        }

        public async static Task<string> GetGitHubMessage(string url, string clientId, string clientSecret)
        {
            //Console.WriteLine($"Running GitHub url: {url}");
            string responseBody = "";
            if (url.Contains("api.github.com") == false)
            {
                throw new Exception("api.github.com missing from URL");
            }
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DevOpsMetrics", "0.1"));
                //If we use a id/secret, we significantly increase the rate from 60 requests an hour to 5000. https://developer.github.com/v3/#rate-limiting
                if (string.IsNullOrEmpty(clientId) == false && string.IsNullOrEmpty(clientSecret) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", clientId, clientSecret))));
                }
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    //Throw a response exception
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine(responseBody);
                    }
                }
            }
            return responseBody;
        }

    }
}
