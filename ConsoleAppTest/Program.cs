using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiClient = new ApiClient("srhEOc1oqMt4euGiUeVBseXk588iBD4mFUD0k3VcFQQiQdRlA1NvVxVY2x0weXej", "obd4UryGMEKgdvb9B84bKGrXxusQUEQ8nYFUba85xst02dq7FNRvdFMNZtze9RDj");
            var binanceClient = new BinanceClient(apiClient);
            string symbol = "adausdt";
            int Length = 60;
            decimal Percent = 5;
            int Limit = 5000;
            IEnumerable<Candlestick> candlestick = binanceClient.GetCandleSticks(symbol, TimeInterval.Hours_1, DateTime.Now.AddDays(-365), DateTime.Now, Limit).Result;
            
            // var tickerPrices = binanceClient.GetAllPrices().Result; //anlık fi,yat

            //var accountInfo = binanceClient.GetAccountInfo().Result; //245832971 , 245835795,245839630

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
            string OTT = string.Empty;

            //foreach (var item in candlestick)
            //{
                if (!File.Exists(filepath))
                {
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        OTT = ReturnOTT(candlestick, Length, Percent);
                        sw.WriteLine(OTT);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        OTT = ReturnOTT(candlestick, Length, Percent);
                        sw.WriteLine(OTT);
                    }
                }
            //}
        }

        public static string ReturnOTT(IEnumerable<Candlestick> candlestick, int length, decimal percent)
        {
            string OTTValues = string.Empty;

            Candlestick[] CandlestickArr = new Candlestick[candlestick.Count()];
            OTT[] OTTArr = new OTT[candlestick.Count()];
            CandlestickArr = candlestick.ToArray();
            bool BuySignal = false;
            bool SellSignal = false;
            int runTimePeriod = candlestick.Count(); // 500
            decimal valpha = (decimal)2 / (length + 1);

            for (int i = 0; i < runTimePeriod; i++)
            {
                if (i == 0)
                {
                    OTTArr[i] = new OTT();
                    OTTArr[i].vud1 = 0;
                    OTTArr[i].vdd1 = 0;
                    OTTArr[i].vUD = 0;
                    OTTArr[i].vDD = 0;
                    OTTArr[i].vCMO = 0;
                    OTTArr[i].VAR = 0;
                    OTTArr[i].MAvg = 0;
                    OTTArr[i].difference = 0;
                    OTTArr[i].longstop = 0;
                    OTTArr[i].shortstop = 0;
                    OTTArr[i].dir = 1;
                    OTTArr[i].MT = 0;
                    OTTArr[i].OTTLine = 0;
                    OTTArr[i].SupportLine = 0;
                    BuySignal = false;
                    SellSignal = false;

                    OTTValues = String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", CandlestickArr[i].OpenDateTime, CandlestickArr[i].Open, CandlestickArr[i].High, CandlestickArr[i].Low, CandlestickArr[i].Close, 0, 0, BuySignal ? "1" : "0", SellSignal ? "1" : "0");
                }
                else
                {
                    OTTArr[i] = new OTT();

                    if (CandlestickArr[i].Close > CandlestickArr[i - 1].Close)
                    {
                        OTTArr[i].vud1 = CandlestickArr[i].Close - CandlestickArr[i - 1].Close;
                        OTTArr[i].vdd1 = 0;

                    }
                    else
                    {
                        OTTArr[i].vud1 = 0;
                        OTTArr[i].vdd1 = CandlestickArr[i - 1].Close - CandlestickArr[i].Close;
                    }

                    if (i < 8)
                    {
                        OTTArr[i].vUD = 0;
                        OTTArr[i].vDD = 0;
                    }
                    else
                    {
                        OTTArr[i].vUD = 0;
                        OTTArr[i].vDD = 0;

                        for (int j = 0; j < 8; j++)
                        {
                            OTTArr[i].vUD = OTTArr[i].vUD + OTTArr[i - j].vud1;
                            OTTArr[i].vDD = OTTArr[i].vDD + OTTArr[i - j].vdd1;
                        }
                    }

                    if (OTTArr[i].vUD + OTTArr[i].vDD != 0)
                        OTTArr[i].vCMO = (OTTArr[i].vUD - OTTArr[i].vDD) / (OTTArr[i].vUD + OTTArr[i].vDD);
                    else
                        OTTArr[i].vCMO = 0;

                    OTTArr[i].VAR = (valpha * Math.Abs(OTTArr[i].vCMO) * CandlestickArr[i].Close) + (1 - valpha * Math.Abs(OTTArr[i].vCMO)) * OTTArr[i - 1].VAR;
                    OTTArr[i].difference = OTTArr[i].VAR * percent * 0.01M;
                    OTTArr[i].longstop = OTTArr[i].VAR - OTTArr[i].difference;
                    OTTArr[i].longstopPrev = OTTArr[i - 1].longstop;

                    if (OTTArr[i].VAR > OTTArr[i].longstopPrev && OTTArr[i].longstop < OTTArr[i].longstopPrev)
                        OTTArr[i].longstop = OTTArr[i].longstopPrev;

                    OTTArr[i].shortstop = OTTArr[i].VAR + OTTArr[i].difference;
                    OTTArr[i].shortstopPrev = OTTArr[i - 1].shortstop;

                    if (OTTArr[i].VAR < OTTArr[i].shortstopPrev && OTTArr[i].shortstop < OTTArr[i].shortstopPrev)
                        OTTArr[i].shortstop = OTTArr[i].shortstopPrev;

                    OTTArr[i].dir = OTTArr[i - 1].dir;

                    if (OTTArr[i].dir == -1 && OTTArr[i].VAR > OTTArr[i].shortstopPrev)
                        OTTArr[i].dir = 1;
                    else if (OTTArr[i].dir == 1 && OTTArr[i].VAR < OTTArr[i].longstopPrev)
                        OTTArr[i].dir = -1;

                    if (OTTArr[i].dir == 1)
                        OTTArr[i].MT = OTTArr[i].longstop;
                    else
                        OTTArr[i].MT = OTTArr[i].shortstop;

                    // OTT
                    if (i < 2)
                        OTTArr[i].OTTLine = 0;
                    else
                    {
                        if ((OTTArr[i].VAR > OTTArr[i].MT))
                            OTTArr[i].OTTLine = OTTArr[i - 2].MT * (200 + percent) / 200;
                        else
                            OTTArr[i].OTTLine = OTTArr[i - 2].MT * (200 - percent) / 200;
                    }

                    OTTArr[i].SupportLine = OTTArr[i].VAR;

                    if (i - 1 >= 0)
                    {
                        if (OTTArr[i].SupportLine > OTTArr[i].OTTLine && OTTArr[i - 1].SupportLine <= OTTArr[i - 1].OTTLine)
                            BuySignal = true;
                        else
                            BuySignal = false;

                        if (OTTArr[i].SupportLine < OTTArr[i].OTTLine && OTTArr[i - 1].SupportLine >= OTTArr[i - 1].OTTLine)
                            SellSignal = true;
                        else
                            SellSignal = false;
                    }

                    OTTValues = OTTValues + "\n" + String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", CandlestickArr[i].OpenDateTime, CandlestickArr[i].Open, CandlestickArr[i].High, CandlestickArr[i].Low, CandlestickArr[i].Close, OTTArr[i].SupportLine, OTTArr[i].OTTLine, BuySignal ? "1" : "0", SellSignal ? "1" : "0");
                }
            }

            return OTTValues;
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

