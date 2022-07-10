using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOpsMetrics.Core.Models.Common
{
    public class ProjectLog
    {
        public ProjectLog()
        {
        } //Note this parameter-less function is required for JSON serialization

        public ProjectLog(string partitionKey,
            int buildsUpdated, int prsUpdated,
            string buildUrl, string prUrl,
            string exceptionMessage, string exceptionStackTrace)
        {
            //Set the properties
            PartitionKey = partitionKey;
            RowKey = DateTime.Now.ToString("yyyy-MMM-dd-HH-mm-ss.fff");
            BuildsUpdated = buildsUpdated;
            PRsUpdated = prsUpdated;
            BuildUrl = buildUrl;
            PRUrl = prUrl;
            ExceptionMessage = exceptionMessage;
            ExceptionStackTrace = exceptionStackTrace;
        }

        public string PartitionKey
        {
            get; set;
        }
        public string RowKey
        {
            get; set;
        }
        public int BuildsUpdated
        {
            get; set;
        }
        public int PRsUpdated
        {
            get; set;
        }
        public string BuildUrl
        {
            get; set;
        }
        public string PRUrl
        {
            get; set;
        }
        public string ExceptionMessage
        {
            get; set;
        }
        public string ExceptionStackTrace
        {
            get; set;
        }
        private string data;
        public string Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                if (data != null)
                {
                    dynamic json = JsonConvert.DeserializeObject(data);
                    if (json != null)
                    {
                        BuildsUpdated = json["BuildsUpdated"];
                        PRsUpdated = json["PRsUpdated"];
                        ExceptionMessage = json["ExceptionMessage"];
                        ExceptionStackTrace = json["ExceptionStackTrace"];
                    }
                }
            }
        }
        public string Json
        {
            get
            {
                JObject jsonObject = new(
                    new JProperty("BuildsUpdated", BuildsUpdated),
                    new JProperty("PRsUpdated", PRsUpdated),
                    new JProperty("ExceptionMessage", ExceptionMessage),
                    new JProperty("ExceptionStackTrace", ExceptionStackTrace)
                    );
                return jsonObject.ToString();
            }
        }

    }
}
