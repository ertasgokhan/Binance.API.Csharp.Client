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
using System.Globalization;
using System.Threading;
using Binance.UtilitiesLib;

namespace Binance.OTT.Trade
{
    public class BinanceTrade
    {
        private static EnvironmentVariables environmentVariables = new EnvironmentVariables();
        private static ApiClient apiClient = new ApiClient("", "");
        private static BinanceClient binanceClient = new BinanceClient(apiClient);
        private static List<Symbol> mySembols = new List<Symbol>();
        private static TelegramBotClient botClient;

        private static void readEnvironmentVariables(string account)
        {
            string filepath = @"C:\TradeBot\" + account + "environment_variables.txt";

            using (StreamReader rd = File.OpenText(filepath))
            {
                while (!rd.EndOfStream)
                {
                    string str = rd.ReadLine();
                    environmentVariables.x = StringCipher.Decrypt(str.Split(';')[0]);
                    environmentVariables.y = StringCipher.Decrypt(str.Split(';')[1]);
                    environmentVariables.z = StringCipher.Decrypt(str.Split(';')[2]);
                    environmentVariables.w = StringCipher.Decrypt(str.Split(';')[3]);
                }
            }

            apiClient = new ApiClient(environmentVariables.x, environmentVariables.y);
            binanceClient = new BinanceClient(apiClient);
            botClient = new TelegramBotClient(environmentVariables.z);
        }

        private static async Task<List<Symbol>> readSymbolsAsync(string account)
        {
            List<Symbol> symbolsList = new List<Symbol>();

            try
            {
                string filepath = @"C:\TradeBot\" + account + "symbols.txt";

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
                        tmp.depositRatio = decimal.Parse(str.Split(';')[8]);
                        tmp.priceRound = int.Parse(str.Split(';')[9]);
                        tmp.quantityRound = int.Parse(str.Split(';')[10]);
                        tmp.symbolCoin = str.Split(';')[11];
                        symbolsList.Add(tmp);
                    }
                }

