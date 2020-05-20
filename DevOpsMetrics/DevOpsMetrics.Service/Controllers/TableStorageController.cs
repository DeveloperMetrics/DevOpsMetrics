﻿using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableStorageController : ControllerBase
    {
        private IConfiguration Configuration;
        public TableStorageController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Get builds from the Azure DevOps API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="patToken"></param>
        /// <param name="organization"></param>
        /// <param name="project"></param>
        /// <param name="branch"></param>
        /// <param name="buildName"></param>
        /// <param name="buildId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("GetAzureDevOpsDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken, TableStorageAuth tableStorageAuth, string organization, string project, string branch, string buildName, string buildId, int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, tableStorageAuth, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.DeploymentName = buildName;
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;
        }

        /// <summary>
        /// Get builds from the Azure DevOps API, and save new records to the storage table
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="patToken"></param>
        /// <param name="organization"></param>
        /// <param name="project"></param>
        /// <param name="branch"></param>
        /// <param name="buildName"></param>
        /// <param name="buildId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("UpdateAzureDevOpsBuilds")]
        public async Task<int> UpdateAzureDevOpsBuilds(bool getSampleData, string patToken,
                string organization, string project, string branch, string buildName, string buildId,
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved = 0;
            try
            {
                TableStorageAuth tableStorageAuth = new TableStorageAuth
                {
                    AccountName = Configuration["AppSettings:AzureStorageAccountName"],
                    AccountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"],
                    TableAzureDevOpsBuilds = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"],
                    TableAzureDevOpsPRs = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"],
                    TableAzureDevOpsPRCommits = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"],
                    TableGitHubRuns = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"],
                    TableGitHubPRs = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"],
                    TableGitHubPRCommits = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"],
                };
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateAzureDevOpsBuilds(patToken, tableStorageAuth, tableStorageAuth.TableAzureDevOpsBuilds, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

        /// <summary>
        /// Get builds from the GitHub API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <param name="branch"></param>
        /// <param name="workflowName"></param>
        /// <param name="workflowId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("UpdateGitHubActionRuns")]
        public async Task<int> UpdateGitHubActionRuns(bool getSampleData, string clientId, string clientSecret,
                string owner, string repo, string branch, string workflowName, string workflowId,
                int numberOfDays, int maxNumberOfItems)
        {
            int numberOfRecordsSaved = 0;
            try
            {
                TableStorageAuth tableStorageAuth = new TableStorageAuth
                {
                    AccountName = Configuration["AppSettings:AzureStorageAccountName"],
                    AccountAccessKey = Configuration["AppSettings:AzureStorageAccountAccessKey"],
                    TableAzureDevOpsBuilds = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"],
                    TableAzureDevOpsPRs = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"],
                    TableAzureDevOpsPRCommits = Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"],
                    TableGitHubRuns = Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"],
                    TableGitHubPRs = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"],
                    TableGitHubPRCommits = Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"],
                };
                AzureTableStorageDA da = new AzureTableStorageDA();
                numberOfRecordsSaved = await da.UpdateGitHubActionRuns(clientId, clientSecret, tableStorageAuth, tableStorageAuth.TableGitHubRuns,
                        owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    numberOfRecordsSaved = -1;
                }
                else
                {
                    throw;
                }
            }
            return numberOfRecordsSaved;
        }

    }
}