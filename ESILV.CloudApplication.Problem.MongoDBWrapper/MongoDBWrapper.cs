using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESILV.CloudApplication.Problem.MongoDBWrapper
{
    public class MongoDBWrapper
    {
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;

        public MongoClient Client => _client;

        public MongoDBWrapper()
        {
            Init(Constants.DATABASE_NAME, Constants.COLLECTION_NAME);
        }

        public MongoDBWrapper(string databaseName, string collectionName)
        {
            Init(databaseName, collectionName);
        }

        public void Init(string databaseName, string collectionName)
        {
            _client = new MongoClient(Constants.CONNECTION_STRING + "/" + databaseName);
            _database = GetDataBase(databaseName);
            _collection = GetCollection(collectionName);
        }

        public IMongoDatabase GetDataBase(string dataBaseName)
        {
            return this._client.GetDatabase(dataBaseName);
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task<List<BsonDocument>> QueryDatabase()
        {
            IAggregateFluent<BsonDocument> aggregate = _collection.Aggregate()
                .Group(new BsonDocument { { "_id", "$token" }, { "count", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "count", -1 } })
                .Limit(10);
            List<BsonDocument> results = await aggregate.ToListAsync();
            foreach (BsonDocument obj in results)
            {
                Console.WriteLine(obj.ToString());
            }
            return results;
        }
    }
}
