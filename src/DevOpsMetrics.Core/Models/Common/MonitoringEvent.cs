using System;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.Models.Common
{
    public class MonitoringEvent
    {
        public MonitoringEvent()
        {
        } //Note this parameter-less function is required for JSON serialization

        public MonitoringEvent(string requestBody)
        {
            //Do some processing to extract some key properties from the json
            JObject json = JObject.Parse(requestBody);
            string resourceGroup = json["data"]["context"]["resourceGroupName"].ToString();
            string dateString = json["data"]["context"]["timestamp"].ToString();

            //Set the properties
            PartitionKey = resourceGroup;
            RowKey = DateTime.Parse(dateString).ToString("yyyy-MMM-dd-HH-mm-ss.fff");
            RequestBody = requestBody;
        }

        public string PartitionKey
        {
            get; set;
        }
        public string RowKey
        {
            get; set;
        }
        public string RequestBody
        {
            get; set;
        }

    }
}
