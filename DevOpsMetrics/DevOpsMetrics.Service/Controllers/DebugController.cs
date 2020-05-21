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
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountAccessKey"] + Environment.NewLine;
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsBuilds"] + Environment.NewLine;
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRs"] + Environment.NewLine;
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountContainerAzureDevOpsPRCommits"] + Environment.NewLine;
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountContainerGitHubRuns"] + Environment.NewLine;
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountContainerGitHubPRs"] + Environment.NewLine;
            result += "AzureStorageAccountName: " + Configuration["AppSettings:AzureStorageAccountContainerGitHubPRCommits"] + Environment.NewLine;
            return result;
        }
    }
}
