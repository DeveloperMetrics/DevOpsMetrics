using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadTimeForChangesController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public LeadTimeForChangesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Get lead time for changes from Azure DevOps API
        [HttpGet("GetAzureDevOpsLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetAzureDevOpsLeadTimeForChanges(bool getSampleData, 
            string organization, string project, string repository, string branch, string buildName, 
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            LeadTimeForChangesModel model = new LeadTimeForChangesModel();
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
                LeadTimeForChangesDA da = new LeadTimeForChangesDA();
                model = await da.GetAzureDevOpsLeadTimesForChanges(getSampleData, tableStorageConfig,
                        organization, project, repository, branch, buildName, numberOfDays, maxNumberOfItems, useCache);
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

        // Get lead time for changes from GitHub API
        [HttpGet("GetGitHubLeadTimeForChanges")]
        public async Task<LeadTimeForChangesModel> GetGitHubLeadTimeForChanges(bool getSampleData, string clientId, string clientSecret,
            string owner, string repo, string branch, string workflowName, string workflowId,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            LeadTimeForChangesModel model = new LeadTimeForChangesModel();
            try
            {
                TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
                LeadTimeForChangesDA da = new LeadTimeForChangesDA();
                model = await da.GetGitHubLeadTimesForChanges(getSampleData, clientId, clientSecret, tableStorageConfig,
                        owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);
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