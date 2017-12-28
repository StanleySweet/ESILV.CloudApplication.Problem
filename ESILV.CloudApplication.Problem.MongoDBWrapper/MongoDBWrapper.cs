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

        public MongoDBWrapper()
        {
            _client = new MongoClient(Constants.CONNECTION_STRING);
        }

        public void Connect(string dbName)
        {
            _database = _client.GetDatabase(dbName);
            _collection = _database.GetCollection<BsonDocument>("tokens");

            //Task t = QueryDatabase(collection);
            //t.ContinueWith((str) =>
            //{
            //    Console.WriteLine(str.Status.ToString());
            //    Console.WriteLine("Query Ends.");
            //});
            //t.Wait();
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


        public void Disconnect()
        {

        }

    }
}
