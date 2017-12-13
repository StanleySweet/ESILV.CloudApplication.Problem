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
    public class Wrapper
    {
        private MongoClient _client;
        private IMongoDatabase _database;

        public Wrapper()
        {
            _client = new MongoClient(Constants.CONNECTION_STRING);
        }

        public void Connect(string dbName)
        {
            _database = _client.GetDatabase(dbName);
            var collection = _database.GetCollection<BsonDocument>("tokens");

            Task t = queryDatabase(collection);
            t.ContinueWith((str) =>
            {
                Console.WriteLine(str.Status.ToString());
                Console.WriteLine("Query Ends.");
            });
            t.Wait();
            Console.ReadKey();
        }

        public async static Task<string> queryDatabase(IMongoCollection<BsonDocument> collection)
        {
            Console.WriteLine("Query Starts...");

            IAggregateFluent<BsonDocument> aggregate = collection.Aggregate()
                .Group(new BsonDocument { { "_id", "$token" }, { "count", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "count", -1 } })
                .Limit(10);
            List<BsonDocument> results = await aggregate.ToListAsync();
            foreach (BsonDocument obj in results)
            {
                Console.WriteLine(obj.ToString());
            }
            return "finished";
        }


        public void Disconnect()
        {

        }

    }
}
