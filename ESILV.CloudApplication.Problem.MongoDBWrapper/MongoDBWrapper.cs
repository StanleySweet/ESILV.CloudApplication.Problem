namespace ESILV.CloudApplication.Problem.MongoDBWrapper
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

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

        public void ImportCsvIntoCollection(StreamReader csvFile, string collectionName)
        {
            string line = csvFile.ReadLine();
            string[] columnNames = Regex.Split(line, ";");
            while ((line = csvFile.ReadLine()) != null)
            {
                BsonDocument row = new BsonDocument();
                string[] cols = Regex.Split(line, ";");
                for (int i = 0; i < columnNames.Length; i++)
                {
                    row.Add(columnNames[i], cols[i]);
                }
                GetCollection(collectionName).InsertOne(row);
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


        /// <summary>
        /// Get the ten most visited destinations per day
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<BsonDocument> FirstQuery(int month)
        {
            IAggregateFluent<BsonDocument> aggregate = _collection.Aggregate()
                .Match(new BsonDocument { { "DayofMonth", month.ToString() } })
                .Group(new BsonDocument { { "_id", "$DestCityName" }, { "count", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "count", -1 } })
                .Limit(10);
            return aggregate.ToList();
        }

        /// <summary>
        /// Get the ten less visited destinations per day
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<BsonDocument> SecondQuery(int month)
        {
            IAggregateFluent<BsonDocument> aggregate = _collection.Aggregate()
                .Match(new BsonDocument { { "DayofMonth", month.ToString() } })
                .Group(new BsonDocument { { "_id", "$DestCityName" }, { "count", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "count", 1 } })
                .Limit(10);
            return aggregate.ToList();
        }

        /// <summary>
        /// Gets the total distance per month
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<BsonDocument> ThirdQuery(int month)
        {
            string mapFunction = @"
                function (){
	                var flight = this;
	                if (flight.Month === " + "\"" + month + "\"" + @")
                    {
                        emit('Total Distance', Number(flight.Distance));
                    }
                }";

            string reduceFunction = "function (key, values) { return Array.sum(values);}";
            var options = new MapReduceOptions<BsonDocument, BsonDocument>() { OutputOptions = MapReduceOutputOptions.Inline };
            return _collection.MapReduce(mapFunction, reduceFunction, options).ToList();
        }

        public List<BsonDocument> FourthQuery(int month)
        {
            string mapFunction = @"
                function ()
                {
                  var flight = this;
                  var regExp = new RegExp(/['""']+/g);
                if (parseFloat(flight.Cancelled) != 0 && Number(flight.Month) == " + month + @")
                {
                    var cleanedCode = flight.CancellationCode.replace(regExp, '');

                    switch (cleanedCode)
                    {
                        case ""A"":
                            emit(""Carrier"", 1);
                            break;
                        case ""B"":
                            emit(""Weather"", 1);
                            break;
                        case ""C"":
                            emit(""National Air System"", 1);
                            break;
                        case ""D"":
                            emit(""Security"", 1);
                            break;
                        default:
                            break;
                    }
                }
            }";

            string reduceFunction = "function (key, values) { return Array.sum(values);}";
            var options = new MapReduceOptions<BsonDocument, BsonDocument>() { OutputOptions = MapReduceOutputOptions.Inline };
            return _collection.MapReduce(mapFunction, reduceFunction, options).ToList();
        }
    }
}
