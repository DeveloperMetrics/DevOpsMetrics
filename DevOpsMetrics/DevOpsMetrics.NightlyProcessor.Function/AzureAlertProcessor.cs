using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DevOpsMetrics.NightlyProcessor.Function
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
                .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
                .AddEnvironmentVariables()
                .Build();

            //Process response
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            log.LogInformation($"C# HTTP trigger function processed request body {requestBody}.");


            //save response to table
            ServiceApiClient api = new ServiceApiClient(configuration);
            bool result = await api.UpdateDevOpsMonitoringEvent("MonitoringEvent", req.ContentLength.ToString(), requestBody);

            string responseMessage = "monitoring event processed successfully";

            return new OkObjectResult(responseMessage);
        }
    }
}
