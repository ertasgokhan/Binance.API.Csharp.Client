using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Market
{
    public class SymbolPrice
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("price")]
        public Decimal Price { get; set; }
    }
}
