using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.WebSocket
{
    public class Balance
    {
        [JsonProperty("a")]
        public string Asset { get; set; }

        [JsonProperty("f")]
        public Decimal Free { get; set; }

        [JsonProperty("l")]
        public Decimal Locked { get; set; }
    }
}
