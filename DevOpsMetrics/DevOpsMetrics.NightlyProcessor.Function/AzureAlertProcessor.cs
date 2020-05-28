using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //string name = req.Query["name"];
            //name = name ?? data?.name;
            log.LogInformation($"C# HTTP trigger function processed request body {requestBody}.");

            //save response to table
            ServiceApiClient api = new ServiceApiClient(configuration);
            await api.UpdateDevOpsMonitoringEvent(requestBody);

            string responseMessage = "monitoring event processed successfully";
            return new OkObjectResult(responseMessage);
        }
    }
}
