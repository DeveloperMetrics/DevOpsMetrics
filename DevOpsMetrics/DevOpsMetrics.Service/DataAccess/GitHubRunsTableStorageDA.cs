using DevOpsMetrics.Service.Models.GitHub;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public class GitHubRunsTableStorageDA 
    {

        private CloudTable CreateConnection(string accountName, string accessKey, string tableName)
        {
            //string name = accountName; // Configuration["accountName"];
            //string accessKey = accessKey; // Configuration["accessKey"];
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accessKey), true);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Get a reference to a table named "items"
            CloudTable itemsTable = tableClient.GetTableReference(tableName);

            return itemsTable;
        }

        public async Task<bool> AddItem(string accountName, string accessKey, string tableName, GitHubRunsTableItem data)
        {
            //Check if the item exists in storage
            GitHubRunsTableItem item = await GetItem(accountName, accessKey, tableName, data.PartitionKey, data.RowKey);
            if (item == null)
            {
                await SaveItem(accountName, accessKey, tableName, data);
                return true; //data saved to table!
            }
            else
            {
                return false; //no updates needed
            }
        }

        public async Task<GitHubRunsTableItem> GetItem(string accountName, string accessKey, string tableName, string partitionKey, string rowKey)
        {
            //prepare the partition key
            partitionKey = Utility.EncodePartitionKey(partitionKey);

            CloudTable itemsTable = CreateConnection(accountName, accessKey, tableName);

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<GitHubRunsTableItem>(partitionKey, rowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = await itemsTable.ExecuteAsync(retrieveOperation);

            return (GitHubRunsTableItem)retrievedResult.Result;
        }

        public async Task<bool> SaveItem(string accountName, string accessKey, string tableName, GitHubRunsTableItem data)
        {
            CloudTable itemsTable = CreateConnection(accountName, accessKey, tableName);

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.InsertOrMerge(data);

            // Execute the insert operation.
            await itemsTable.ExecuteAsync(insertOperation);
            return true;
        }

        //public async Task<IEnumerable<T>> GetItems(string tableName)
        //{
        //    CloudTable itemsTable = CreateConnection(tableName);

        //    // Construct the query operation for all customer entities where PartitionKey="Smith".
        //    TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Item"));

        //    // execute the query on the table
        //    TableQuerySegment<T> resultSegment = await itemsTable.ExecuteQuerySegmentedAsync<T>(query, null);

        //    //Convert the array into a list and sort by Name
        //    List<T> results = resultSegment.Results.ToList<T>();
        //    results.Sort((x, y) => x.Name.CompareTo(y.Name));

        //    return results;
        //}

        ////public async Task<bool> DeleteItem(string tableName, string name)
        ////{
        ////    CloudTable itemsTable = CreateConnection(tableName);

        ////    // Create a retrieve operation that expects a customer entity.
        ////    TableOperation retrieveOperation = TableOperation.Retrieve<T>("Item", name);

        ////    // Execute the operation.
        ////    TableResult retrievedResult = await itemsTable.ExecuteAsync(retrieveOperation);

        ////    // Assign the result to a CustomerEntity object.
        ////    T deleteEntity = (T)retrievedResult.Result;

        ////    if (deleteEntity != null)
        ////    {
        ////        // Create the TableOperation that inserts the customer entity.
        ////        TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

        ////        // Execute the delete operation.
        ////        await itemsTable.ExecuteAsync(deleteOperation);
        ////    }
        ////    return true;
        ////}

    }
}