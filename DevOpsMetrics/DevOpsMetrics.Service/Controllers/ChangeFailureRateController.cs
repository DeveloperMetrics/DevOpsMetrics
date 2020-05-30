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
        private IConfiguration Configuration;
        public ChangeFailureRateController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("GetChangeFailureRate")]
        public async Task<ChangeFailureRateModel> GetChangeFailureRate(bool getSampleData,
            string organization_owner, string project_repo, string branch, string buildName_workflowName, string buildId_workflowId,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            ChangeFailureRateModel model = new ChangeFailureRateModel();
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            ChangeFailureRateDA da = new ChangeFailureRateDA();
            model = await da.GetChangeFailureRate(getSampleData, tableStorageAuth, organization_owner, project_repo, branch, buildName_workflowName, buildId_workflowId, numberOfDays, maxNumberOfItems, useCache);
            return model;
        }

    }
}