using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Market
{
    public class Balance
    {
        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("free")]
        public Decimal Free { get; set; }

        [JsonProperty("locked")]
        public Decimal Locked { get; set; }
    }
}
