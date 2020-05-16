﻿using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadTimeForChangesController : ControllerBase
    {
        private IConfiguration Configuration;
        public LeadTimeForChangesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Get lead time for changes from Azure DevOps API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="patToken"></param>
        /// <param name="organization"></param>
        /// <param name="project"></param>
        /// <param name="repositoryId"></param>
        /// <param name="branch"></param>
        /// <param name="buildId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("GetAzureDevOpsLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData, string patToken, string organization, string project, string repositoryId, string branch, string buildId, int numberOfDays, int maxNumberOfItems = 20)
        {
            LeadTimeForChangesModel model = new LeadTimeForChangesModel();
            try
            {
                LeadTimeForChangesDA da = new LeadTimeForChangesDA();
                model = await da.GetAzureDevOpsLeadTimesForChanges(getSampleData, patToken, organization, project, repositoryId, branch, buildId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.ProjectName = project;
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
        /// Get lead time for changes from GitHub API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <param name="branch"></param>
        /// <param name="workflowId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <returns></returns>
        [HttpGet("GetGitHubLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData, string clientId, string clientSecret, string owner, string repo, string branch, string workflowId, int numberOfDays, int maxNumberOfItems = 20)
        {
            LeadTimeForChangesModel model = new LeadTimeForChangesModel();
            try
            {
                LeadTimeForChangesDA da = new LeadTimeForChangesDA();
                model = await da.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, owner, repo, branch, workflowId, numberOfDays, maxNumberOfItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.ProjectName = repo;
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;

        }
    }
}