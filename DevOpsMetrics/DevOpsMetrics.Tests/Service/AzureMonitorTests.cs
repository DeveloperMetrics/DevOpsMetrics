﻿using DevOpsMetrics.Service.DataAccess.TableStorage;
using DevOpsMetrics.Service.Models.AzureDevOps;
using DevOpsMetrics.Service.Models.Common;
using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class AzureMonitorTests
    {
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            config.AddUserSecrets<AzureTableStorageDATests>();
            Configuration = config.Build();
        }

        [TestMethod]
        public async Task AzureMonitorProcessingTest()
        {
            //Arrange
            TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
            MonitoringEvent monitoringEvent = new MonitoringEvent
            {
                PartitionKey = "Test123",
                RowKey = DateTime.Parse("2020-05-25T00:11:28.7763615Z").ToString("yyyy-MMM-dd-HH-mm-ss.fff"),
                RequestBody = @"
{
  ""schemaId"": ""AzureMonitorMetricAlert"",
  ""data"": {
    ""version"": ""2.0"",
    ""properties"": null,
    ""status"": ""Activated"",
    ""context"": {
      ""timestamp"": ""2020-05-25T00:11:28.7763615Z"",
      ""id"": ""/subscriptions/07db7d0b-a6cb-4e58-b07e-e1d541c39f5b/resourceGroups/SamLearnsAzureFeatureFlags/providers/microsoft.insights/metricAlerts/ServerErrors%20featureflags-data-eu-service"",
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
      ""resourceGroupName"": ""SamLearnsAzureFeatureFlags"",
      ""resourceName"": ""featureflags-data-eu-service"",
      ""resourceType"": ""Microsoft.Web/sites"",
      ""resourceId"": ""/subscriptions/07db7d0b-a6cb-4e58-b07e-e1d541c39f5b/resourceGroups/SamLearnsAzureFeatureFlags/providers/Microsoft.Web/sites/featureflags-data-eu-service"",
      ""portalLink"": ""https://portal.azure.com/#resource/subscriptions/07db7d0b-a6cb-4e58-b07e-e1d541c39f5b/resourceGroups/SamLearnsAzureFeatureFlags/providers/Microsoft.Web/sites/featureflags-data-eu-service""
    }
  }
}
"
            };

            //Act
            AzureTableStorageDA da = new AzureTableStorageDA();
            bool result = await da.UpdateDevOpsMonitoringEvent(tableStorageAuth, monitoringEvent);

            //Assert
            Assert.IsTrue(result == true);
        }
    }
}
