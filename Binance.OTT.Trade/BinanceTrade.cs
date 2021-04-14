using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Telegram.Bot;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Binance.OTT.Trade
{
    public class BinanceTrade
    {
        private const string sourceDirectory = @"C:\BinanceBot\";
        private const string apiKey = "srhEOc1oqMt4euGiUeVBseXk588iBD4mFUD0k3VcFQQiQdRlA1NvVxVY2x0weXej";
        private const string apiSecret = "obd4UryGMEKgdvb9B84bKGrXxusQUEQ8nYFUba85xst02dq7FNRvdFMNZtze9RDj";
        private static TelegramBotClient botClient = new TelegramBotClient("1724957087:AAH0ByKhfMJIGPP8JI51oJMqCh9HbwwmRrU");
        private static ApiClient apiClient = new ApiClient(apiKey, apiSecret);
        private static BinanceClient binanceClient = new BinanceClient(apiClient);
        private static List<Symbol> mySembols = new List<Symbol>();

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
                    tmp.depositRatio = int.Parse(str.Split(';')[8]);
                    tmp.symbolCoin = str.Split(';')[9];
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

        private static List<Candlestick> readCurrentCandleSticks(List<Symbol> symbols, List<Balance> myBalances)
        {
            List<Candlestick> candlestickList = new List<Candlestick>();
            Balance myCurrentBalance = new Balance();
            Balance myCurrentUSDTBalance = new Balance();
            decimal myCurrentBalanceAmount = 0;
            decimal myCurrentUSDTBalanceAmount = 0;
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
                myCurrentBalance = myBalances.FirstOrDefault(i => i.Asset == item.symbolCoin);
                myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");

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

                // Calculate Available Amount
                if (myCurrentBalance != null && myCurrentUSDTBalance != null)
                {
                    myCurrentBalanceAmount = (myCurrentBalance.Free + myCurrentBalance.Locked) * lastCandleStick.Close;
                    myCurrentUSDTBalanceAmount = (((myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked) - 2) * item.depositRatio) / 100;

                    if (myCurrentBalanceAmount < 10.02M && myCurrentUSDTBalanceAmount > 10.02M)
                        mySembols.Where(i => i.symbolCoin == item.symbolCoin).ToList().ForEach(c => c.availableAmount = myCurrentUSDTBalanceAmount);
                    else
                        mySembols.Where(i => i.symbolCoin == item.symbolCoin).ToList().ForEach(c => c.availableAmount = 0);
                }
                else if (myCurrentBalance == null)
                {
                    myCurrentUSDTBalanceAmount = (((myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked) - 2) * item.depositRatio) / 100;

                    if (myCurrentUSDTBalanceAmount > 10.02M)
                        mySembols.Where(i => i.symbolCoin == item.symbolCoin).ToList().ForEach(c => c.availableAmount = myCurrentUSDTBalanceAmount);
                    else
                        mySembols.Where(i => i.symbolCoin == item.symbolCoin).ToList().ForEach(c => c.availableAmount = 0);
                }
            }

            return candlestickList;
        }

        private static List<Balance> getBalances()
        {
            // Get Acount Infos
            var accountInfos = binanceClient.GetAccountInfo().Result;

            return accountInfos.Balances.Where(i => i.Locked != 0 || i.Free != 0).ToList();
        }

        private static List<Order> getCurrentOpenOrders(List<Symbol> symbols)
        {
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
            List<Binance.API.Csharp.Client.Models.Account.Trade> myLastTrades = new List<API.Csharp.Client.Models.Account.Trade>();
            List<Order> myCurrentOrder = new List<Order>();
            List<Order> myLastFilledOrders = new List<Order>();
            Order myLastOrder = new Order();

            foreach (var item in symbols)
            {
                myLastOrder = new Order();
                myCurrentOrder = binanceClient.GetAllOrders(item.symbol).Result.ToList();

                if (myCurrentOrder != null && myCurrentOrder.Count() > 0)
                {
                    myLastOrder = myCurrentOrder.LastOrDefault(i => i.Status == "FILLED");

                    if (myLastOrder != null)
                        myLastFilledOrders.Add(myLastOrder);
                }
            }

            return myLastFilledOrders;
        }

        private static void SendMessageFromTelegramBot(string message)
        {
            botClient.SendTextMessageAsync("-535329225", message);
        }

        public static async Task TradeAsync()
        {
            Symbol myLastSymbol = new Symbol();
            List<Candlestick> myCandlesticks = new List<Candlestick>();
            List<Candlestick> myAvailableCandlesticks = new List<Candlestick>();
            List<Balance> myBalances = new List<Balance>();
            Order myCurrentOpenOrder = new Order();
            Balance myCurrentUSDTBalance = new Balance();
            Balance myCurrentCoinBalance = new Balance();
            Order myCurrentLastTrade = new Order();
            Candlestick myCurrentCandleStick = new Candlestick();
            Candlestick myAvailableCurrentCandleStick = new Candlestick();
            NewOrder myNewOrder = new NewOrder();
            CanceledOrder myCancelOrder = new CanceledOrder();
            decimal buyPrice = 0;
            decimal buyQuantity = 0;
            decimal orderAmount = 0;
            decimal sellPrice = 0;
            decimal sellQuantity = 0;
            decimal availableBuyAmount = 0;
            decimal currentCoinUSDTAmount = 0;
            decimal currentCoinAmount = 0;
            long orderId = 0;

            // Read Symbols
            mySembols = readSymbols();

            // Get Balances
            myBalances = getBalances();

            // Read CandleSticks
            myCandlesticks = readLastCandleSticks(mySembols);
            myAvailableCandlesticks = readCurrentCandleSticks(mySembols, myBalances);

            // Get Open Orders
            List<Order> myOpenOrders = getCurrentOpenOrders(mySembols);

            // Get Account Last Trades
            List<Order> myLastTrades = getLastTrades(mySembols);

            // Get My Last Symbol
            myLastSymbol = mySembols.LastOrDefault();

            // USDT Balanace
            myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");
            if (myCurrentUSDTBalance != null)
                SendMessageFromTelegramBot(string.Format("Mevcut USDT miktarı: {0}", Math.Round((myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked), 2)));

            // Trade
            foreach (var item in mySembols)
            {
                // Get Account Info && Balances
                myBalances = getBalances();
                myCurrentOpenOrder = myOpenOrders.FirstOrDefault(i => i.Symbol == item.symbol.ToUpper());
                myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");
                myCurrentCoinBalance = myBalances.FirstOrDefault(i => i.Asset == item.symbolCoin);
                myCurrentLastTrade = myLastTrades.FirstOrDefault(i => i.Symbol == item.symbol.ToUpper());
                myCurrentCandleStick = myCandlesticks.FirstOrDefault(i => i.Symbol == item.symbol);
                myAvailableCurrentCandleStick = myAvailableCandlesticks.FirstOrDefault(i => i.Symbol == item.symbol);
                buyPrice = 0;
                buyQuantity = 0;
                sellPrice = 0;
                sellQuantity = 0;
                orderId = 0;
                orderAmount = 0;
                currentCoinUSDTAmount = 0;
                currentCoinAmount = 0;

                // Get Available Amount
                availableBuyAmount = item.availableAmount;

                // Send Coin Info
                if (myCurrentCoinBalance != null)
                {
                    currentCoinUSDTAmount = Math.Round(((myCurrentCoinBalance.Free + myCurrentCoinBalance.Locked) * myAvailableCurrentCandleStick.Close), 2);
                    currentCoinAmount = myCurrentCoinBalance.Free + myCurrentCoinBalance.Locked;
                }
                else
                {
                    currentCoinUSDTAmount = 0;
                    currentCoinAmount = 0;
                }

                SendMessageFromTelegramBot(string.Format("Mevcut {0} miktarı: {1} ({2} USDT)", item.symbolCoin, currentCoinAmount, currentCoinUSDTAmount));

                // Case 1
                if ((myCurrentLastTrade == null || (myCurrentLastTrade.Side == "SELL")) && myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && myCurrentOpenOrder == null && availableBuyAmount > 10.02M && myCurrentUSDTBalance.Free > availableBuyAmount)
                {
                    if ((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)) > myAvailableCurrentCandleStick.High)
                        buyPrice = Math.Round(myAvailableCurrentCandleStick.Close + (myAvailableCurrentCandleStick.Close * 0.002M), 2);
                    else
                        buyPrice = Math.Round((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)), 2);
                    buyQuantity = Math.Round((availableBuyAmount / buyPrice), 4);

                    myNewOrder = await binanceClient.PostNewOrder(item.symbol, buyQuantity, buyPrice, OrderSide.BUY);
                    orderAmount = Math.Round(buyQuantity * buyPrice, 2);

                    SendMessageFromTelegramBot(string.Format("{0} için {1} adet ve {2} fiyattan ALIM emri girilmiştir. İşlem hacmi {3}", item.symbol.ToUpper(), buyQuantity, buyPrice, orderAmount));
                } // Case 2
                else if (myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "SELL"))
                {
                    orderId = myCurrentOpenOrder.OrderId;

                    myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                    SendMessageFromTelegramBot(string.Format("{0} için ALIM emri İPTAL edilmiştir. Order Id: {1}", item.symbol.ToUpper(), orderId));
                } // Case 3
                else if (myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "BUY"))
                {
                    if ((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)) > myAvailableCurrentCandleStick.High)
                        buyPrice = Math.Round(myAvailableCurrentCandleStick.Close + (myAvailableCurrentCandleStick.Close * 0.002M), 2);
                    else
                        buyPrice = Math.Round((myCurrentCandleStick.OTTLine - (myCurrentCandleStick.OTTLine * item.buyRatio)), 2);

                    if (myCurrentOpenOrder.Price != buyPrice)
                    {
                        orderId = myCurrentOpenOrder.OrderId;

                        myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                        // Calculate Quantity 
                        buyQuantity = Math.Round((availableBuyAmount / buyPrice), 4);

                        myNewOrder = await binanceClient.PostNewOrder(item.symbol, buyQuantity, buyPrice, OrderSide.BUY);
                        orderAmount = Math.Round(buyQuantity * buyPrice, 2);

                        SendMessageFromTelegramBot(string.Format("{0} için önceki verilen ALIM emri İPTAL edilmiştir. (Order Id: {1}) - {2} adet ve {3} fiyattan ALIM emri güncellenmiştir. İşlem Hacmi {4}", item.symbol.ToUpper(), orderId, buyQuantity, buyPrice, orderAmount));
                    }
                    else
                    {
                        SendMessageFromTelegramBot(string.Format("{0} için mevcuttaki ALIM emri GÜNCELLENMEMİŞTİR. Mevcut ALIM fiyatı {1}", item.symbol.ToUpper(), buyPrice));
                    }
                } // Case 4
                else if ((myCurrentLastTrade != null && myCurrentLastTrade.Side == "BUY") && myCurrentCandleStick.SupportLine < myCurrentCandleStick.OTTLine && myCurrentOpenOrder == null && myCurrentCoinBalance.Free > 0)
                {
                    if ((myCurrentCandleStick.OTTLine + (myCurrentCandleStick.OTTLine * item.sellRatio)) < myAvailableCurrentCandleStick.Low)
                        sellPrice = Math.Round(myAvailableCurrentCandleStick.Close - (myAvailableCurrentCandleStick.Close * 0.002M), 2);
                    else
                        sellPrice = Math.Round((myCurrentCandleStick.OTTLine + (myCurrentCandleStick.OTTLine * item.sellRatio)), 2);
                    sellQuantity = Math.Round(myCurrentCoinBalance.Free - 0.0001M, 4);

                    myNewOrder = await binanceClient.PostNewOrder(item.symbol, sellQuantity, sellPrice, OrderSide.SELL);
                    orderAmount = Math.Round(sellQuantity * sellPrice, 2);

                    SendMessageFromTelegramBot(string.Format("{0} için {1} adet ve {2} fiyattan SATIŞ emri girilmiştir. İşlem hacmi {3}", item.symbol.ToUpper(), sellQuantity, sellPrice, orderAmount));
                } // Case 5
                else if (myCurrentCandleStick.SupportLine < myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "BUY"))
                {
                    orderId = myCurrentOpenOrder.OrderId;

                    myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                    SendMessageFromTelegramBot(string.Format("{0} için ALIM emri İPTAL edilmiştir. Order Id: {1}", item.symbol.ToUpper(), orderId));
                } // Case 6
                else if (myCurrentCandleStick.SupportLine < myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "SELL"))
                {
                    if ((myCurrentCandleStick.OTTLine + (myCurrentCandleStick.OTTLine * item.sellRatio)) < myAvailableCurrentCandleStick.Low)
                        sellPrice = Math.Round(myAvailableCurrentCandleStick.Close - (myAvailableCurrentCandleStick.Close * 0.002M), 2);
                    else
                        sellPrice = Math.Round((myCurrentCandleStick.OTTLine + (myCurrentCandleStick.OTTLine * item.sellRatio)), 2);

                    if (myCurrentOpenOrder.Price != sellPrice)
                    {
                        orderId = myCurrentOpenOrder.OrderId;

                        myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                        sellQuantity = Math.Round(myCurrentCoinBalance.Locked - 0.0001M, 4);

                        myNewOrder = await binanceClient.PostNewOrder(item.symbol, sellQuantity, sellPrice, OrderSide.SELL);
                        orderAmount = Math.Round(sellQuantity * sellPrice, 2);

                        SendMessageFromTelegramBot(string.Format("{0} için önceki verilen SATIŞ emri İPTAL edilmiştir. (Order Id: {1}) - {2} adet ve {3} fiyattan SATIŞ emri güncellenmiştir. İşlem Hacmi {4}", item.symbol.ToUpper(), orderId, sellQuantity, sellPrice, orderAmount));
                    }
                    else
                    {
                        SendMessageFromTelegramBot(string.Format("{0} için mevcuttaki SATIŞ emri GÜNCELLENMEMİŞTİR. Mevcut SATIŞ fiyatı {1}", item.symbol.ToUpper(), sellPrice));
                    }
                }
            }
        }
    }
}