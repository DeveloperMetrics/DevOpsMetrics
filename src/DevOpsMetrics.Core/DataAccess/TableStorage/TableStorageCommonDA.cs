using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsMetrics.Core.Models.Azure;
using DevOpsMetrics.Core.Models.Common;
using Microsoft.Azure.Cosmos.Table;

namespace DevOpsMetrics.Core.DataAccess.TableStorage
{
    public class TableStorageCommonDA
    {
        private readonly string ConfigurationString;
        private readonly string TableName;

        public TableStorageCommonDA(TableStorageConfiguration tableStorageConfig, string tableName)
        {
            ConfigurationString = tableStorageConfig.StorageAccountConnectionString;
            TableName = tableName;
        }

        public TableStorageCommonDA()
        {
        }

        private CloudTable CreateConnection()
        {
            //CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(AccountName, AccessKey), true);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Get a reference to a table named "items"
            CloudTable table = tableClient.GetTableReference(TableName);

            // Create the table if it doesn't exist
            table.CreateIfNotExists();

            return table;
        }

        public async Task<bool> AddItem(AzureStorageTableModel data, bool forceUpdate = false)
        {
            //Check if the item exists in storage
            AzureStorageTableModel item = await GetItem(data.PartitionKey, data.RowKey);
            if (item == null || forceUpdate == true)
            {
                await SaveItem(data);
                return true; //data saved to table!
            }
            else
            {
                return false; //no updates needed
            }
        }

        public async Task<AzureStorageTableModel> GetItem(string partitionKey, string rowKey)
        {
            //prepare the partition key
            partitionKey = EncodePartitionKey(partitionKey);

            CloudTable table = CreateConnection();

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<AzureStorageTableModel>(partitionKey, rowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            return (AzureStorageTableModel)retrievedResult.Result;
        }

        //This can't be async, because of how it queries the underlying data
        public List<AzureStorageTableModel> GetItems(string partitionKey)
        {
            partitionKey = EncodePartitionKey(partitionKey);

            CloudTable table = CreateConnection();

            // execute the query on the table
            List<AzureStorageTableModel> list = table.CreateQuery<AzureStorageTableModel>()
                                     .Where(ent => ent.PartitionKey == partitionKey)
                                     .ToList();

            return list;
        }

        public async Task<bool> SaveItem(AzureStorageTableModel data)
        {
            CloudTable table = CreateConnection();

            // Create the TableOperation that inserts/merges the entity.
            TableOperation operation = TableOperation.InsertOrMerge(data);
            await table.ExecuteAsync(operation);
            return true;
        }

        ////public async Task<bool> DeleteItem(string tableName, string name)
        ////{
        ////    CloudTable table = CreateConnection(tableName);

        ////    // Create a retrieve operation that expects a customer entity.
        ////    TableOperation retrieveOperation = TableOperation.Retrieve<T>("Item", name);

        ////    // Execute the operation.
        ////    TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

        ////    // Assign the result to a CustomerEntity object.
        ////    T deleteEntity = (T)retrievedResult.Result;

        ////    if (deleteEntity != null)
        ////    {
        ////        // Create the TableOperation that inserts the customer entity.
        ////        TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

        ////        // Execute the delete operation.
        ////        await table.ExecuteAsync(deleteOperation);
        ////    }
        ////    return true;
        ////}

        public string EncodePartitionKey(string text)
        {
            //The forward slash(/) character
            //The backslash(\) character
            //The number sign(#) character
            //The question mark (?) character
            text = text.Replace("/", "_");
            //text = text.Replace("\\", "_");
            //text = text.Replace("#", "_");
            //text = text.Replace("?", "_");

            ////Control characters from U+0000 to U+001F, including:
            ////The horizontal tab(\t) character
            //text = text.Replace("\t", "_");
            ////The linefeed(\n) character
            //text = text.Replace("\n", "_");
            ////The carriage return (\r) character
            //text = text.Replace("\r", "_");
            ////Control characters from U + 007F to U+009F

            return text;
        }

        //public string DecodePartitionKey(string text)
        //{
        //    return text.Replace("_", "/");
        //}

    }
}