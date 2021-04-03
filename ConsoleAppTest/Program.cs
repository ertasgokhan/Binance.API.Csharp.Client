using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleAppTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var apiClient = new ApiClient("srhEOc1oqMt4euGiUeVBseXk588iBD4mFUD0k3VcFQQiQdRlA1NvVxVY2x0weXej", "obd4UryGMEKgdvb9B84bKGrXxusQUEQ8nYFUba85xst02dq7FNRvdFMNZtze9RDj");
            var binanceClient = new BinanceClient(apiClient);
            string symbol = "grtusdt";
            IEnumerable<Candlestick> candlestick = binanceClient.GetCandleSticks(symbol, TimeInterval.Hours_1, DateTime.Now.AddDays(-90), DateTime.Now).Result;
            // var tickerPrices = binanceClient.GetAllPrices().Result; //anlık fi,yat

            // var accountInfo = binanceClient.GetAccountInfo().Result; 245832971 , 245835795,245839630

            //      var buyOrder = binanceClient.PostNewOrder("GRTUSDT", (decimal)11, (decimal)1, OrderSide.BUY).Result;

            //  var sellOrder = binanceClient.PostNewOrder("GRTUSDT", (decimal)8.7, (decimal)1.3676, OrderSide.SELL).Result;
            // var canceledOrder = binanceClient.CancelOrder("grtusdt", 245839630).Result;

            // var openOrders = binanceClient.GetCurrentOpenOrders("grtusdt").Result;


            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + symbol + ".txt";

            foreach (var item in candlestick)
            {
                if (!File.Exists(filepath))
                {
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(String.Format("{0};{1};{2};{3};{4}", item.OpenDateTime, item.Open, item.High, item.Low, item.Close));
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(String.Format("{0};{1};{2};{3};{4}", item.OpenDateTime, item.Open, item.High, item.Low, item.Close));
                    }
                }
            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }

}

