using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DORASummaryController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DORASummaryController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Get DORA Summary Items
        [HttpGet("GetDORASummaryItems")]
        public DORASummaryItem GetDORASummaryItems(string owner, string repository)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            DORASummaryItem model = DORASummaryDA.GetDORASummaryItem(tableStorageConfig, owner, repository);

            return model;
        }

    }
}