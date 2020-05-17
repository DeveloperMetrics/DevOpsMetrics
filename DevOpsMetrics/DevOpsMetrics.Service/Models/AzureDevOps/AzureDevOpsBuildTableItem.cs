using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models.AzureDevOps
{
    public class AzureDevOpsBuildTableItem : TableEntity
    {
        public AzureDevOpsBuildTableItem(string partitionKey, string rowKey, string data)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Data = data;
        }

        public string Data { get; set; }
    }
}
