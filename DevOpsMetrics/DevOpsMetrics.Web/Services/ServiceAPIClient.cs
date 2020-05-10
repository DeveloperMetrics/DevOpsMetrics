using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Web.Services
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

        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, string organization, string project, string branch, string buildName, string buildId, int numberOfDays)
        {
            string url = $"/api/DeploymentFrequency/GetAzureDevOpsDeploymentFrequency?getSampleData={getSampleData}&patToken={patToken}&organization={organization}&project={project}&AzureDevOpsbranch={branch}&buildName={buildName}&buildId={buildId}&numberOfDays={numberOfDays}";
            return await GetResponse<DeploymentFrequencyModel>(_client, url);
        }

        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowName, string workflowId, int numberOfDays)
        {
            string url = $"/api/DeploymentFrequency/GetGitHubDeploymentFrequency?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowName={workflowName}&workflowId={workflowId}&numberOfDays={numberOfDays}";
            return await GetResponse<DeploymentFrequencyModel>(_client, url);
        }

        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData, string patToken, string organization, string project, string branch, string buildId)
        {
            //if (getSampleData == true)
            //{
            //    List<PullRequestModel> results = new List<PullRequestModel>
            //    {
            //        new PullRequestModel
            //        {
            //            PullRequestId = "122",
            //            Branch = "abc122",
            //            BuildCount = 1,
            //            Commits = new List<Commit>
            //            {
            //                new Commit
            //                {
            //                    commitId = "hij",
            //                    date = DateTime.Now.AddDays(-11),
            //                    name = "commit A"
            //                }
            //            },
            //            StartDateTime = DateTime.Now.AddDays(-11),
            //            EndDateTime = DateTime.Now.AddDays(-11).AddMinutes(5)
            //        },
            //        new PullRequestModel
            //        {
            //            PullRequestId = "123",
            //            Branch = "abc123",
            //            BuildCount = 2,
            //            Commits = new List<Commit>
            //            {
            //                new Commit
            //                {
            //                    commitId = "abc",
            //                    date = DateTime.Now.AddDays(-7),
            //                    name = "commit 1"
            //                },
            //                new Commit
            //                {
            //                    commitId = "def",
            //                    date = DateTime.Now.AddDays(-5),
            //                    name = "commit 2"
            //                }
            //            },
            //            StartDateTime = DateTime.Now.AddDays(-7),
            //            EndDateTime = DateTime.Now.AddDays(-5)
            //        },
            //        new PullRequestModel
            //        {
            //            PullRequestId = "124",
            //            Branch = "xyz890",
            //            BuildCount = 3,
            //            Commits = new List<Commit>
            //            {
            //                new Commit
            //                {
            //                    commitId = "abc",
            //                    date = DateTime.Now.AddDays(-7),
            //                    name = "commit 1"
            //                },
            //                new Commit
            //                {
            //                    commitId = "def",
            //                    date = DateTime.Now.AddDays(-5),
            //                    name = "commit 2"
            //                },
            //                new Commit
            //                {
            //                    commitId = "ghi",
            //                    date = DateTime.Now.AddDays(-2),
            //                    name = "commit 3"
            //                }
            //            },
            //            StartDateTime = DateTime.Now.AddDays(-7),
            //            EndDateTime = DateTime.Now.AddDays(-2)
            //        }
            //    };
            //    return results;
            //}
            //else
            //{
                string url = $"/api/LeadTimeForChanges/GetAzureDevOpsLeadTimeForChanges?getSampleData={getSampleData}&patToken={patToken}&organization={organization}&project={project}&branch={branch}&buildId={buildId}";
                return await GetResponse<LeadTimeForChangesModel>(_client, url);
            //}
        }

        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowId)
        {
            //if (getSampleData == true)
            //{
            //    List<PullRequestModel> results = new List<PullRequestModel>
            //    {
            //        new PullRequestModel
            //        {
            //            PullRequestId = "221",
            //            Branch = "abc123",
            //            BuildCount = 2,
            //            Commits = new List<Commit>
            //            {
            //                new Commit
            //                {
            //                    commitId="abc",
            //                    date = DateTime.Now.AddDays(-7),
            //                    name = "commit 1"
            //                },
            //                new Commit
            //                {
            //                    commitId="def",
            //                    date = DateTime.Now.AddDays(-5),
            //                    name = "commit 2"
            //                }
            //            },
            //            StartDateTime = DateTime.Now.AddDays(-7),
            //            EndDateTime = DateTime.Now.AddDays(-5)
            //        },
            //        new PullRequestModel
            //        {
            //            PullRequestId = "222",
            //            Branch = "xyz890",
            //            BuildCount = 3,
            //            Commits = new List<Commit>
            //            {
            //                new Commit
            //                {
            //                    commitId="abc",
            //                    date = DateTime.Now.AddDays(-7),
            //                    name = "commit 1"
            //                },
            //                new Commit
            //                {
            //                    commitId="def",
            //                    date = DateTime.Now.AddDays(-5),
            //                    name = "commit 2"
            //                },
            //                new Commit
            //                {
            //                    commitId="ghi",
            //                    date = DateTime.Now.AddDays(-2),
            //                    name = "commit 3"
            //                }
            //            },
            //            StartDateTime = DateTime.Now.AddDays(-7),
            //            EndDateTime = DateTime.Now.AddDays(-2)
            //        }
            //    };
            //    return results;
            //}
            //else
            //{
                string url = $"/api/LeadTimeForChanges/GetGitHubLeadTimeForChanges?getSampleData={getSampleData}&clientId={clientId}&clientSecret={clientSecret}&owner={owner}&repo={repo}&branch={branch}&workflowId={workflowId}";
                return await GetResponse<LeadTimeForChangesModel>(_client, url);
            //}
        }

        private async Task<T> GetResponse<T>(HttpClient client, string url)
        {
            T obj = default;
            if (client != null && url != null)
            {
                Debug.WriteLine("Running url: " + client.BaseAddress.ToString() + url);
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
