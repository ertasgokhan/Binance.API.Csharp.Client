using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.Generate.OTT
{
    public class Symbol
    {
        public string symbol { get; set; }

        public int length { get; set; }

        public decimal percent { get; set; }

        public int pastDataLength { get; set; }
    }
}
