using System;
using System.Threading.Tasks;
using StockSymbolsApi.Models;

namespace StockSymbolsApi.Services
{
    public interface IFinanceService
    {
        Task<HistoricalData> GetHistoricalData(string symbol, string region);
        Task<ChartResponse> GetChart(string symbol, string region, string interval, string range);
    }
}
