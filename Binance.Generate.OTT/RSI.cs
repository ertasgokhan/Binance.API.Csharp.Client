using System;
using System.Collections.Generic;
using System.Text;

namespace Binance.Generate.OTT
{
    public class RSI
    {
        public decimal avgGain { get; set; }
        public decimal avgLosses { get; set; }
        public decimal relativeStrength { get; set; }
        public decimal rsi { get; set; }
    }
}
