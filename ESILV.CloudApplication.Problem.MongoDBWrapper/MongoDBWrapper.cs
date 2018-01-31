namespace ESILV.CloudApplication.Problem.MongoDBWrapper
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class MongoDBWrapper
    {
        #region Attributes
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;
        public MongoClient Client => _client; 
        #endregion
        #region Constructors
        public MongoDBWrapper()
        {
            Init(Constants.DATABASE_NAME, Constants.COLLECTION_NAME);
        }

        public MongoDBWrapper(string databaseName, string collectionName)
        {
            Init(databaseName, collectionName);
        }
        #endregion
        #region Utils
        /// <summary>
        /// Imports a csv in the collection
        /// WARNING : Might be irresponsive for some time with high number of 
        /// documents
        /// </summary>
        /// <param name="csvFile"></param>
        /// <param name="collectionName"></param>
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
        #endregion
        #region Accessors
        /// <summary>
        /// Returns the database matching the given name 
        /// </summary>
        /// <param name="dataBaseName"></param>
        /// <returns></returns>
        public IMongoDatabase GetDataBase(string dataBaseName)
        {
            return this._client.GetDatabase(dataBaseName);
        }
        /// <summary>
        /// Returns the collection matching the given name 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _database.GetCollection<BsonDocument>(collectionName);
        }
        #endregion
        #region Queries
        /// <summary>
        /// Gets the number of elements in the database
        /// </summary>
        /// <returns></returns>
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
        public List<BsonDocument> FirstQuery(int day)
        {
            IAggregateFluent<BsonDocument> aggregate = _collection.Aggregate()
                .Match(new BsonDocument { { "DayofMonth", day.ToString() } })
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
        public List<BsonDocument> SecondQuery(int day)
        {
            IAggregateFluent<BsonDocument> aggregate = _collection.Aggregate()
                .Match(new BsonDocument { { "DayofMonth", day.ToString() } })
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
	                if (flight.Month === """ + month + @""")
                    {
                        emit('Total Distance', Number(flight.Distance));
                    }
                }";

            string reduceFunction = "function (key, values) { return Array.sum(values);}";
            var options = new MapReduceOptions<BsonDocument, BsonDocument>() { OutputOptions = MapReduceOutputOptions.Inline };
            return _collection.MapReduce(mapFunction, reduceFunction, options).ToList();
        }
        /// <summary>
        /// Number of cancellation per type of problems for a month
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Types of problems causing cancellation per airport
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<BsonDocument> FifthQuery(string airportCityName)
        {
            string mapFunction = @"
                function ()
                {
                  var flight = this;
                  var regExp = new RegExp(/['""']+/g);
                if (parseFloat(flight.Cancelled) != 0 && flight.DestCityName.replace(regExp, '') == """ + airportCityName + @""")
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
        /// <summary>
        /// Number of cancellations due to bad weather for a year
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<BsonDocument> SixthQuery(int year)
        {
            string mapFunction = @"
                function ()
                {
                  var flight = this;
                  var regExp = new RegExp(/['""']+/g);
                if (parseFloat(flight.Cancelled) != 0 && flight.Year == """ + year + @""")
                {
                    var cleanedCode = flight.CancellationCode.replace(regExp, '');

                    switch (cleanedCode)
                    {
                        case ""B"":
                            emit(""Weather"", 1);
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
        /// <summary>
        /// Average delay in minutes for a destination airport and for a year.
        /// </summary>
        /// <param name="airportCityName"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<BsonDocument> SeventhQuery(string airportCityName, int year)
        {
            string mapFunction = @"function(){
                var flights = this;
                var regExp = new RegExp(/['""']+/g);
                if (flights.DestCityName.replace(regExp, '') == """ + airportCityName + @""" && flights.Year.replace(regExp,'') == """ + year + @""")
                    emit(flights.DestCityName.replace(regExp, ''), (Number(flights.DepDelayMinutes) + Number(flights.ArrDelayMinutes)));	
            }";
            string reduceFunction = "function (key, values) { return Array.avg(values);}";
            var options = new MapReduceOptions<BsonDocument, BsonDocument>() { OutputOptions = MapReduceOutputOptions.Inline };
            return _collection.MapReduce(mapFunction, reduceFunction, options).ToList();
        }
        /// <summary>
        /// Average taxi time for a given airport
        /// </summary>
        /// <param name="airportCityName"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<BsonDocument> EighthQuery(string airportCityName)
        {
            string mapFunction = @"function(){
                var flights = this;
                var regExp = new RegExp(/['""']+/g);
                if (flights.DestCityName.replace(regExp, '') == """ + airportCityName + @""")
                    emit(flights.DestCityName.replace(regExp, ''), (Number(flights.TaxiIn) + Number(flights.TaxiOut)));
            }";
            string reduceFunction = "function (key, values) { return Array.avg(values);}";
            var options = new MapReduceOptions<BsonDocument, BsonDocument>() { OutputOptions = MapReduceOutputOptions.Inline };
            return _collection.MapReduce(mapFunction, reduceFunction, options).ToList();
        }
        #endregion
    }
}
