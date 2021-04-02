using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Account
{
    public class Withdraw
    {
        [JsonProperty("amount")]
        public Decimal Amount { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("txId")]
        public string TxId { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("applyTime")]
        public long ApplyTime { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}
