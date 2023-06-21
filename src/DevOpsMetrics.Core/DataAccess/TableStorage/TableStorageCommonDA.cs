using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using DevOpsMetrics.Core.Models.Azure;

namespace DevOpsMetrics.Core.DataAccess.TableStorage
{
    public class TableStorageCommonDA
    {
        private readonly string ConfigurationString;
        private readonly string TableName;

        public TableStorageCommonDA(string tableConnectionString, string tableName)
        {
            ConfigurationString = tableConnectionString;
            TableName = tableName;
        }

        public TableStorageCommonDA()
        {
        }

        private TableClient CreateConnection()
        {
            // Create a TableServiceClient for service level operations
            TableServiceClient serviceClient = new(ConfigurationString);

            // Create the table if it doesn't exist
            TableClient tableClient = serviceClient.GetTableClient(TableName);

            // Create the table if it doesn't exist.
            tableClient.CreateIfNotExists();

            return tableClient;
        }

        public async Task<bool> AddItem(AzureStorageTableModel data, bool forceUpdate = false)
        {
            //Check if the item exists in storage
            AzureStorageTableModel item = await GetItem(data.PartitionKey, data.RowKey);
            if (item == null || forceUpdate)
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

            TableClient tableClient = CreateConnection();

            // Get the entity.
            NullableResponse<AzureStorageTableModel> temp = await tableClient.GetEntityIfExistsAsync<AzureStorageTableModel>(partitionKey, rowKey);
            if (temp.HasValue)
            {
                return await tableClient.GetEntityAsync<AzureStorageTableModel>(partitionKey, rowKey);
            }
            else
            {
                return null;
            }
        }

        //This can't be async, because of how it queries the underlying data
        public async Task<List<AzureStorageTableModel>> GetItems(string partitionKey)
        {
            partitionKey = EncodePartitionKey(partitionKey);

            TableClient tableClient = CreateConnection();

            AsyncPageable<AzureStorageTableModel> results = tableClient.QueryAsync<AzureStorageTableModel>(e => e.PartitionKey == partitionKey);
            List<AzureStorageTableModel> list = await results.ToListAsync();
            return list;
        }

        public async Task<bool> SaveItem(AzureStorageTableModel data)
        {
            TableClient tableClient = CreateConnection();
            await tableClient.UpsertEntityAsync(data);

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

        public static string EncodePartitionKey(string text)
        {
            if (string.IsNullOrEmpty(text) == false)
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
            }

            return text;
        }

        //public string DecodePartitionKey(string text)
        //{
        //    return text.Replace("_", "/");
        //}

    }
}