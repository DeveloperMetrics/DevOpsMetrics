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

        public async Task<List<AzureDevOpsBuild>> GetAZDeployments(bool getDemoData, string patToken, string organization, string project, string branch, string buildId)
        {
            if (getDemoData == true)
            {
                List<AzureDevOpsBuild> results = new List<AzureDevOpsBuild>();
                AzureDevOpsBuild item1 = new AzureDevOpsBuild
                {
                    queueTime = DateTime.Now.AddDays(-7).AddMinutes(-4),
                    finishTime = DateTime.Now.AddDays(-7).AddMinutes(0),
                    buildNumber = "1",
                    sourceBranch = "master",
                    status = "completed",
                    url = "https://dev.azure.com/samsmithnz/samlearnsazure/1"
                };
                results.Add(item1);
                results.Add(item1);
                AzureDevOpsBuild item2 = new AzureDevOpsBuild
                {
                    queueTime = DateTime.Now.AddDays(-5).AddMinutes(-5),
                    finishTime = DateTime.Now.AddDays(-5).AddMinutes(0),
                    buildNumber = "2",
                    sourceBranch = "master",
                    status = "completed",
                    url = "https://dev.azure.com/samsmithnz/samlearnsazure/2"
                };
                results.Add(item2);
                results.Add(item2);
                AzureDevOpsBuild item3 = new AzureDevOpsBuild
                {
                    queueTime = DateTime.Now.AddDays(-4).AddMinutes(-1),
                    finishTime = DateTime.Now.AddDays(-4).AddMinutes(0),
                    buildNumber = "3",
                    sourceBranch = "master",
                    status = "failed",
                    url = "https://dev.azure.com/samsmithnz/samlearnsazure/3"
                };
                results.Add(item3);
                AzureDevOpsBuild item4 = new AzureDevOpsBuild
                {
                    queueTime = DateTime.Now.AddDays(-3).AddMinutes(-4),
                    finishTime = DateTime.Now.AddDays(-3).AddMinutes(0),
                    buildNumber = "4",
                    sourceBranch = "master",
                    status = "completed",
                    url = "https://dev.azure.com/samsmithnz/samlearnsazure/4"
                };
                results.Add(item4);
                results.Add(item4);
                AzureDevOpsBuild item5 = new AzureDevOpsBuild
                {
                    queueTime = DateTime.Now.AddDays(-2).AddMinutes(-7),
                    finishTime = DateTime.Now.AddDays(-2).AddMinutes(0),
                    buildNumber = "5",
                    sourceBranch = "master",
                    status = "completed",
                    url = "https://dev.azure.com/samsmithnz/samlearnsazure/5"
                };
                results.Add(item5);
                results.Add(item5);
                AzureDevOpsBuild item6 = new AzureDevOpsBuild
                {
                    queueTime = DateTime.Now.AddDays(-1).AddMinutes(-5),
                    finishTime = DateTime.Now.AddDays(-1).AddMinutes(0),
                    buildNumber = "6",
                    sourceBranch = "master",
                    status = "inProgress",
                    url = "https://dev.azure.com/samsmithnz/samlearnsazure/6"
                };
                results.Add(item6);

                return results;
            }
            else
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
        }

        public async Task<DeploymentFrequencyModel> GetAZDeploymentFrequency(bool getDemoData, string patToken, string organization, string project, string branch, string buildId, int numberOfDays)
        {
            if (getDemoData == true)
            {
                return new DeploymentFrequencyModel
                {
                    deploymentsPerDay = 10f,
                    deploymentsPerDayDescription = "Elite"
                };
            }
            else
            {
                string url = $"/api/DeploymentFrequency/GetAzDeploymentFrequency?patToken={patToken}&organization={organization}&project={project}&AzureDevOpsbranch={branch}&buildId={buildId}&numberOfDays={numberOfDays}";
                return await GetResponse<DeploymentFrequencyModel>(_client, url);
            }
        }

        public async Task<List<GitHubActionsRun>> GetGHDeployments(bool getDemoData, string owner, string repo, string branch, string workflowId)
        {
            if (getDemoData == true)
            {
                List<GitHubActionsRun> results = new List<GitHubActionsRun>();
                GitHubActionsRun item1 = new GitHubActionsRun
                {
                    created_at = DateTime.Now.AddDays(-7).AddMinutes(-12),
                    updated_at = DateTime.Now.AddDays(-7).AddMinutes(0),
                    run_number = "1",
                    head_branch = "master",
                    status = "completed",
                    html_url = "https://github.com/samsmithnz/devopsmetrics/1"
                };
                results.Add(item1);
                GitHubActionsRun item2 = new GitHubActionsRun
                {
                    created_at = DateTime.Now.AddDays(-6).AddMinutes(-16),
                    updated_at = DateTime.Now.AddDays(-6).AddMinutes(0),
                    run_number = "2",
                    head_branch = "master",
                    status = "completed",
                    html_url = "https://github.com/samsmithnz/devopsmetrics/2"
                };
                results.Add(item2);
                results.Add(item2);
                GitHubActionsRun item3 = new GitHubActionsRun
                {
                    created_at = DateTime.Now.AddDays(-4).AddMinutes(-9),
                    updated_at = DateTime.Now.AddDays(-4).AddMinutes(0),
                    run_number = "3",
                    head_branch = "master",
                    status = "failed",
                    html_url = "https://github.com/samsmithnz/devopsmetrics/3"
                };
                results.Add(item3);
                GitHubActionsRun item4 = new GitHubActionsRun
                {
                    created_at = DateTime.Now.AddDays(-3).AddMinutes(-10),
                    updated_at = DateTime.Now.AddDays(-3).AddMinutes(0),
                    run_number = "4",
                    head_branch = "master",
                    status = "completed",
                    html_url = "https://github.com/samsmithnz/devopsmetrics/4"
                };
                results.Add(item4);
                results.Add(item4);
                GitHubActionsRun item5 = new GitHubActionsRun
                {
                    created_at = DateTime.Now.AddDays(-2).AddMinutes(-11),
                    updated_at = DateTime.Now.AddDays(-2).AddMinutes(0),
                    run_number = "5",
                    head_branch = "master",
                    status = "failed",
                    html_url = "https://github.com/samsmithnz/devopsmetrics/5"
                };
                results.Add(item5);
                results.Add(item5);
                GitHubActionsRun item6 = new GitHubActionsRun
                {
                    created_at = DateTime.Now.AddDays(-1).AddMinutes(-8),
                    updated_at = DateTime.Now.AddDays(-1).AddMinutes(0),
                    run_number = "6",
                    head_branch = "master",
                    status = "completed",
                    html_url = "https://github.com/samsmithnz/devopsmetrics/6"
                };
                results.Add(item6);
                results.Add(item6);

                return results;
            }
            else
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
        }

        public async Task<DeploymentFrequencyModel> GetGHDeploymentFrequency(bool getDemoData, string owner, string repo, string branch, string workflowId, int numberOfDays)
        {
            if (getDemoData == true)
            {
                return new DeploymentFrequencyModel
                {
                    deploymentsPerDay = 1f / 30f,
                    deploymentsPerDayDescription = "Low"
                };
            }
            else
            {
                string url = $"/api/DeploymentFrequency/GetGHDeploymentFrequency?owner={owner}&repo={repo}&GHbranch={branch}&workflowId={workflowId}&numberOfDays={numberOfDays}";
                return await GetResponse<DeploymentFrequencyModel>(_client, url);
            }
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
