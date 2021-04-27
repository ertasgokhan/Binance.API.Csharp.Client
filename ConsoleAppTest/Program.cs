using Binance.Generate.OTT;
using Binance.OTT.Trade;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace ConsoleAppTest
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            await GenerateOTTLine.GenerateOTT(@"COMMON\");

            await BinanceTrade.TradeAsync(@"COMMON\");

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime.Subtract(startTime);
            Console.WriteLine("Time Difference {0}:{1}:{2} ", span.Hours, span.Minutes, span.Seconds);
            Console.ReadLine();
        }
    }
}
