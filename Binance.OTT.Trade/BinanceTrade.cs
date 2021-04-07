using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Binance.OTT.Trade
{
    public class BinanceTrade
    {
        private const string sourceDirectory = @"C:\BinanceBot\";
        private const string apiKey = "srhEOc1oqMt4euGiUeVBseXk588iBD4mFUD0k3VcFQQiQdRlA1NvVxVY2x0weXej";
        private const string apiSecret = "obd4UryGMEKgdvb9B84bKGrXxusQUEQ8nYFUba85xst02dq7FNRvdFMNZtze9RDj";

        private static List<Symbol> readSymbols()
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

        private static List<Candlestick> readLastCandleSticks(List<Symbol> symbols)
        {
            List<Candlestick> candlestickList = new List<Candlestick>();
            string filepath = string.Empty;
            int lineNumber = 0;
            string symbolCandleStick = string.Empty;
            string lastCandleStickStr = string.Empty;
            Candlestick lastCandleStick = new Candlestick();

            foreach (var item in symbols)
            {
                filepath = sourceDirectory + item.symbol + ".txt";
                lineNumber = 0;
                symbolCandleStick = string.Empty;
                lastCandleStickStr = string.Empty;
                lastCandleStick = new Candlestick();

                using (StreamReader rd = File.OpenText(filepath))
                {
                    symbolCandleStick = rd.ReadToEnd();
                    lineNumber = symbolCandleStick.Split('\n').Length;
                    lastCandleStickStr = symbolCandleStick.Split('\n')[lineNumber - 3];
                    // Read Lines
                    lastCandleStick.Symbol = item.symbol;
                    lastCandleStick.OpenDateTime = DateTime.Parse(lastCandleStickStr.Split(';')[0]);
                    lastCandleStick.Open = (decimal)(Decimal.Parse(lastCandleStickStr.Split(';')[1]));
                    lastCandleStick.High = (decimal)(Decimal.Parse(lastCandleStickStr.Split(';')[2]));
                    lastCandleStick.Low = (decimal)(Decimal.Parse(lastCandleStickStr.Split(';')[3]));
                    lastCandleStick.Close = (decimal)(Decimal.Parse(lastCandleStickStr.Split(';')[4]));
                    lastCandleStick.SupportLine = (decimal)(Decimal.Parse(lastCandleStickStr.Split(';')[5]));
                    lastCandleStick.OTTLine = (decimal)(Decimal.Parse(lastCandleStickStr.Split(';')[6]));
                    lastCandleStick.BuySignal = lastCandleStickStr.Split(';')[7] == "0" ? false : true;
                    lastCandleStick.SellSignal = lastCandleStickStr.Split(';')[8] == "0" ? false : true;
                    candlestickList.Add(lastCandleStick);
                }
            }

            return candlestickList;
        }

        private static List<Balance> getBalances()
        {
            var apiClient = new ApiClient(apiKey, apiSecret);
            var binanceClient = new BinanceClient(apiClient);
            // Get Acount Infos
            var accountInfos = binanceClient.GetAccountInfo().Result;

            return accountInfos.Balances.Where(i => i.Locked != 0 || i.Free != 0).ToList();
        }

        private static List<Order> getCurrentOpenOrders(List<Symbol> symbols)
        {
            var apiClient = new ApiClient(apiKey, apiSecret);
            var binanceClient = new BinanceClient(apiClient);
            List<Order> myOpenOrders = new List<Order>();
            List<Order> myCurrentOpenOrders = new List<Order>();

            foreach (var item in symbols)
            {
                myCurrentOpenOrders = binanceClient.GetCurrentOpenOrders(item.symbol).Result.ToList();

                if (myCurrentOpenOrders != null && myCurrentOpenOrders.Count() > 0)
                    myOpenOrders.AddRange(myCurrentOpenOrders);
            }

            return myOpenOrders;
        }

        private static List<Order> getLastTrades(List<Symbol> symbols)
        {
            var apiClient = new ApiClient(apiKey, apiSecret);
            var binanceClient = new BinanceClient(apiClient);
            List<Binance.API.Csharp.Client.Models.Account.Trade> myLastTrades = new List<API.Csharp.Client.Models.Account.Trade>();
            List<Order> myCurrentOrder = new List<Order>();
            List<Order> myLastFilledOrders = new List<Order>();
            int orderId = 0;

            foreach (var item in symbols)
            {
                orderId = 0;
                myLastTrades = binanceClient.GetTradeList(item.symbol).Result.ToList();

                if (myLastTrades != null && myLastTrades.Count > 0)
                {
                    orderId = myLastTrades.Last().Id;

                    if (orderId > 0)
                    {
                        myCurrentOrder = binanceClient.GetAllOrders(item.symbol, orderId).Result.ToList();

                        if (myCurrentOrder != null && myCurrentOrder.Count() > 0)
                            myLastFilledOrders.AddRange(myCurrentOrder);
                    }
                }
            }

            return myLastFilledOrders;
        }

        public static void Trade()
        {
            // Get Account Info && Balances
            var myBalances = getBalances();

            // Read Symbols
            List<Symbol> mySembols = readSymbols();

            // Read CandleSticks
            List<Candlestick> myCandlesticks = new List<Candlestick>();

            if (mySembols != null && mySembols.Count > 0)
            {
                myCandlesticks = readLastCandleSticks(mySembols);
            }

            // Get Open Orders
            List<Order> myOpenOrders = getCurrentOpenOrders(mySembols);

            // Get Account Last Trades
            List<Order> myLastTrades = getLastTrades(mySembols);
        }
    }
}
