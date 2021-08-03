using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StockSymbolsApi.Models;

namespace StockSymbolsApi.Services
{
    public class YahooFinanceService : IFinanceService
    {
        private readonly ILogger<YahooFinanceService> _logger;
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly string _apiHost;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Culture = CultureInfo.InvariantCulture,
        };

        public YahooFinanceService(ILogger<YahooFinanceService> logger, IConfiguration config)
        {
            _logger = logger;
            _client = new HttpClient();
            _apiKey = config["RapidApiKey"];
            _apiHost = config["RapidApiHost"];
        }

        /// <summary>
        /// Loads Historical data from Yahoo FinanceService. Not used for the main task.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="HistoricalData"/> object.</returns>
        public async Task<HistoricalData> GetHistoricalData(string symbol, string region)
        {
            string url = $"https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v3/get-historical-data?symbol={symbol}";
            if (!string.IsNullOrWhiteSpace(region))
            {
                url += $"&region={region}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    { "x-rapidapi-key", _apiKey },
                    { "x-rapidapi-host", _apiHost },
                },
            };

            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(body);
                
                var result = JsonConvert.DeserializeObject<HistoricalData>(body, _jsonSettings);
                return result;
            }
        }
        
        /// <summary>
        /// Loads Chart data from Yahoo FinanceService.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="Chart"/> object.</returns>
        public async Task<ChartResponse> GetChart(string symbol, string region, string interval, string range)
        {
            string url = $"https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-chart?symbol={symbol}&interval={interval}&range={range}";
            if (!string.IsNullOrWhiteSpace(region))
            {
                url += $"&region={region}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    { "x-rapidapi-key", _apiKey },
                    { "x-rapidapi-host", _apiHost },
                },
            };

            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(body);
                
                var result = JsonConvert.DeserializeObject<ChartResponse>(body, _jsonSettings);
                return result;
            }
        }
    }
}
