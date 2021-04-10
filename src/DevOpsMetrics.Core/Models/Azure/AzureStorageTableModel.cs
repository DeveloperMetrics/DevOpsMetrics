using DevOpsMetrics.Core.DataAccess.TableStorage;
using Microsoft.Azure.Cosmos.Table;

namespace DevOpsMetrics.Core.Models.Azure
{
    public class AzureStorageTableModel : TableEntity
    {
        public AzureStorageTableModel(string partitionKey, string rowKey, string data)
        {
            TableStorageCommonDA common = new TableStorageCommonDA();
            PartitionKey = common.EncodePartitionKey(partitionKey);
            RowKey = common.EncodePartitionKey(rowKey);
            Data = data;
        }

        //TableEntity requires an empty constructor
        public AzureStorageTableModel()
        { }

        public string Data { get; set; }
    }
}
