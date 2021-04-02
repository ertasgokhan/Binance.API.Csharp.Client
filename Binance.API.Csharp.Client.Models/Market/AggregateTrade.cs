using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Market
{
    public class AggregateTrade
    {
        [JsonProperty("a")]
        public int AggregateTradeId { get; set; }

        [JsonProperty("p")]
        public Decimal Price { get; set; }

        [JsonProperty("q")]
        public Decimal Quantity { get; set; }

        [JsonProperty("f")]
        public int FirstTradeId { get; set; }

        [JsonProperty("l")]
        public int LastTradeId { get; set; }

        [JsonProperty("T")]
        public long TimeStamp { get; set; }

        [JsonProperty("m")]
        public bool BuyerIsMaker { get; set; }

        [JsonProperty("M")]
        public bool BestPriceMatch { get; set; }
    }
}
