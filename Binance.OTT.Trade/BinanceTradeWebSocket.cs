using Binance.API.Csharp.Client;
using Binance.UtilitiesLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Binance.OTT.Trade
{
    public class BinanceTradeWebSocket
    {
        private static EnvironmentVariables environmentVariables = new EnvironmentVariables();
        private static TelegramBotClient botClient;
        private static List<Symbol> mySembols = new List<Symbol>();

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

            //apiClient = new ApiClient(environmentVariables.x, environmentVariables.y);
            //binanceClient = new BinanceClient(apiClient);
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
                        tmp.reviewPeriodLength = int.Parse(str.Split(';')[6]);
                        tmp.profitRatio = Decimal.Parse(str.Split(';')[7]);
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
                    //WriteLog(ex.InnerException.Message, account);
                }
                else
                {
                    await SendTelegramMessageAsync(string.Format("Sembol listesi okunurken hata oluştu. Hata: {0}", ex.Message));
                    //WriteLog(ex.Message, account);
                }

                return symbolsList;
            }
        }

        private static async Task SendTelegramMessageAsync(string message)
        {
            await botClient.SendTextMessageAsync(environmentVariables.w, message);
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
                        symbolCandleStick = rd.ReadToEnd().Trim();
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
                        symbolCandleStick = rd.ReadToEnd().Trim();
                        lineNumber = symbolCandleStick.Split('\n').Length;
                        lastCandleStickStr = symbolCandleStick.Split('\n')[lineNumber - 1];
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

        private static async Task<List<Candlestick>> readReviewPeriodCandleSticksAsync(List<Symbol> symbols, string account)
        {
            List<Candlestick> candlestickList = new List<Candlestick>();

            try
            {
                string filepath = string.Empty;
                int lineNumber = 0;
                string symbolCandleStick = string.Empty;
                string reviewCandleStick = string.Empty;
                Candlestick lastCandleStick = new Candlestick();

                foreach (var item in symbols)
                {
                    filepath = @"C:\TradeBot\COMMON\" + item.symbol + ".txt";
                    lineNumber = 0;
                    symbolCandleStick = string.Empty;
                    lastCandleStick = new Candlestick();

                    if (item.reviewPeriodLength > 0 && item.profitRatio > 0)
                    {
                        using (StreamReader rd = File.OpenText(filepath))
                        {
                            symbolCandleStick = rd.ReadToEnd().Trim();
                            lineNumber = symbolCandleStick.Split('\n').Length;
                            reviewCandleStick = symbolCandleStick.Split('\n')[lineNumber - (item.reviewPeriodLength + 1)];
                            // Read Lines
                            lastCandleStick.Symbol = item.symbol;
                            lastCandleStick.OpenDateTime = DateTime.Parse(reviewCandleStick.Split(';')[0]);
                            lastCandleStick.Open = (decimal)(Decimal.Parse(reviewCandleStick.Split(';')[1]));
                            lastCandleStick.High = (decimal)(Decimal.Parse(reviewCandleStick.Split(';')[2]));
                            lastCandleStick.Low = (decimal)(Decimal.Parse(reviewCandleStick.Split(';')[3]));
                            lastCandleStick.Close = (decimal)(Decimal.Parse(reviewCandleStick.Split(';')[4]));
                            lastCandleStick.SupportLine = (decimal)(Decimal.Parse(reviewCandleStick.Split(';')[5]));
                            lastCandleStick.OTTLine = (decimal)(Decimal.Parse(reviewCandleStick.Split(';')[6]));
                            lastCandleStick.BuySignal = reviewCandleStick.Split(';')[7] == "0" ? false : true;
                            lastCandleStick.SellSignal = reviewCandleStick.Split(';')[8] == "0" ? false : true;
                            candlestickList.Add(lastCandleStick);
                        }
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

        private static async Task<List<Order>> getCurrentOpenOrdersAsync(List<Symbol> symbols, string account)
        {
            List<Order> myOpenOrders = new List<Order>();
            List<Order> myCurrentOpenOrders = new List<Order>();

            try
            {
                foreach (var item in symbols)
                {
                    myCurrentOpenOrders = null; //binanceClient.GetCurrentOpenOrders(item.symbol).Result.ToList();

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
                    myCurrentOrder = null; //binanceClient.GetAllOrders(item.symbol).Result.ToList();

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

        public class Order
        {
            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("orderId")]
            public long OrderId { get; set; }

            [JsonProperty("clientOrderId")]
            public string ClientOrderId { get; set; }

            [JsonProperty("price")]
            public Decimal Price { get; set; }

            [JsonProperty("origQty")]
            public Decimal OrigQty { get; set; }

            [JsonProperty("executedQty")]
            public Decimal ExecutedQty { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("timeInForce")]
            public string TimeInForce { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("side")]
            public string Side { get; set; }

            [JsonProperty("stopPrice")]
            public Decimal StopPrice { get; set; }

            [JsonProperty("icebergQty")]
            public Decimal IcebergQty { get; set; }

            [JsonProperty("time")]
            public long Time { get; set; }
        }

        public class Balance
        {
            public string Asset { get; set; }

            public Decimal Free { get; set; }

            public Decimal Locked { get; set; }
        }

        public static async Task TradeAsync(string account)
        {
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            //// Read Environment Variables
            //readEnvironmentVariables(account);
            //await SendTelegramMessageAsync("****************** Trade İşlemi Başlamıştır ******************");

            //var client = new BinanceClient(new BinanceClientOptions
            //{
            //    ApiCredentials = new ApiCredentials(environmentVariables.x, environmentVariables.y)
            //});

            //var startResult = client.Spot.UserStream.StartUserStream();

            //if (!startResult.Success)
            //    throw new Exception($"Failed to start user stream: {startResult.Error}");

            //client.Spot.Order.CancelAllOpenOrders("");

            //var socketClient = new BinanceSocketClient();

            //socketClient.Spot.SubscribeToUserDataUpdates(startResult.Data,
            //   data =>
            //   {
                   
            //   },
            //   data =>
            //   {

            //   },
            //   data =>
            //   {

            //   },
            //   data =>
            //   {

            //   });
        }
    }
}
