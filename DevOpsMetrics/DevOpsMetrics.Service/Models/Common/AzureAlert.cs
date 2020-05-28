using DevOpsMetrics.Service.DataAccess.TableStorage;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace DevOpsMetrics.Service.Models.Common
{
    public class AzureAlert
    {
        //JSON serialization requires an empty constructor
        public AzureAlert() { }

        public string name { get; set; }
        public string resourceGroupName { get; set; }
        public string resourceName { get; set; }
        public string status { get; set; }
        public DateTime timestamp { get; set; }
    }
}
