using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockSymbolsApi.Models
{
    public class StockPerformance
    {
        public Dictionary<long, double> GivenPerformance { get; set; }
        public Dictionary<long, double> EtfPerformance { get; set; }
    }
}