                return symbolsList;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("Sembol listesi okunurken hata oluştu. Hata: {0}", ex.InnerException));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Sembol listesi okunurken hata oluştu. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }

                return symbolsList;
            }
        }

        private static async Task<List<Candlestick>> readLastCandleSticksAsync(List<Symbol> symbols, string account)
        {
            List<Candlestick> candlestickList = new List<Candlestick>();

            try
            {
                string filepath = string.Empty;
                int lineNumber = 0;
                string symbolCandleStick = string.Empty;
                string lastCandleStickStr = string.Empty;
                Candlestick lastCandleStick = new Candlestick();

                foreach (var item in symbols)
                {
                    filepath = @"C:\TradeBot\COMMON\" + item.symbol + ".txt";
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

                        if (lastCandleStick.BuySignal)
                            await SendTelegramMessageAsync(string.Format("{0} için OTT tarafından AL sinyali gelmiştir", item.symbol.ToUpper()));
                        else if (lastCandleStick.SellSignal)
                            await SendTelegramMessageAsync(string.Format("{0} için OTT tarafından SAT sinyali gelmiştir", item.symbol.ToUpper()));
                    }
                }

                return candlestickList;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("15 DK'lık mum verileri okunurken hata oluştu. Hata: {0}", ex.InnerException.Message));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("15 DK'lık mum verileri okunurken hata oluştu. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }

                return candlestickList;
            }
        }

        private static async Task<List<Candlestick>> readCurrentCandleSticksAsync(List<Symbol> symbols, string account)
        {
            List<Candlestick> candlestickList = new List<Candlestick>();

            try
            {
                string filepath = string.Empty;
                int lineNumber = 0;
                string symbolCandleStick = string.Empty;
                string lastCandleStickStr = string.Empty;
                Candlestick lastCandleStick = new Candlestick();

                foreach (var item in symbols)
                {
                    filepath = @"C:\TradeBot\COMMON\" + item.symbol + ".txt";
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
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("Anlık mum verileri okunurken hata oluştu. Hata: {0}", ex.InnerException.Message));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Anlık mum verileri okunurken hata oluştu. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }

                return candlestickList;
            }
        }

        private static async Task calculateAvailableAmountAsync(List<Symbol> symbols, List<Balance> myBalances, List<Candlestick> candlestickList, string account)
        {
            try
            {
                Candlestick myCurrentCandlestickList = new Candlestick();
                Balance myCurrentBalance = new Balance();
                Balance myCurrentUSDTBalance = new Balance();
                decimal myCurrentBalanceAmount = 0;
                decimal myCurrentUSDTBalanceAmount = 0;
                decimal idleDepositRatio = 0;
                decimal idleUSDTBalance = 0;
                decimal symbolUSDTBalanceCount = 0;
                decimal addedAvailableAmount = 0;
                decimal finalAvailableAmount = 0;

                foreach (var item in symbols)
                {
                    myCurrentBalance = myBalances.FirstOrDefault(i => i.Asset == item.symbolCoin);
                    myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");
                    myCurrentCandlestickList = candlestickList.FirstOrDefault(i => i.Symbol == item.symbol);

                    // Calculate Available Amount
                    if (myCurrentBalance != null && myCurrentUSDTBalance != null)
                    {
                        myCurrentBalanceAmount = (myCurrentBalance.Free + myCurrentBalance.Locked) * myCurrentCandlestickList.Close;
                        myCurrentUSDTBalanceAmount = (((myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked) - 2) * item.depositRatio) / 100;

                        if (myCurrentBalanceAmount < 10.02M)
                        {
                            mySembols.Where(i => i.symbolCoin == item.symbolCoin).ToList().ForEach(c => c.availableAmount = myCurrentUSDTBalanceAmount);
                            symbolUSDTBalanceCount++;
                        }
                        else
                        {
                            mySembols.Where(i => i.symbolCoin == item.symbolCoin).ToList().ForEach(c => c.availableAmount = 0);
                            idleDepositRatio += item.depositRatio;
                        }
                    }
                    else if (myCurrentBalance == null)
                    {
                        myCurrentUSDTBalanceAmount = (((myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked) - 2) * item.depositRatio) / 100;
                        mySembols.Where(i => i.symbolCoin == item.symbolCoin).ToList().ForEach(c => c.availableAmount = myCurrentUSDTBalanceAmount);
                        symbolUSDTBalanceCount++;
                    }
                }

                idleUSDTBalance = (((myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked) - 2) * idleDepositRatio) / 100;

                foreach (var item2 in symbols)
                {
                    if (item2.availableAmount > 0)
                    {
                        addedAvailableAmount = Math.Round(idleUSDTBalance / symbolUSDTBalanceCount, 2);
                        finalAvailableAmount = item2.availableAmount + addedAvailableAmount;

                        if (finalAvailableAmount > 10.02M)
                            mySembols.Where(i => i.symbolCoin == item2.symbolCoin).ToList().ForEach(c => c.availableAmount = c.availableAmount + addedAvailableAmount);
                        else
                            mySembols.Where(i => i.symbolCoin == item2.symbolCoin).ToList().ForEach(c => c.availableAmount = 0);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("Kullanılabilir USDT bakiyelerin hesaplanması sırasında hata oluştu. Hata: {0}", ex.InnerException.Message));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Kullanılabilir USDT bakiyelerin hesaplanması sırasında hata oluştu. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }
            }
        }

        private static async Task<List<Balance>> getBalancesAsync(List<Symbol> mySembols, string account)
        {
            var tempBalances = new List<Balance>();

            try
            {
                // Get Acount Infos
                var accountInfos = binanceClient.GetAccountInfo().Result;
                var myCurrentSymbol = new Symbol();

                tempBalances = accountInfos.Balances.Where(i => i.Locked != 0 || i.Free != 0).ToList();

                foreach (var item in tempBalances)
                {
                    myCurrentSymbol = mySembols.FirstOrDefault(i => i.symbolCoin == item.Asset);

                    if (myCurrentSymbol != null)
                    {
                        item.Free = Math.Round(item.Free, myCurrentSymbol.quantityRound);
                        item.Locked = Math.Round(item.Locked, myCurrentSymbol.quantityRound);
                    }
                }

                return tempBalances;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("Coinlerin bakiyelerini çekerken hata oluştu. Hata: {0}", ex.InnerException.Message));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Coinlerin bakiyelerini çekerken hata oluştu. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }

                return tempBalances;
            }
        }

        private static async Task<List<Order>> getCurrentOpenOrdersAsync(List<Symbol> symbols, string account)
        {
            List<Order> myOpenOrders = new List<Order>();
            List<Order> myCurrentOpenOrders = new List<Order>();

            try
            {
                foreach (var item in symbols)
                {
                    myCurrentOpenOrders = binanceClient.GetCurrentOpenOrders(item.symbol).Result.ToList();

                    if (myCurrentOpenOrders != null && myCurrentOpenOrders.Count() > 0)
                        myOpenOrders.AddRange(myCurrentOpenOrders);
                }

                return myOpenOrders;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("Açık emirler çekilirken hata oluştu. Hata: {0}", ex.InnerException.Message));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Coinlerin bakiyelerini çekerken hata oluştu. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }

                return myOpenOrders;
            }
        }

        private static async Task<List<Order>> getLastTradesAsync(List<Symbol> symbols, string account)
        {
            List<Order> myCurrentOrder = new List<Order>();
            List<Order> myLastFilledOrders = new List<Order>();
            Order myLastOrder = new Order();
            Order myLastOpenOrder = new Order();

            try
            {
                List<Binance.API.Csharp.Client.Models.Account.Trade> myLastTrades = new List<API.Csharp.Client.Models.Account.Trade>();

                foreach (var item in symbols)
                {
                    myLastOrder = new Order();
                    myCurrentOrder = binanceClient.GetAllOrders(item.symbol).Result.ToList();

                    if (myCurrentOrder != null && myCurrentOrder.Count() > 0)
                    {
                        myLastOrder = myCurrentOrder.LastOrDefault(i => i.Status == "FILLED");
                        //myLastOpenOrder = myCurrentOrder.LastOrDefault(i => i.Status == "NEW");

                        if (myLastOrder != null)
                            myLastFilledOrders.Add(myLastOrder);

                        //if (myLastOpenOrder != null)
                        //    myOpenOrders.Add(myLastOpenOrder);
                    }
                }

                return myLastFilledOrders;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("Son gerçekleşen tradeler çekilirken hata oluştu. Hata: {0}", ex.InnerException.Message));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Son gerçekleşen tradeler çekilirken hata oluştu. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }

                return myLastFilledOrders;
            }
        }

        private static void WriteLog(string LogMessage, string account)
        {
            string filepath = @"C:\TradeBot\" + account + "\\Log.txt";

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

        private static void WriteOrderLog(string LogMessage, string account)
        {
            string filepath = @"C:\TradeBot\" + account + "\\OrderLog.txt";

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

        private static void WriteOpenOrderLog(string symbol, string LogMessage, string account)
        {
            string filepath = @"C:\TradeBot\" + account + symbol + "_OpenOrderLog.txt";

            if (File.Exists(filepath))
                File.Delete(filepath);

            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine(LogMessage);
            }
        }

        private static long ReadOpenOrderLog(string symbol, string account)
        {
            string filepath = @"C:\TradeBot\" + account + symbol + "_OpenOrderLog.txt";
            string value = string.Empty;

            if (File.Exists(filepath))
            {
                using (StreamReader rd = File.OpenText(filepath))
                {
                    while (!rd.EndOfStream)
                    {
                        value += rd.ReadLine().Trim();
                    }
                }
            }

            if (!string.IsNullOrEmpty(value))
                return long.Parse(value);
            else
                return 0;
        }

        private static async Task SendTelegramMessageAsync(string message)
        {
            await botClient.SendTextMessageAsync(environmentVariables.w, message);
        }

        private static async Task CalculateSpotWalletAsync(string account, List<Candlestick> myAvailableCandlesticks)
        {
            List<Balance> myBalances = new List<Balance>();
            Balance myCurrentUSDTBalance = new Balance();
            Balance myCurrentCoinBalance = new Balance();
            Candlestick myAvailableCurrentCandleStick = new Candlestick();
            decimal totalAmount = 0;
            decimal currentCoinUSDTAmount = 0;

            // Get Balance
            myBalances = await getBalancesAsync(mySembols, account);

            // Get USDT Balance
            myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");

            foreach (var item in mySembols)
            {
                myCurrentCoinBalance = myBalances.FirstOrDefault(i => i.Asset == item.symbolCoin);
                myAvailableCurrentCandleStick = myAvailableCandlesticks.FirstOrDefault(i => i.Symbol == item.symbol);

                // Send Coin Info
                if (myCurrentCoinBalance != null)
                    currentCoinUSDTAmount = Math.Round(((myCurrentCoinBalance.Free + myCurrentCoinBalance.Locked) * myAvailableCurrentCandleStick.Close), 2);
                else
                    currentCoinUSDTAmount = 0;

                totalAmount += currentCoinUSDTAmount;
            }

            await SendTelegramMessageAsync(string.Format("Toplam Spot Cüzdan Bakiyesi: {0} USDT", Math.Round(totalAmount + myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked, 2)));
        }

        public static async Task TradeAsync(string account)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            // Read Environment Variables
            readEnvironmentVariables(account);
            await SendTelegramMessageAsync("****************** Trade İşlemi Başlamıştır ******************");
            List<Candlestick> myCandlesticks = new List<Candlestick>();
            List<Candlestick> myAvailableCandlesticks = new List<Candlestick>();
            List<Balance> myBalances = new List<Balance>();
            Order myCurrentOpenOrder = new Order();
            Order logOrderDetail = new Order();
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
            string bulkMessage = string.Empty;
            long orderId = 0;
            //long logOrderId = 0;

            // Read Symbols
            mySembols = await readSymbolsAsync(account);

            // Get Balances
            myBalances = await getBalancesAsync(mySembols, account);

            // Read CandleSticks
            myCandlesticks = await readLastCandleSticksAsync(mySembols, account);
            myAvailableCandlesticks = await readCurrentCandleSticksAsync(mySembols, account);

            // Calculate Available Amount
            await calculateAvailableAmountAsync(mySembols, myBalances, myAvailableCandlesticks, account);

            // Get Open Orders
            List<Order> myOpenOrders = await getCurrentOpenOrdersAsync(mySembols, account);

            // Get Account Last Trades
            List<Order> myLastTrades = await getLastTradesAsync(mySembols, account);

            try
            {
                // USDT Balanace
                myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");
                if (myCurrentUSDTBalance != null)
                    await SendTelegramMessageAsync(string.Format("Long Poz miktarı: {0} USDT", Math.Round((myCurrentUSDTBalance.Free + myCurrentUSDTBalance.Locked), 2)));

                // Trade
                foreach (var item in mySembols)
                {
                    // Get Account Info && Balances
                    //logOrderId = ReadOpenOrderLog(item.symbol.ToUpper(), account);
                    myBalances = await getBalancesAsync(mySembols, account);
                    myCurrentOpenOrder = myOpenOrders.FirstOrDefault(i => i.Symbol == item.symbol.ToUpper());
                    myCurrentUSDTBalance = myBalances.FirstOrDefault(i => i.Asset == "USDT");
                    myCurrentCoinBalance = myBalances.FirstOrDefault(i => i.Asset == item.symbolCoin);
                    myCurrentLastTrade = myLastTrades.FirstOrDefault(i => i.Symbol == item.symbol.ToUpper());
                    myCurrentCandleStick = myCandlesticks.FirstOrDefault(i => i.Symbol == item.symbol);
                    myAvailableCurrentCandleStick = myAvailableCandlesticks.FirstOrDefault(i => i.Symbol == item.symbol);
                    logOrderDetail = null;
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

                    bulkMessage += string.Format("Mevcut {0} miktarı: {1} ({2} USDT) \n", item.symbolCoin, currentCoinAmount, currentCoinUSDTAmount);
                    //WriteLog(string.Format("{0} Mevcut {1} miktarı: {2} ({3} USDT)", DateTime.Now.ToString(), item.symbolCoin, currentCoinAmount, currentCoinUSDTAmount), account);

                    //if (myCurrentOpenOrder != null)
                    //{
                    //    if (logOrderId != myCurrentOpenOrder.OrderId)
                    //    {
                    //        myCurrentOpenOrder = null;

                    //        logOrderDetail = binanceClient.GetOrder(item.symbol.ToUpper(), logOrderId).Result;

                    //        if (logOrderDetail != null && logOrderDetail.Status == "NEW")
                    //            myCurrentOpenOrder = logOrderDetail;
                    //        else
                    //            WriteOpenOrderLog(item.symbol.ToUpper(), string.Empty, account);
                    //    }
                    //}
                    //else
                    //{
                    //    if (logOrderId != 0)
                    //    {
                    //        logOrderDetail = binanceClient.GetOrder(item.symbol.ToUpper(), logOrderId).Result;

                    //        if (logOrderDetail != null && logOrderDetail.Status == "NEW")
                    //            myCurrentOpenOrder = logOrderDetail;
                    //        else
                    //            WriteOpenOrderLog(item.symbol.ToUpper(), string.Empty, account);
                    //    }
                    //}

                    // Case 1
                    if ((myCurrentLastTrade == null || (myCurrentLastTrade.Side == "SELL")) && myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && myCurrentOpenOrder == null && availableBuyAmount > 10.02M && myCurrentUSDTBalance.Free > availableBuyAmount)
                    {
                        if ((myCurrentCandleStick.SupportLine - (myCurrentCandleStick.SupportLine * item.buyRatio)) > myAvailableCurrentCandleStick.High)
                            buyPrice = Math.Round(myAvailableCurrentCandleStick.Close + (myAvailableCurrentCandleStick.Close * 0.002M), item.priceRound);
                        else
                            buyPrice = Math.Round((myCurrentCandleStick.SupportLine - (myCurrentCandleStick.SupportLine * item.buyRatio)), item.priceRound);

                        buyQuantity = Math.Round((availableBuyAmount / buyPrice), item.quantityRound);

                        myNewOrder = await binanceClient.PostNewOrder(item.symbol, buyQuantity, buyPrice, OrderSide.BUY);
                        orderAmount = Math.Round(buyQuantity * buyPrice, item.priceRound);

                        //WriteOpenOrderLog(item.symbol.ToUpper(), myNewOrder.OrderId.ToString(), account);

                        bulkMessage += string.Format("{0} için {1} adet ve {2} fiyattan ALIM emri girilmiştir. İşlem hacmi {3} \n", item.symbol.ToUpper(), buyQuantity, buyPrice, orderAmount);
                        WriteOrderLog(string.Format("{0};AL;{1};{2};{3};{4}", item.symbol.ToUpper(), buyQuantity, buyPrice, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);
                    } // Case 2
                    else if (myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "SELL"))
                    {
                        orderId = myCurrentOpenOrder.OrderId;

                        myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                        //WriteOpenOrderLog(item.symbol.ToUpper(), string.Empty, account);

                        bulkMessage += string.Format("{0} için SATIŞ emri İPTAL edilmiştir. Order Id: {1} \n", item.symbol.ToUpper(), orderId);
                        WriteOrderLog(string.Format("{0};IPTAL;{1};{2};{3};{4}", item.symbol.ToUpper(), myCurrentOpenOrder.OrigQty, myCurrentOpenOrder.Price, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);
                    } // Case 3
                    else if (myCurrentCandleStick.SupportLine > myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "BUY"))
                    {
                        if ((myCurrentCandleStick.SupportLine - (myCurrentCandleStick.SupportLine * item.buyRatio)) > myAvailableCurrentCandleStick.High)
                            buyPrice = Math.Round(myAvailableCurrentCandleStick.Close + (myAvailableCurrentCandleStick.Close * 0.002M), item.priceRound);
                        else
                            buyPrice = Math.Round((myCurrentCandleStick.SupportLine - (myCurrentCandleStick.SupportLine * item.buyRatio)), item.priceRound);

                        if (myCurrentOpenOrder.Price != buyPrice)
                        {
                            orderId = myCurrentOpenOrder.OrderId;

                            myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                            WriteOrderLog(string.Format("{0};IPTAL;{1};{2};{3};{4}", item.symbol.ToUpper(), myCurrentOpenOrder.OrigQty, myCurrentOpenOrder.Price, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);

                            // Calculate Quantity 
                            buyQuantity = Math.Round((availableBuyAmount / buyPrice), item.quantityRound);

                            myNewOrder = await binanceClient.PostNewOrder(item.symbol, buyQuantity, buyPrice, OrderSide.BUY);
                            orderAmount = Math.Round(buyQuantity * buyPrice, 2);

                            //WriteOpenOrderLog(item.symbol.ToUpper(), myNewOrder.OrderId.ToString(), account);

                            bulkMessage += string.Format("{0} için önceki verilen ALIM emri İPTAL edilmiştir. (Order Id: {1}) - {2} adet ve {3} fiyattan ALIM emri güncellenmiştir. İşlem Hacmi {4} \n", item.symbol.ToUpper(), orderId, buyQuantity, buyPrice, orderAmount);
                            WriteOrderLog(string.Format("{0};AL;{1};{2};{3};{4}", item.symbol.ToUpper(), buyQuantity, buyPrice, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);
                        }
                        else
                        {
                            bulkMessage += string.Format("{0} için mevcuttaki ALIM emri GÜNCELLENMEMİŞTİR. Mevcut ALIM fiyatı {1} \n", item.symbol.ToUpper(), buyPrice);
                        }
                    } // Case 4
                    else if ((myCurrentLastTrade != null && myCurrentLastTrade.Side == "BUY") && myCurrentCandleStick.SupportLine < myCurrentCandleStick.OTTLine && myCurrentOpenOrder == null && (myCurrentCoinBalance != null && myCurrentCoinBalance.Free > 0))
                    {
                        if ((myCurrentCandleStick.SupportLine + (myCurrentCandleStick.SupportLine * item.sellRatio)) < myAvailableCurrentCandleStick.Low)
                            sellPrice = Math.Round(myAvailableCurrentCandleStick.Close - (myAvailableCurrentCandleStick.Close * 0.002M), item.priceRound);
                        else
                            sellPrice = Math.Round((myCurrentCandleStick.SupportLine + (myCurrentCandleStick.SupportLine * item.sellRatio)), item.priceRound);

                        sellQuantity = Math.Round(myCurrentCoinBalance.Free - (decimal)Math.Pow(10, (item.quantityRound * -1)), item.quantityRound);

                        myNewOrder = await binanceClient.PostNewOrder(item.symbol, sellQuantity, sellPrice, OrderSide.SELL);
                        orderAmount = Math.Round(sellQuantity * sellPrice, item.priceRound);

                        //WriteOpenOrderLog(item.symbol.ToUpper(), myNewOrder.OrderId.ToString(), account);

                        bulkMessage += string.Format("{0} için {1} adet ve {2} fiyattan SATIŞ emri girilmiştir. İşlem hacmi {3} \n", item.symbol.ToUpper(), sellQuantity, sellPrice, orderAmount);
                        WriteOrderLog(string.Format("{0};SAT;{1};{2};{3};{4}", item.symbol.ToUpper(), sellQuantity, sellPrice, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);
                    } // Case 5
                    else if (myCurrentCandleStick.SupportLine < myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "BUY"))
                    {
                        orderId = myCurrentOpenOrder.OrderId;

                        myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                        //WriteOpenOrderLog(item.symbol.ToUpper(), string.Empty, account);

                        bulkMessage += string.Format("{0} için ALIM emri İPTAL edilmiştir. Order Id: {1} \n", item.symbol.ToUpper(), orderId);
                        WriteOrderLog(string.Format("{0};IPTAL;{1};{2};{3};{4}", item.symbol.ToUpper(), myCurrentOpenOrder.OrigQty, myCurrentOpenOrder.Price, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);
                    } // Case 6
                    else if (myCurrentCandleStick.SupportLine < myCurrentCandleStick.OTTLine && (myCurrentOpenOrder != null && myCurrentOpenOrder.Side == "SELL"))
                    {
                        if ((myCurrentCandleStick.SupportLine + (myCurrentCandleStick.SupportLine * item.sellRatio)) < myAvailableCurrentCandleStick.Low)
                            sellPrice = Math.Round(myAvailableCurrentCandleStick.Close - (myAvailableCurrentCandleStick.Close * 0.002M), item.priceRound);
                        else
                            sellPrice = Math.Round((myCurrentCandleStick.SupportLine + (myCurrentCandleStick.SupportLine * item.sellRatio)), item.priceRound);

                        if (myCurrentOpenOrder.Price != sellPrice)
                        {
                            orderId = myCurrentOpenOrder.OrderId;

                            myCancelOrder = await binanceClient.CancelOrder(item.symbol, orderId);

                            WriteOrderLog(string.Format("{0};IPTAL;{1};{2};{3};{4}", item.symbol.ToUpper(), myCurrentOpenOrder.OrigQty, myCurrentOpenOrder.Price, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);

                            sellQuantity = Math.Round((myCurrentCoinBalance.Free + myCurrentCoinBalance.Locked) - (decimal)Math.Pow(10, (item.quantityRound * -1)), item.quantityRound);

                            myNewOrder = await binanceClient.PostNewOrder(item.symbol, sellQuantity, sellPrice, OrderSide.SELL);
                            orderAmount = Math.Round(sellQuantity * sellPrice, item.priceRound);

                            //WriteOpenOrderLog(item.symbol.ToUpper(), myNewOrder.OrderId.ToString(), account);

                            bulkMessage += string.Format("{0} için önceki verilen SATIŞ emri İPTAL edilmiştir. (Order Id: {1}) - {2} adet ve {3} fiyattan SATIŞ emri güncellenmiştir. İşlem Hacmi {4} \n", item.symbol.ToUpper(), orderId, sellQuantity, sellPrice, orderAmount);
                            WriteOrderLog(string.Format("{0};SAT;{1};{2};{3};{4}", item.symbol.ToUpper(), sellQuantity, sellPrice, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), account);
                        }
                        else
                        {
                            bulkMessage += string.Format("{0} için mevcuttaki SATIŞ emri GÜNCELLENMEMİŞTİR. Mevcut SATIŞ fiyatı {1} \n", item.symbol.ToUpper(), sellPrice);
                        }
                    }
                    else
                    {
                        bulkMessage += string.Format("{0} için bu periyotta herhangi bir işlem yapılmamıştır \n", item.symbol.ToUpper());
                    }
                }

                if (!string.IsNullOrEmpty(bulkMessage))
                    await SendTelegramMessageAsync(bulkMessage);

                await CalculateSpotWalletAsync(account, myAvailableCandlesticks);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    await SendTelegramMessageAsync(string.Format("Trade işlemi sırasında hata oluşmuştır. Hata: {0}", ex.InnerException.Message));
                    WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Trade işlemi sırasında hata oluşmuştır. Hata: {0}", ex.Message));
                    WriteLog(ex.Message, account);
                }
            }
        }
    }
}