using System;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service.Utility;
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
            TableStorageConfiguration tableStorageConfiguration = new();
            DORASummaryItem model = DORASummaryDA.GetDORASummaryItem(tableStorageConfiguration, owner, repository);

            return model;
        }



    }
}