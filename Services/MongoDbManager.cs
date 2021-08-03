using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using StockSymbolsApi.Models;

namespace StockSymbolsApi.Services
{
    public class MongoDbManager : IDbManager
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Chart> _chartsCollection;
        private readonly string _mongoUri;
        private readonly int? DbFetchLimit = 100;
        private const string DbName = "stock";

        public MongoDbManager(IConfiguration config)
        {
            _mongoUri = config["MongoConnectionString"];
            _client = new MongoClient(_mongoUri);
            _db = _client.GetDatabase(DbName);
            _chartsCollection = _db.GetCollection<Chart>("charts");
        }

        /// <summary>
        /// Insert a Chart data document into Mongo DB.
        /// </summary>
        /// <param name="chart"><see cref="Chart"/> objects</param>
        public Task SaveChart(Chart chart)
        {
            return _chartsCollection.InsertOneAsync(chart);
        }

        /// <summary>
        /// Insert many Chart data documents into Mongo DB.
        /// </summary>
        /// <param name="charts">A list of <see cref="Chart"/> objects</param>
        public Task SaveCharts(List<Chart> charts)
        {
            return _chartsCollection.InsertManyAsync(charts);
        }

        /// <summary>
        /// Fetch list of Chart data documents from Mongo DB.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>List of <see cref="Chart"/> objects.</returns>
        public async Task<List<Chart>> GetCharts(string symbol, string range)
        {
            var filter = new FilterDefinitionBuilder<Chart>()
                .Where(ch => ch.Result.Any(r => r.Meta.Symbol == symbol) && ch.Result.Any(r => r.Meta.Range == range));

            var charts = await _chartsCollection.Find(filter).Limit(DbFetchLimit).Sort("{ $natural: -1 }").ToListAsync();
            return charts;
        }

        /// <summary>
        /// Fetch latest Chart data document from Mongo DB.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="Chart"/> objects.</returns>
        public async Task<Chart> GetLatestChart(string symbol, string range)
        {
            var filter = new FilterDefinitionBuilder<Chart>()
                .Where(ch => ch.Result.Any(r => r.Meta.Symbol == symbol) && ch.Result.Any(r => r.Meta.Range == range));

            var charts = await _chartsCollection.Find(filter).Limit(1).Sort("{ $natural: -1 }").ToListAsync();
            return charts.FirstOrDefault();
        }

    }
}
