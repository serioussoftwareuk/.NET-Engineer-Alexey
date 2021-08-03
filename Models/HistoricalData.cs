using System;
using System.Collections.Generic;
using StockSymbolsApi.Extensions;

namespace StockSymbolsApi.Models
{
    public class HistoricalData
    {
        public List<StockPrice> Prices { get; set; }
        public List<StockEventData> EventsData { get; set; }
        public bool IsPending { get; set; }
        public string Id { get; set; }
        public long FirstTradeDate { get; set; }
        public DateTime FirstTradeDateConverted => DateTimeConverter.FromUnixEpoch(FirstTradeDate);
    }

    public class StockPrice
    {
        public long DateTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public double AdjustedClose { get; set; }
        public DateTime DateTimeConverted => DateTimeConverter.FromUnixEpoch(DateTime);
    }

    public class StockEventData
    {
        public long Date { get; set; }
        public decimal Amount { get; set; }
        public decimal Data { get; set; }
        public string Type { get; set; }
        public DateTime DateConverted => DateTimeConverter.FromUnixEpoch(Date);
    }

}
