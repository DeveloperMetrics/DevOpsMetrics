using DevOpsMetrics.Service.Models;
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

        public async Task<List<AzureDevOpsBuild>> GetAZDeployments(string patToken, string organization, string project, string branch, string buildId)
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

        public async Task<float> GetAZDeploymentFrequency(string patToken, string organization, string project, string branch, string buildId, int numberOfDays)
        {
            string url = $"/api/DeploymentFrequency/GetAzDeploymentFrequency?patToken={patToken}&organization={organization}&project={project}&AzureDevOpsbranch={branch}&buildId={buildId}&numberOfDays={numberOfDays}";
            return await GetResponse<float>(_client, url);
        }

        public async Task<List<GitHubActionsRun>> GetGHDeployments(string owner, string repo, string branch, string workflowId)
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

        public async Task<float> GetGHDeploymentFrequency(string owner, string repo, string branch, string workflowId, int numberOfDays)
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
