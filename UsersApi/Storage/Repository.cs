using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using UsersApi.Util;

namespace UsersApi.Storage
{
    public class Repository : IRepository
    {
        private readonly ConcurrentDictionary<string, AsyncLazy<CloudTable>> _tables;
        private readonly CloudTableClient _client;

        public Repository(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _client = storageAccount.CreateCloudTableClient();

            _tables = new ConcurrentDictionary<string, AsyncLazy<CloudTable>>();
        }

        public async Task<T> GetEntityAsync<T>(StorageTablesNames tableName, string rowKey, string partitionKey) where T : class, ITableEntity
        {
            var cloudTable = await GetTable(tableName).Value;

            var retrieveTableOperation =
                TableOperation.Retrieve<T>(partitionKey, rowKey);
            var tableResult = await cloudTable.ExecuteAsync(retrieveTableOperation);

            return tableResult?.Result as T;
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>(StorageTablesNames tableName) where T : class, ITableEntity, new()
        {
            var cloudTable = await GetTable(tableName).Value;

            TableContinuationToken token = null;
            var entities = new List<T>();
            do
            {
                var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(new TableQuery<T>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        public async Task<int> InsertOrReplaceEntityAsync(StorageTablesNames tableName, ITableEntity tableEntity)
        {
            var cloudTable = await GetTable(tableName).Value;
            var insertOrReplaceTableOperation = TableOperation.InsertOrReplace(tableEntity);
            var tableResult = await cloudTable.ExecuteAsync(insertOrReplaceTableOperation);

            return tableResult.HttpStatusCode;
        }

        private AsyncLazy<CloudTable> GetTable(StorageTablesNames tableName)
        {
            var tableNameStr = Enum.GetName(typeof(StorageTablesNames), tableName);

            var cloudTable = new AsyncLazy<CloudTable>(async () =>
            {
                var table =
                    _client.GetTableReference(tableNameStr);
                await table.CreateIfNotExistsAsync();
                return table;
            });

            if (!_tables.ContainsKey(tableNameStr))
            {
                _tables[tableNameStr] = cloudTable;
            }

            return _tables[tableNameStr];
        }
    }
}
