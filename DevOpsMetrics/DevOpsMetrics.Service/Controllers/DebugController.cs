using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private IConfiguration Configuration;
        public DebugController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("GetDebug")]
        public string GetDebug()
        {
            string result = "";
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountName"] + Environment.NewLine;
            result += "AzureStorageAccountAccessKey: " + Configuration["AppSettings:AzureStorageAccountAccessKey"] + Environment.NewLine;
            result += "AzureStorageAccountContainerAzureDevOpsBuilds: " + Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"] + Environment.NewLine;
            result += "AzureStorageAccountContainerAzureDevOpsPRs: " + Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"] + Environment.NewLine;
            result += "AzureStorageAccountContainerAzureDevOpsPRCommits: " + Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"] + Environment.NewLine;
            result += "AzureStorageAccountContainerGitHubRuns: " + Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"] + Environment.NewLine;
            result += "AzureStorageAccountContainerGitHubPRs: " + Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"] + Environment.NewLine;
            result += "AzureStorageAccountContainerGitHubPRCommits: " + Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"] + Environment.NewLine;
            return result;
        }
    }
}
