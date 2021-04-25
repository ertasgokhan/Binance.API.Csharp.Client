using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Binance.UtilitiesLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Binance.Generate.OTT
{
    public static class GenerateOTTLine
    {
        private const int limit = 1000;
        private static EnvironmentVariables environmentVariables = new EnvironmentVariables();
        private static ApiClient apiClient = new ApiClient("", "");
        private static BinanceClient binanceClient = new BinanceClient(apiClient);
        private static TelegramBotClient botClient;

        private static async Task SendTelegramMessageAsync(string message)
        {
            await botClient.SendTextMessageAsync(environmentVariables.w, message);
        }

        private static async Task<List<Symbol>> readSymbolsAsync(string account)
        {
            // Read Environment Variables
            ReadEnvironmentVariables(account);

            List<Symbol> symbolsList = new List<Symbol>();
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
                    symbolsList.Add(tmp);
                }
            }

            await SendTelegramMessageAsync(string.Format("OTTLine Generate için tüm Symboller dosyadan okunmuştur"));

            return symbolsList;
        }

        private static void ReadEnvironmentVariables(string account)
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

        private static async Task GetForOnePairAsync(Symbol symbolItem, string account)
        {
            try
            {
                // Read Environment Variables
                ReadEnvironmentVariables(account);

                string symbol = symbolItem.symbol;
                int Length = symbolItem.length;
                decimal Percent = symbolItem.percent;

                string filepath = @"C:\TradeBot\" + account + symbol + ".txt";
                string OTTLines = string.Empty;
                List<Candlestick> candlestick = new List<Candlestick>();
                List<Candlestick> tempCandlestick = new List<Candlestick>();

                for (int i = -15; i < 0; i++)
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

                await SendTelegramMessageAsync(string.Format("{0} için mum verileri başarıyla okunmuştur", symbol.ToUpper()));
            }
            catch (Exception ex)
            {
                await SendTelegramMessageAsync(string.Format("{0} için mum verileri okunma sırasında hata alınmıştır. {1}", symbolItem.symbol.ToUpper(), ((System.IO.FileLoadException)ex.InnerException).Message));
                WriteLog(((System.IO.FileLoadException)ex.InnerException).Message, account);
            }
        }

        private static string ReturnOTT(List<Candlestick> candlestick, int length, decimal percent)
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

        private static void SendEmail(string symbol, string account)
        {
            string toAddress = "";
            string fromPassword = "";
            string filepath = @"C:\TradeBot\" + account + "emails.txt";
            string filepath2 = @"C:\TradeBot\" + account + "emailsPass.txt";

            using (StreamReader rd = File.OpenText(filepath))
            {
                while (!rd.EndOfStream)
                {
                    toAddress = rd.ReadLine();
                }
            }

            using (StreamReader rd = File.OpenText(filepath2))
            {
                while (!rd.EndOfStream)
                {
                    fromPassword = rd.ReadLine();
                }
            }

            string fromAddress = "definexbinance@gmail.com";
            string subject = symbol + " mum verileri hakkında";
            string body = DateTime.Now + " tarihinde " + symbol + " için mum verileri başarıyla çekilmiştir.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public static async Task GenerateOTT(string account)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

            // Read Symbols
            List<Symbol> symbolsList = await readSymbolsAsync(account);

            // Generate OTT Lines
            foreach (var item in symbolsList)
            {
                await GetForOnePairAsync(item, account);
            }
        }
    }
}
