using System;
using Azure;
using Azure.Data.Tables;
using DevOpsMetrics.Core.DataAccess.TableStorage;

namespace DevOpsMetrics.Core.Models.Azure
{
    public class AzureStorageTableModel : ITableEntity
    {
        public AzureStorageTableModel(string partitionKey, string rowKey, string data)
        {
            PartitionKey = TableStorageCommonDA.EncodePartitionKey(partitionKey);
            RowKey = TableStorageCommonDA.EncodePartitionKey(rowKey);
            Data = data;
        }

        //TableEntity requires an empty constructor
        public AzureStorageTableModel()
        {
        }

        public string Data
        {
            get; set;
        }
        public string PartitionKey
        {
            get; set;
        }
        public string RowKey
        {
            get; set;
        }
        public DateTimeOffset? Timestamp
        {
            get; set;
        }
        public ETag ETag
        {
            get; set;
        }
    }
}
