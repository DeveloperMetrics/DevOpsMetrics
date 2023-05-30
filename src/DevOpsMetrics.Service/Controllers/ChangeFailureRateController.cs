using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
        public async Task<ChangeFailureRateModel> GetChangeFailureRate(bool getSampleData,
            DevOpsPlatform targetDevOpsPlatform, string organization_owner, string project_repo, string branch, string buildName_workflowName,
            int numberOfDays, int maxNumberOfItems)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            ChangeFailureRateModel model = await ChangeFailureRateDA.GetChangeFailureRate(getSampleData, tableStorageConfig, targetDevOpsPlatform,
                organization_owner, project_repo, branch, buildName_workflowName,
                numberOfDays, maxNumberOfItems);
            return model;
        }

        [HttpGet("UpdateChangeFailureRate")]
        public async Task<bool> UpdateChangeFailureRate(string organization_owner, string project_repo,
            string buildName_workflowName, int percentComplete, int numberOfDays)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await ChangeFailureRateDA.UpdateChangeFailureRate(tableStorageConfig,
                organization_owner, project_repo, buildName_workflowName,
                percentComplete, numberOfDays);

        }

    }
}