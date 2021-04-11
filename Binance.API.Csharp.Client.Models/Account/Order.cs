using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.Account
{

    public class Order
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("price")]
        public Decimal Price { get; set; }

        [JsonProperty("origQty")]
        public Decimal OrigQty { get; set; }

        [JsonProperty("executedQty")]
        public Decimal ExecutedQty { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("stopPrice")]
        public Decimal StopPrice { get; set; }

        [JsonProperty("icebergQty")]
        public Decimal IcebergQty { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }
    }
}
