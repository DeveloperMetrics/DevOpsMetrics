using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeFailureRateController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ChangeFailureRateController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("GetChangeFailureRate")]
        public ChangeFailureRateModel GetChangeFailureRate(bool getSampleData,
            DevOpsPlatform targetDevOpsPlatform, string organization_owner, string project_repo, string branch, string buildName_workflowName,
            int numberOfDays, int maxNumberOfItems)
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            ChangeFailureRateModel model = da.GetChangeFailureRate(getSampleData, tableStorageAuth, targetDevOpsPlatform,
                organization_owner, project_repo, branch, buildName_workflowName,
                numberOfDays, maxNumberOfItems);
            return model;
        }

        [HttpGet("UpdateChangeFailureRate")]
        public async Task<bool> UpdateChangeFailureRate(string organization_owner, string project_repo, string buildName_workflowName, int percentComplete, int numberOfDays)
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            return await da.UpdateChangeFailureRate(tableStorageAuth,
                organization_owner, project_repo, buildName_workflowName,
                percentComplete, numberOfDays);

        }

    }
}