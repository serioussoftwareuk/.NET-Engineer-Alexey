using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockSymbolsApi.Models;

namespace StockSymbolsApi.Services
{
    public interface IDbManager
    {
        Task SaveChart(Chart chart);
        Task SaveCharts(List<Chart> charts);
        Task<List<Chart>> GetCharts(string symbol, string range);
        Task<Chart> GetLatestChart(string symbol, string range);
    }
}
