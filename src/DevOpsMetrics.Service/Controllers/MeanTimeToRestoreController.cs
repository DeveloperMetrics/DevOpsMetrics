using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeanTimeToRestoreController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public MeanTimeToRestoreController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("GetAzureMeanTimeToRestore")]
        public async Task<MeanTimeToRestoreModel> GetAzureMeanTimeToRestore(bool getSampleData,
            DevOpsPlatform targetDevOpsPlatform, string resourceGroup,
            int numberOfDays, int maxNumberOfItems)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            MeanTimeToRestoreModel model = await MeanTimeToRestoreDA.GetAzureMeanTimeToRestore(getSampleData, tableStorageConfig,
                targetDevOpsPlatform, resourceGroup,
                numberOfDays, maxNumberOfItems);
            return model;
        }

    }
}