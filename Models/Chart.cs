using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using StockSymbolsApi.Extensions;

namespace StockSymbolsApi.Models
{
    public class ChartResponse
    {
        public Chart Chart { get; set; }
    }

    public class Chart
    {
        public ObjectId Id { get; set; }    // MongoDB key
        public List<ChartResult> Result { get; set; }
        public string Error { get; set; }
    }

    public class ChartResult
    {
        public List<long> Timestamp { get; set; }
        public ChartIndicator Indicators { get; set; }
        public ChartMeta Meta { get; set; }
    }
    
    public class ChartIndicator
    {
        public List<ChartQuote> Quote { get; set; }
    }

    public class ChartQuote
    {
        public List<double> Open { get; set; }
        public List<double> High { get; set; }
        public List<double> Low { get; set; }
        public List<double> Close { get; set; }
        public List<long> Volume { get; set; }
    }

    public class ChartMeta
    {
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public string ExchangeName { get; set; }
        public string InstrumentType { get; set; }
        public long FirstTradeDate { get; set; }
        public long RegularMarketTime { get; set; }
        public long Gmtoffset { get; set; }
        public string Timezone { get; set; }
        public string ExchangeTimezoneName { get; set; }
        public double RegularMarketPrice { get; set; }
        public double ChartPreviousClose { get; set; }
        public double PreviousClose { get; set; }
        public int Scale { get; set; }
        public int PriceHint { get; set; }
        public string DataGranularity { get; set; }
        public string Range { get; set; }
        public List<string> ValidRanges { get; set; }
        public ChartTradingPeriod CurrentTradingPeriod { get; set; }
        public ChartTradingPeriods TradingPeriods { get; set; }
    }

    public class ChartTradingPeriod
    {
        public ChartPeriod Pre { get; set; }
        public ChartPeriod Regular { get; set; }
        public ChartPeriod Post { get; set; }
    }

    public class ChartTradingPeriods
    {
        public List<List<ChartPeriod>> Pre { get; set; }
        public List<List<ChartPeriod>> Regular { get; set; }
        public List<List<ChartPeriod>> Post { get; set; }
    }

    public class ChartPeriod
    {
        public string Timezone { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public long Gmtoffset { get; set; }
        [BsonIgnore]
        public DateTime EndDate => DateTimeConverter.FromUnixEpoch(End);
    }

}
