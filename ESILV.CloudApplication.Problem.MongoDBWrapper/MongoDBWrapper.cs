namespace ESILV.CloudApplication.Problem.MongoDBWrapper
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
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

        public void ImportCsvIntoCollection(StreamReader csvFile)
        {
            string line = csvFile.ReadLine();
            string[] columnNames = Regex.Split(line, ",");
            while ((line = csvFile.ReadLine()) != null)
            {
                BsonDocument row = new BsonDocument();
                string[] cols = Regex.Split(line, ",");
                for (int i = 0; i < columnNames.Length; i++)
                {
                    row.Add(columnNames[i], cols[i]);
                }
                _collection.InsertOne(row);
            }
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

        public List<BsonDocument> CountLines()
        {
            IAggregateFluent<BsonDocument> aggregate = _collection.Aggregate()
                .Group(new BsonDocument { { "_id", "$token" }, { "count", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "count", -1 } })
                .Limit(10);
            List<BsonDocument> results = aggregate.ToList();
            foreach (BsonDocument obj in results)
            {
                Console.WriteLine(obj.ToString());
            }
            return results;
        }

        public List<BsonDocument> FirstQuery()
        {
            return new List<BsonDocument>();
        }
    }
}
