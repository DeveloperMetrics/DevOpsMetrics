using DevOpsMetrics.Service.Models.Common;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DevOpsMetrics.Service.DataAccess.TableStorage
{
    public class TableStorageCommonDA
    {
        private string AccountName;
        private string AccessKey;
        private string TableName;

        public TableStorageCommonDA(string accountName, string accessKey, string tableName)
        {
            AccountName = accountName;
            AccessKey = accessKey;
            TableName = tableName;
        }

        public TableStorageCommonDA()
        { }

        private CloudTable CreateConnection()
        {
            //string name = accountName; // Configuration["accountName"];
            //string accessKey = accessKey; // Configuration["accessKey"];
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(AccountName, AccessKey), true);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Get a reference to a table named "items"
            CloudTable table = tableClient.GetTableReference(TableName);

            return table;
        }

        public async Task<bool> AddItem(AzureStorageTableModel data)
        {
            //Check if the item exists in storage
            AzureStorageTableModel item = await GetItem(data.PartitionKey, data.RowKey);
            if (item == null)
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
        public List<AzureStorageTableModel> GetItems(string partitionKeyFilter)
        {
            CloudTable table = CreateConnection();

            // execute the query on the table
            List<AzureStorageTableModel> list = table.CreateQuery<AzureStorageTableModel>()
                                     .Where(ent => ent.PartitionKey == partitionKeyFilter)
                                     .ToList();

            return list;
        }

        public async Task<bool> SaveItem(AzureStorageTableModel data)
        {
            CloudTable table = CreateConnection();

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.InsertOrMerge(data);

            // Execute the insert operation.
            await table.ExecuteAsync(insertOperation);
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
            text = text.Replace("/", "_");

            //The forward slash(/) character
            //The backslash(\) character
            //The number sign(#) character
            //The question mark (?) character

            //Control characters from U+0000 to U+001F, including:
            //The horizontal tab(\t) character
            //The linefeed(\n) character
            //The carriage return (\r) character
            //Control characters from U + 007F to U+009F

            return text.Replace("/", "_");
        }

        public string DecodePartitionKey(string text)
        {
            return text.Replace("_", "/");
        }

    }
}