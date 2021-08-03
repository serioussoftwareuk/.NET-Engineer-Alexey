using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockSymbolsApi.Helpers;
using StockSymbolsApi.Models;
using StockSymbolsApi.Services;

namespace StockSymbolsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockSymbolController : ControllerBase
    {
        private readonly ILogger<StockSymbolController> _logger;
        private readonly IFinanceService _financeService;
        private readonly IDbManager _dbManager;
        private readonly StockPerformanceCalculator _calculator;

        private const string EtfSymbol = "SPY";
        private const string ByDayRange = "5d";
        private const string ByHourRange = "1d";
        private const string ByDayInterval = "1d";
        private const string ByHourInterval = "60m";

        public StockSymbolController(ILogger<StockSymbolController> logger,
            IFinanceService financeService,
            IDbManager dbManager,
            StockPerformanceCalculator calculator)
        {
            _logger = logger;
            _financeService = financeService;
            _dbManager = dbManager;
            _calculator = calculator;
        }
        
        /// <summary>
        /// Loads Historical data from FinanceService. Not used for the main task.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="HistoricalData"/> JSON object.</returns>
        [HttpGet("historical")]
        public async Task<IActionResult> GetHistorical([System.Web.Http.FromUri] string symbol, [System.Web.Http.FromUri] string region)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest("Symbol parameter is required");
            }

            try
            {
                var result = await _financeService.GetHistoricalData(symbol, region);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("historical", ex);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Loads Chart data from FinanceService for 1 week and save it to DB.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="Chart"/> JSON object.</returns>
        [HttpGet("chartByDay")]
        public async Task<IActionResult> GetChartByDay([System.Web.Http.FromUri] string symbol, [System.Web.Http.FromUri] string region)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest("Symbol parameter is required");
            }

            try
            {
                var result = await FetchSymbol(symbol, ByDayInterval, ByDayRange, region);
                await FetchSymbol(EtfSymbol, ByDayInterval, ByDayRange, region);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("chartByDay", ex);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Loads Chart data from FinanceService for 1 day and save it to DB.
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="Chart"/> JSON object.</returns>
        [HttpGet("chartByHour")]
        public async Task<IActionResult> GetChartByHour([System.Web.Http.FromUri] string symbol, [System.Web.Http.FromUri] string region)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest("Symbol parameter is required");
            }

            try
            {
                var result = await FetchSymbol(symbol, ByHourInterval, ByHourRange, region);
                await FetchSymbol(EtfSymbol, ByDayInterval, ByDayRange, region);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("chartByHour", ex);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets cached Chart data list from DB for 1 week
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>Array of <see cref="Chart"/> JSON objects.</returns>
        [HttpGet("chartsCacheByDay")]
        public async Task<IActionResult> GetChartsFromCacheByDay([System.Web.Http.FromUri] string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest("Symbol parameter is required");
            }

            try
            {
                var result = await _dbManager.GetCharts(symbol, ByDayRange);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("chartsCacheByDay", ex);
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Gets cached Chart data list from DB for 1 day
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>Array of <see cref="Chart"/> JSON objects.</returns>
        [HttpGet("chartsCacheByHour")]
        public async Task<IActionResult> GetChartsFromCacheByHour([System.Web.Http.FromUri] string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest("Symbol parameter is required");
            }

            try
            {
                var result = await _dbManager.GetCharts(symbol, ByHourRange);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("chartsCacheByHour", ex);
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Calculates performance comparison for 1 week based on latest cached data from DB
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="StockPerformance"/> JSON object.</returns>
        [HttpGet("perfCompByDay")]
        public async Task<IActionResult> GetPerformanceComparisonByDay([System.Web.Http.FromUri] string symbol, [System.Web.Http.FromUri] bool takeDataFromCache = false)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest("Symbol parameter is required");
            }

            try
            {
                Chart givenSymbol;
                Chart etfSymbol;

                if (takeDataFromCache)
                {
                    givenSymbol = await _dbManager.GetLatestChart(symbol, ByDayRange);
                    etfSymbol = await _dbManager.GetLatestChart(EtfSymbol, ByDayRange);
                }
                else
                {
                    givenSymbol = await FetchSymbol(symbol, ByDayInterval, ByDayRange);
                    etfSymbol = await FetchSymbol(EtfSymbol, ByDayInterval, ByDayRange);
                }

                var result = _calculator.CalculatePerformanceComparison(givenSymbol, etfSymbol);
                
                // The line below is serialized using Newtonsoft JSON because a default serializer throws the following for such data:
                // System.NotSupportedException: The collection type 'System.Collections.Generic.Dictionary`2[System.Int64,System.Double]'
                return Ok(JsonConvert.SerializeObject(result));

                // Tried the following to resolve the issue above, but response content looks worse with this:
                //Response.Headers.Add("Content-Type", "application/json");
                //return new JsonResult(result, new JsonSerializerOptions{});
            }
            catch (Exception ex)
            {
                _logger.LogError("perfCompByDay", ex);
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Calculates performance comparison for 1 day based on latest cached data from DB
        /// </summary>
        /// <param name="symbol">Stock symbol</param>
        /// <returns><see cref="StockPerformance"/> JSON object.</returns>
        [HttpGet("perfCompByHour")]
        public async Task<IActionResult> GetPerformanceComparisonByHour([System.Web.Http.FromUri] string symbol, [System.Web.Http.FromUri] bool takeDataFromCache = false)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest("Symbol parameter is required");
            }

            try
            {
                Chart givenSymbol;
                Chart etfSymbol;

                if (takeDataFromCache)
                {
                    givenSymbol = await _dbManager.GetLatestChart(symbol, ByDayRange);
                    etfSymbol = await _dbManager.GetLatestChart(EtfSymbol, ByDayRange);
                }
                else
                {
                    givenSymbol = await FetchSymbol(symbol, ByDayInterval, ByDayRange);
                    etfSymbol = await FetchSymbol(EtfSymbol, ByDayInterval, ByDayRange);
                }

                var result = _calculator.CalculatePerformanceComparison(givenSymbol, etfSymbol);

                // The same issue as at perfCompByDay endpoint: 
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                _logger.LogError("perfCompByHour", ex);
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Fetch ETF symbol data and save it to DB cache in order to compare any given symbol with "SPY" later on.
        /// </summary>
        private async Task<Chart> FetchSymbol(string symbol, string interval, string range, string region = null)
        {
            var result = await _financeService.GetChart(symbol, region, ByDayInterval, ByDayRange);
            await _dbManager.SaveChart(result.Chart);

            return result.Chart;
        }
    }
}
