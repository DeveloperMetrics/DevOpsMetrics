using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Models.GitHub
{
    public class GitHubRunsTableItem : TableEntity
    {
        public GitHubRunsTableItem(string partitionKey, string rowKey, string data)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Data = data;
        }

        //TableEntity requires an empty constructor
        public GitHubRunsTableItem()
        { }

        public string Data { get; set; }
    }
}
