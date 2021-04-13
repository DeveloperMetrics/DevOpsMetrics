using System;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.Models.Common
{
    public class ProjectLog
    {
        public ProjectLog() { } //Note this parameter-less function is required for JSON serialization

        public ProjectLog(string partitionKey, int buildsUpdated, int prsUpdated, string exceptionMessage, string exceptionStackTrace)
        {
            //Do some processing to extract some key properties from the json
            //JObject json = JObject.Parse(requestBody);
            //string resourceGroup = json["data"]["context"]["resourceGroupName"].ToString();
            //string dateString = json["data"]["context"]["timestamp"].ToString();

            //Set the properties
            PartitionKey = partitionKey;
            RowKey = DateTime.Now.ToString("yyyy-MMM-dd-HH-mm-ss.fff");
            BuildsUpdated = buildsUpdated;
            PRsUpdated = prsUpdated;
            ExceptionMessage = exceptionMessage;
            ExceptionStackTrace = exceptionStackTrace;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int BuildsUpdated { get; set; }
        public int PRsUpdated { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string Json 
        { 
            get
            {
                JObject newObject = new JObject(
                    new JProperty("BuildsUpdated", BuildsUpdated),
                    new JProperty("PRsUpdated", PRsUpdated),
                    new JProperty("ExceptionMessage", ExceptionMessage),
                    new JProperty("ExceptionStackTrace", ExceptionStackTrace)
                    );
                return newObject.ToString();
            }
        }

    }
}
