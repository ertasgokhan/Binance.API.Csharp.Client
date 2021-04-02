using System;

namespace Binance.API.Csharp.Client.Models.Market
{
    public class Candlestick
    {
        public long OpenTime { get; set; }

        public Decimal Open { get; set; }

        public Decimal High { get; set; }

        public Decimal Low { get; set; }

        public Decimal Close { get; set; }

        public Decimal Volume { get; set; }

        public long CloseTime { get; set; }

        public Decimal QuoteAssetVolume { get; set; }

        public int NumberOfTrades { get; set; }

        public Decimal TakerBuyBaseAssetVolume { get; set; }

        public Decimal TakerBuyQuoteAssetVolume { get; set; }

        public DateTime OpenDateTime { get; set; }
        public DateTime CloseDateTime { get; set; }

    }
}
