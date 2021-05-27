using System;
using System.Collections.Generic;
using System.Text;

namespace Binance.OTT.Trade
{
    public class Candlestick
    {
        public string Symbol { get; set; }

        public DateTime OpenDateTime { get; set; }

        public Decimal Open { get; set; }

        public Decimal High { get; set; }

        public Decimal Low { get; set; }

        public Decimal Close { get; set; }

        public Decimal SupportLine { get; set; }

        public Decimal OTTLine { get; set; }

        public bool BuySignal { get; set; }

        public bool SellSignal { get; set; }

        public Decimal rsi { get; set; }

    }
}
