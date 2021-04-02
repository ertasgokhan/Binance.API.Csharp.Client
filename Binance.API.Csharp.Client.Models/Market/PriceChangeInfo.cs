using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Market
{
    public class PriceChangeInfo
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("priceChange")]
        public Decimal PriceChange { get; set; }

        [JsonProperty("priceChangePercent")]
        public Decimal PriceChangePercent { get; set; }

        [JsonProperty("weightedAvgPrice")]
        public Decimal WeightedAvgPrice { get; set; }

        [JsonProperty("prevClosePrice")]
        public Decimal PrevClosePrice { get; set; }

        [JsonProperty("lastPrice")]
        public Decimal LastPrice { get; set; }

        [JsonProperty("bidPrice")]
        public Decimal BidPrice { get; set; }

        [JsonProperty("askPrice")]
        public Decimal AskPrice { get; set; }

        [JsonProperty("openPrice")]
        public Decimal OpenPrice { get; set; }

        [JsonProperty("highPrice")]
        public Decimal HighPrice { get; set; }

        [JsonProperty("lowPrice")]
        public Decimal LowPrice { get; set; }

        [JsonProperty("volume")]
        public Decimal Volume { get; set; }

        [JsonProperty("openTime")]
        public long OpenTime { get; set; }

        [JsonProperty("closeTime")]
        public long CloseTime { get; set; }

        [JsonProperty("firstId")]
        public int FirstId { get; set; }

        [JsonProperty("lastId")]
        public int LastId { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
