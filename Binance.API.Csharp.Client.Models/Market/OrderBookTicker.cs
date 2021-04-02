using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Market
{
    public class OrderBookTicker
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("bidPrice")]
        public Decimal BidPrice { get; set; }

        [JsonProperty("bidQty")]
        public Decimal BidQuantity { get; set; }

        [JsonProperty("askPrice")]
        public Decimal AskPrice { get; set; }

        [JsonProperty("askQty")]
        public Decimal AskQuantity { get; set; }
    }
}
