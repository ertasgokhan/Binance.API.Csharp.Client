using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Binance.Generate.OTT
{
    public static class GenerateOTTLine
    {
        public const string sourceDirectory = @"C:\BinanceBot\";
        public const string apiKey = "srhEOc1oqMt4euGiUeVBseXk588iBD4mFUD0k3VcFQQiQdRlA1NvVxVY2x0weXej";
        public const string apiSecret = "obd4UryGMEKgdvb9B84bKGrXxusQUEQ8nYFUba85xst02dq7FNRvdFMNZtze9RDj";
        public const int limit = 1000;

        public static void GenerateOTT()
        {
            try
            {
                List<Symbol> symbolsList = readSymbols();

                Parallel.ForEach(symbolsList, item =>
                {
                    GetForOnePair(item);
                });
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        public static List<Symbol> readSymbols()
        {
            List<Symbol> symbolsList = new List<Symbol>();
            string filepath = sourceDirectory + "symbols.txt";

            using (StreamReader rd = File.OpenText(filepath))
            {
                while (!rd.EndOfStream)
                {
                    Symbol tmp = new Symbol();
                    string str = rd.ReadLine();
                    tmp.symbol = str.Split(';')[0];
                    tmp.length = int.Parse(str.Split(';')[1]);
                    tmp.percent = Decimal.Parse(str.Split(';')[2]);
                    symbolsList.Add(tmp);
                }
            }

            return symbolsList;
        }

        public static void GetForOnePair(Symbol symbolItem)
        {
            var apiClient = new ApiClient(apiKey, apiSecret);
            var binanceClient = new BinanceClient(apiClient);
            string symbol = symbolItem.symbol;
            int Length = symbolItem.length;
            decimal Percent = symbolItem.percent;

            string filepath = sourceDirectory + symbol + ".txt";
            string OTTLines = string.Empty;
            List<Candlestick> candlestick = new List<Candlestick>();
            List<Candlestick> tempCandlestick = new List<Candlestick>();

            for (int i = -24; i < 0; i++)
            {
                tempCandlestick = binanceClient.GetCandleSticks(symbol, TimeInterval.Hours_1, DateTime.Now.AddMonths(i), DateTime.Now.AddMonths(i + 1), limit).Result.ToList();

                if (tempCandlestick != null && tempCandlestick.Count() > 0)
                    candlestick.AddRange(tempCandlestick);
            }

            if (File.Exists(filepath))
                File.Delete(filepath);

            using (StreamWriter sw = File.CreateText(filepath))
            {
                OTTLines = ReturnOTT(candlestick, Length, Percent);
                sw.WriteLine(OTTLines);
            }
        }

        public static string ReturnOTT(List<Candlestick> candlestick, int length, decimal percent)
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
                    if (OTTArr[i] == null)
                        OTTArr[i] = new OTT();

                    if (i + 2 < runTimePeriod)
                        OTTArr[i + 2] = new OTT();

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

                        for (int j = 0; j <= 8; j++)
                        {
                            OTTArr[i].vUD = OTTArr[i].vUD + OTTArr[i - j].vud1;
                            OTTArr[i].vDD = OTTArr[i].vDD + OTTArr[i - j].vdd1;
                        }
                    }

                    if (OTTArr[i].vUD + OTTArr[i].vDD != 0)
                        OTTArr[i].vCMO = (OTTArr[i].vUD - OTTArr[i].vDD) / (OTTArr[i].vUD + OTTArr[i].vDD);
                    else
                        OTTArr[i].vCMO = 0;

                    OTTArr[i].VAR = (decimal)((decimal)(valpha * Math.Abs(OTTArr[i].vCMO) * CandlestickArr[i].Close) + (decimal)(1 - valpha * Math.Abs(OTTArr[i].vCMO)) * OTTArr[i - 1].VAR);
                    OTTArr[i].difference = OTTArr[i].VAR * percent * 0.01M;
                    OTTArr[i].longstop = OTTArr[i].VAR - OTTArr[i].difference;
                    OTTArr[i].longstopPrev = OTTArr[i - 1].longstop;

                    if (OTTArr[i].VAR > OTTArr[i].longstopPrev && OTTArr[i].longstop < OTTArr[i].longstopPrev)
                        OTTArr[i].longstop = OTTArr[i].longstopPrev;

                    OTTArr[i].shortstop = OTTArr[i].VAR + OTTArr[i].difference;
                    OTTArr[i].shortstopPrev = OTTArr[i - 1].shortstop;

                    if (OTTArr[i].VAR < OTTArr[i].shortstopPrev && OTTArr[i].shortstop > OTTArr[i].shortstopPrev)
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
                    if (i + 2 < runTimePeriod)
                    {
                        if ((OTTArr[i].VAR > OTTArr[i].MT))
                            OTTArr[i + 2].OTTLine = (decimal)((OTTArr[i].MT) * (decimal)((200 + percent) / 200));
                        else
                            OTTArr[i + 2].OTTLine = (decimal)((OTTArr[i].MT) * (decimal)((200 - percent) / 200));
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

        public static void WriteLog(string LogMessage)
        {
            string filepath = sourceDirectory + "\\Log.txt";

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(LogMessage);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(LogMessage);
                }
            }
        }
    }
}
