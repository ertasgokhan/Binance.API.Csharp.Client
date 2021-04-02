using Newtonsoft.Json;
using System.Collections.Generic;

namespace Binance.API.Csharp.Client.Models.Account
{
    public class WithdrawHistory
    {
        [JsonProperty("withdrawList")]
        public IEnumerable<Deposit> WithdrawList { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
