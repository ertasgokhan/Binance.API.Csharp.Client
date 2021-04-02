using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Account
{
    public class Deposit
    {
        [JsonProperty("insertTime")]
        public long InsertTime { get; set; }

        [JsonProperty("amount")]
        public Decimal Amount { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}
