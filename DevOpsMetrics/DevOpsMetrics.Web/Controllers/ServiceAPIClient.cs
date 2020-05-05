using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Web.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Web.Controllers
{
    public class ServiceApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public ServiceApiClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_configuration["AppSettings:WebServiceURL"])
            };
        }
        public async Task<IndexDeploymentModel> GetIndexPage(string patToken, string organization, string project, string azBranch, string buildId,
                                                            string owner, string repo, string ghBranch, string workflowId,
                                                            int numberOfDays)
        {
            IndexDeploymentModel index = new IndexDeploymentModel();
            List<AzureDevOpsBuild> azList = await GetAZDeployments(patToken, organization, project, azBranch, buildId);
            float azDeploymentFrequency = await GetAZDeploymentFrequency(patToken, organization, project, azBranch, buildId, numberOfDays);
            List<GitHubActionsRun> ghList = await GetGHDeployments(owner, repo, ghBranch, workflowId);
            float ghDeploymentFrequency = await GetGHDeploymentFrequency(owner, repo, ghBranch, workflowId, numberOfDays);

            IndexDeploymentModel indexModel = new IndexDeploymentModel();

            //Limit Azure DevOps to latest 10 results
            if (azList.Count < 10)
            {
                indexModel.AZList = azList;
            }
            else
            {
                indexModel.AZList = new List<AzureDevOpsBuild>();
                //Only show the last ten builds
                for (int i = azList.Count - 10; i < azList.Count; i++)
                {
                    indexModel.AZList.Add(azList[i]);
                }
                indexModel.AZList[7].status = "failed";
            }
            indexModel.AZDeploymentFrequency = azDeploymentFrequency;

            //Limit Github to latest 10 results
            if (ghList.Count < 10)
            {
                indexModel.GHList = ghList;
            }
            else
            {
                indexModel.GHList = new List<GitHubActionsRun>();
                //Only show the last ten builds
                for (int i = ghList.Count - 10; i < ghList.Count; i++)
                {
                    indexModel.GHList.Add(ghList[i]);
                }
                indexModel.GHList[2].status = "failed";
                indexModel.GHList[3].status = "failed";
            }
            indexModel.GHDeploymentFrequency = ghDeploymentFrequency;

            return index;
        }

        private async Task<List<AzureDevOpsBuild>> GetAZDeployments(string patToken, string organization, string project, string branch, string buildId)
        {
            string url = $"/api/DeploymentFrequency/GetAzDeployments?patToken={patToken}&organization={organization}&project={project}&AzureDevOpsbranch={branch}&buildId={buildId}";
            List<AzureDevOpsBuild> results = await GetResponse<List<AzureDevOpsBuild>>(_client, url);
            if (results == null)
            {
                return new List<AzureDevOpsBuild>();
            }
            else
            {
                return results;
            }
        }

        private async Task<float> GetAZDeploymentFrequency(string patToken, string organization, string project, string branch, string buildId, int numberOfDays)
        {
            string url = $"/api/DeploymentFrequency/GetAzDeploymentFrequency?patToken={patToken}&organization={organization}&project={project}&AzureDevOpsbranch={branch}&buildId={buildId}&numberOfDays={numberOfDays}";
            return await GetResponse<float>(_client, url);
        }

        private async Task<List<GitHubActionsRun>> GetGHDeployments(string owner, string repo, string branch, string workflowId)
        {
            string url = $"/api/DeploymentFrequency/GetGHDeployments?owner={owner}&repo={repo}&GHbranch={branch}&workflowId={workflowId}";
            List<GitHubActionsRun> results = await GetResponse<List<GitHubActionsRun>>(_client, url);
            if (results == null)
            {
                return new List<GitHubActionsRun>();
            }
            else
            {
                return results;
            }
        }

        private async Task<float> GetGHDeploymentFrequency(string owner, string repo, string branch, string workflowId, int numberOfDays)
        {
            string url = $"/api/DeploymentFrequency/GetGHDeploymentFrequency?owner={owner}&repo={repo}&GHbranch={branch}&workflowId={workflowId}&numberOfDays={numberOfDays}";
            return await GetResponse<float>(_client, url);
        }

        private async Task<T> GetResponse<T>(HttpClient client, string url)
        {
            T obj = default;
            if (client != null && url != null)
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody) == false)
                    {
                        obj = JsonConvert.DeserializeObject<T>(responseBody);
                    }
                }
            }
            return obj;
        }
    }
}
