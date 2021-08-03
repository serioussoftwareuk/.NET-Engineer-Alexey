using System;
using System.Collections.Generic;
using System.Linq;
using StockSymbolsApi.Models;

namespace StockSymbolsApi.Helpers
{
    public class StockPerformanceCalculator
    {
        /// <summary>
        /// Calculates performance comparison for two stock symbols
        /// </summary>
        /// <param name="givenSymbol">Given stock symbol (<see cref="Chart"/> object)</param>
        /// <param name="etfSymbol">ETF stock symbol (<see cref="Chart"/> object)</param>
        /// <returns>Returns <see cref="StockPerformance"/> object containing two Dictionaries of timestamp-performance pairs.</returns>
        public StockPerformance CalculatePerformanceComparison(Chart givenSymbol, Chart etfSymbol)
        {
            return new StockPerformance
            {
                GivenPerformance = Calculate(givenSymbol),
                EtfPerformance = Calculate(etfSymbol),
            };
        }

        private Dictionary<long, double> Calculate(Chart symbol)
        {
            var performance = new Dictionary<long, double>();

            var timestamps = symbol?.Result?.FirstOrDefault()?.Timestamp;
            var prices = symbol?.Result?.FirstOrDefault()?.Indicators?.Quote?.FirstOrDefault()?.Close;

            if (timestamps?.Any() != true || prices?.Any() != true)
            {
                performance.Add(0, 0);
                return performance;
            }
            
            int counter = 0;
            double firstPrice = 0;

            foreach (var time in timestamps)
            {
                if (counter == 0)
                {
                    firstPrice = prices.FirstOrDefault();
                }

                var comparison = prices.Count > counter ? (((prices[counter] - firstPrice) / firstPrice) * 100) : 0;

                performance.Add(time, comparison);
                counter++;
            }

            return performance;
        }
    }
}
