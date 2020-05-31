using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeanTimeToRestoreController : ControllerBase
    {
        private IConfiguration Configuration;
        public MeanTimeToRestoreController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("GetAzureMeanTimeToRestore")]
        public async Task<MeanTimeToRestoreModel> GetAzureMeanTimeToRestore(bool getSampleData,
            DevOpsPlatform targetDevOpsPlatform, string resourceGroup,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            MeanTimeToRestoreDA da = new MeanTimeToRestoreDA();
            MeanTimeToRestoreModel model = await da.GetAzureMeanTimeToRestore(getSampleData, tableStorageAuth,
                targetDevOpsPlatform, resourceGroup,
                numberOfDays, maxNumberOfItems, useCache);
            return model;
        }

    }
}