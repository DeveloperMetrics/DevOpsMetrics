using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOpsMetrics.Function
{
    public static class AzureAlertProcessor
    {
        [FunctionName("AzureAlertProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //Load settings
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables()
                .Build();
            ServiceApiClient serviceApiClient = new(configuration);

            //Process response
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"C# HTTP trigger function processed request body {requestBody}.");

            //save response to table
            //ServiceApiClient api = new ServiceApiClient(configuration);
            //SettingsController settingsController = new(configuration, new AzureTableStorageDA());
            MonitoringEvent monitoringEvent = new(requestBody);
            await serviceApiClient.UpdateDevOpsMonitoringEvent(monitoringEvent);

            string responseMessage = "monitoring event processed successfully";
            return new OkObjectResult(responseMessage);
        }
    }
}
