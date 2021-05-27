using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.OTT.Trade
{
    public class Symbol
    {
        public string symbol { get; set; }

        public string symbolCoin { get; set; }

        public int length { get; set; }

        public decimal percent { get; set; }

        public decimal buyRatio { get; set; }

        public decimal sellRatio { get; set; }

        public bool buyTrendSwitch { get; set; }

        public int reviewPeriodLength { get; set; }

        public decimal profitRatio { get; set; }

        public decimal depositRatio { get; set; }

        public decimal availableAmount { get; set; }

        public int quantityRound { get; set; }

        public int priceRound { get; set; }

        public int rsiSupport { get; set; }

    }
}
