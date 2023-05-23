using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class AzureMonitorTests : BaseConfiguration
    {
        [TestMethod]
        public async Task AzureMonitorProcessingTest()
        {
            //Arrange
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(base.Configuration);
            MonitoringEvent monitoringEvent = new(@"
{
  ""schemaId"": ""AzureMonitorMetricAlert"",
  ""data"": {
    ""version"": ""2.0"",
    ""properties"": null,
    ""status"": ""Activated"",
    ""context"": {
      ""timestamp"": ""2020-05-25T00:11:28.7763615Z"",
      ""id"": ""/subscriptions/07db7d0b-a6cb-4e58-b07e-e1d541c39f5b/resourceGroups/SamsFeatureFlags/providers/microsoft.insights/metricAlerts/ServerErrors%20featureflags-data-eu-service"",
      ""name"": ""ServerErrors featureflags-data-eu-service"",
      ""description"": """",
      ""conditionType"": ""SingleResourceMultipleMetricCriteria"",
      ""severity"": ""3"",
      ""condition"": {
        ""windowSize"": ""PT5M"",
        ""allOf"": [
          {
            ""metricName"": ""Http5xx"",
            ""metricNamespace"": ""Microsoft.Web/sites"",
            ""operator"": ""GreaterThan"",
            ""threshold"": ""10"",
            ""timeAggregation"": ""Total"",
            ""dimensions"": [
              {
                ""name"": ""ResourceId"",
                ""value"": ""featureflags-data-eu-service.azurewebsites.net""
              }
            ],
            ""metricValue"": 11.0,
            ""webTestName"": null
          }
        ]
      },
      ""subscriptionId"": ""07db7d0b-a6cb-4e58-b07e-e1d541c39f5b"",
      ""resourceGroupName"": ""SamsFeatureFlags"",
      ""resourceName"": ""featureflags-data-eu-service"",
      ""resourceType"": ""Microsoft.Web/sites"",
      ""resourceId"": ""/subscriptions/07db7d0b-a6cb-4e58-b07e-e1d541c39f5b/resourceGroups/SamsFeatureFlags/providers/Microsoft.Web/sites/featureflags-data-eu-service"",
      ""portalLink"": ""https://portal.azure.com/#resource/subscriptions/07db7d0b-a6cb-4e58-b07e-e1d541c39f5b/resourceGroups/SamsFeatureFlags/providers/Microsoft.Web/sites/featureflags-data-eu-service""
    }
  }
}
");

            //Act
            AzureTableStorageDA da = new();
            bool result = await da.UpdateDevOpsMonitoringEventInStorage(tableStorageConfig, monitoringEvent);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
