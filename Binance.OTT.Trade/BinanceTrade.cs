using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
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
                    tmp.buyRatio = Decimal.Parse(str.Split(';')[3]);
                    tmp.sellRatio = Decimal.Parse(str.Split(';')[4]);
                    tmp.buyTrendSwitch = str.Split(';')[5] == "0" ? false : true;
                    tmp.buyTrendRatio = Decimal.Parse(str.Split(';')[6]);
                    tmp.sellTrendRatio = Decimal.Parse(str.Split(';')[7]);
                    tmp.symbolCoin = str.Split(';')[8];
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

        private static List<Candlestick> readCurrentCandleSticks(List<Symbol> symbols)
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
                    lastCandleStickStr = symbolCandleStick.Split('\n')[lineNumber - 2];
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

        private static List<Order> getLastSellTrades(List<Symbol> symbols)
        {
            var apiClient = new ApiClient(apiKey, apiSecret);
            var binanceClient = new BinanceClient(apiClient);
            List<Binance.API.Csharp.Client.Models.Account.Trade> myLastTrades = new List<API.Csharp.Client.Models.Account.Trade>();
            List<Order> myCurrentOrder = new List<Order>();
            List<Order> myLastFilledOrders = new List<Order>();

            foreach (var item in symbols)
            {
                myCurrentOrder = binanceClient.GetAllOrders(item.symbol).Result.ToList();

                if (myCurrentOrder != null && myCurrentOrder.Count() > 0 && myCurrentOrder[myCurrentOrder.Count - 1].Side == "SELL" && myCurrentOrder[myCurrentOrder.Count - 1].Status == "FILLED")
                    myLastFilledOrders.Add(myCurrentOrder[myCurrentOrder.Count - 1]);
            }

            return myLastFilledOrders;
        }

        public static async System.Threading.Tasks.Task TradeAsync()
        {
            var apiClient = new ApiClient(apiKey, apiSecret);
            var binanceClient = new BinanceClient(apiClient);

            // Get Account Info && Balances
            var myBalances = getBalances();

            // Read Symbols
            List<Symbol> mySembols = readSymbols();

            // Read CandleSticks
            List<Candlestick> myCandlesticks = new List<Candlestick>();
            List<Candlestick> myAvailableCandlesticks = new List<Candlestick>();


            if (mySembols != null && mySembols.Count > 0)
            {
                myCandlesticks = readLastCandleSticks(mySembols);
                myAvailableCandlesticks = readCurrentCandleSticks(mySembols);
            }

            // Get Open Orders
            List<Order> myOpenOrders = getCurrentOpenOrders(mySembols);

            // Get Account Last Trades
            List<Order> myLastSellTrades = getLastSellTrades(mySembols);

            // Trade
            foreach (var item in mySembols)
            {
                Order myCurrentOpenOrder = myOpenOrders.FirstOrDefault(i => i.Symbol == item.symbol.ToUpper());
                Balance myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");
                Balance myCurrentCoinBalance = myBalances.FirstOrDefault(i => i.Asset == item.symbolCoin);
                Candlestick myCurrentCandleStick = myCandlesticks.FirstOrDefault(i => i.Symbol == item.symbol);
                Candlestick myAvailableCurrentCandleStick = myAvailableCandlesticks.FirstOrDefault(i => i.Symbol == item.symbol);
                decimal buyPrice = 0;
                decimal buyQuantity = 0;
                decimal sellPrice = 0;
                decimal sellQuantity = 0;
                decimal availableBuyAmount = 12;
                long orderId = 0;

                // Case 1
                if (myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && myCurrentOpenOrder == null && myCurrentUSDTBalance.Free > availableBuyAmount)
                {
                    if ((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)) > myAvailableCurrentCandleStick.High)
                        buyPrice = Math.Round(myAvailableCurrentCandleStick.Close + (myAvailableCurrentCandleStick.Close * 0.002M), 2);
                    else
                        buyPrice = Math.Round((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)), 2);
                    buyQuantity = Math.Round((availableBuyAmount / buyPrice), 4);

                    NewOrder myNewOrder = await binanceClient.PostNewOrder(item.symbol, buyQuantity, buyPrice, OrderSide.BUY);
                } // Case 2
                else if (myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "SELL"))
                {
                    orderId = myCurrentOpenOrder.OrderId;

                    CanceledOrder myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);
                } // Case 3
                else if (myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "BUY"))
                {
                    orderId = myCurrentOpenOrder.OrderId;

                    CanceledOrder myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                    if ((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)) > myAvailableCurrentCandleStick.High)
                        buyPrice = Math.Round(myAvailableCurrentCandleStick.Close + (myAvailableCurrentCandleStick.Close * 0.002M), 2);
                    else
                        buyPrice = Math.Round((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)), 2);
                    buyQuantity = Math.Round((availableBuyAmount / buyPrice), 4);

                    NewOrder myNewOrder = await binanceClient.PostNewOrder(item.symbol, buyQuantity, buyPrice, OrderSide.BUY);
                } // Case 4
                else if (myCurrentCandleStick.SupportLine < myCurrentCandleStick.OTTLine && myCurrentOpenOrder == null && myCurrentCoinBalance.Free > 0)
                {
                    if ((myCurrentCandleStick.OTTLine + (myCurrentCandleStick.OTTLine * item.sellRatio)) < myAvailableCurrentCandleStick.Low)
                        sellPrice = Math.Round(myAvailableCurrentCandleStick.Close - (myAvailableCurrentCandleStick.Close * 0.002M), 2);
                    else
                        sellPrice = Math.Round((myCurrentCandleStick.OTTLine + (myCurrentCandleStick.OTTLine * item.sellRatio)), 2);
                    sellQuantity = Math.Round(myCurrentCoinBalance.Free - 0.0001M, 4);

                    NewOrder myNewOrder = await binanceClient.PostNewOrder(item.symbol, sellQuantity, sellPrice, OrderSide.SELL);
                }
            }
        }
    }
}
