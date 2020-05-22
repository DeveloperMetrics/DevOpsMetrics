using DevOpsMetrics.Service.DataAccess.TableStorage;
using Microsoft.Azure.Cosmos.Table;

namespace DevOpsMetrics.Service.Models.Common
{
    public class AzureStorageTableModel : TableEntity
    {
        public AzureStorageTableModel(string partitionKey, string rowKey, string data)
        {
            TableStorageCommonDA common = new TableStorageCommonDA();
            PartitionKey = common.EncodePartitionKey(partitionKey);
            RowKey = rowKey;
            Data = data;
        }

        //TableEntity requires an empty constructor
        public AzureStorageTableModel()
        { }

        public string Data { get; set; }
    }
}
