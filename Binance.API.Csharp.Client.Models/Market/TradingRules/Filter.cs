using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Market.TradingRules
{
    public class Filter
    {
        [JsonProperty("filterType")]
        public string FilterType { get; set; }

        [JsonProperty("minPrice")]
        public Decimal MinPrice { get; set; }

        [JsonProperty("maxPrice")]
        public Decimal MaxPrice { get; set; }

        [JsonProperty("tickSize")]
        public Decimal TickSize { get; set; }

        [JsonProperty("minQty")]
        public Decimal MinQty { get; set; }

        [JsonProperty("maxQty")]
        public Decimal MaxQty { get; set; }

        [JsonProperty("stepSize")]
        public Decimal StepSize { get; set; }

        [JsonProperty("minNotional")]
        public Decimal MinNotional { get; set; }
    }
}
