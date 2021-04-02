using Newtonsoft.Json;
using System;

namespace Binance.API.Csharp.Client.Models.WebSocket
{
    public class KlineMessage
    {
        [JsonProperty("e")]
        public string EventType { get; set; }

        [JsonProperty("E")]
        public long EventTime { get; set; }

        [JsonProperty("s")]
        public string Symbol { get; set; }

        [JsonProperty("k")]
        public KlineMessage.KlineData KlineInfo { get; set; }

        public class KlineData
        {
            [JsonProperty("t")]
            public long StartTime { get; set; }

            [JsonProperty("T")]
            public long EndTime { get; set; }

            [JsonProperty("s")]
            public string Symbol { get; set; }

            [JsonProperty("i")]
            public string Interval { get; set; }

            [JsonProperty("f")]
            public int FirstTradeId { get; set; }

            [JsonProperty("L")]
            public int LastTradeId { get; set; }

            [JsonProperty("o")]
            public Decimal Open { get; set; }

            [JsonProperty("c")]
            public Decimal Close { get; set; }

            [JsonProperty("h")]
            public Decimal High { get; set; }

            [JsonProperty("l")]
            public Decimal Low { get; set; }

            [JsonProperty("v")]
            public Decimal Volume { get; set; }

            [JsonProperty("n")]
            public int NumberOfTrades { get; set; }

            [JsonProperty("x")]
            public bool IsFinal { get; set; }

            [JsonProperty("q")]
            public Decimal QuoteVolume { get; set; }

            [JsonProperty("V")]
            public Decimal ActiveBuyVolume { get; set; }

            [JsonProperty("Q")]
            public Decimal ActiveBuyQuoteVolume { get; set; }
        }
    }
}
