using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public HealthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("Get")]
        public string Get()
        {
            return "healthy";
        }

    }
}