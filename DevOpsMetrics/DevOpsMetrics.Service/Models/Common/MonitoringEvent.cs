namespace DevOpsMetrics.Service.Models.Common
{
    public class MonitoringEvent
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string RequestBody { get; set; }

    }
}
