using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.API.Csharp.Client.Utils
{
    public static class EndPoints
    {
        public static readonly string TestConnectivity = "/api/v1/ping";
        public static readonly string CheckServerTime = "/api/v1/time";
        public static readonly string OrderBook = "/api/v1/depth";
        public static readonly string AggregateTrades = "/api/v1/aggTrades";
        public static readonly string Candlesticks = "/api/v3/klines";
        public static readonly string TickerPriceChange24H = "/api/v1/ticker/24hr";
        public static readonly string AllPrices = "/api/v1/ticker/allPrices";
        public static readonly string OrderBookTicker = "/api/v1/ticker/allBookTickers";
        public static readonly string TradingRules = "https://gist.githubusercontent.com/Ninj0r/3029b9d635f8f81f5ffab9cc9df5cc61/raw/810530a2118e5d8cdcfcc4d220349976a0acf131/tradingRules_20171022.json";
        public static readonly string NewOrder = "/api/v3/order";
        public static readonly string NewOrderTest = "/api/v3/order/test";
        public static readonly string QueryOrder = "/api/v3/order";
        public static readonly string CancelOrder = "/api/v3/order";
        public static readonly string CurrentOpenOrders = "/api/v3/openOrders";
        public static readonly string AllOrders = "/api/v3/allOrders";
        public static readonly string AccountInformation = "/api/v3/account";
        public static readonly string TradeList = "/api/v3/myTrades";
        public static readonly string Withdraw = "/wapi/v1/withdraw.html";
        public static readonly string DepositHistory = "/wapi/v1/getDepositHistory.html";
        public static readonly string WithdrawHistory = "/wapi/v1/getWithdrawHistory.html";
        public static readonly string StartUserStream = "/api/v1/userDataStream";
        public static readonly string KeepAliveUserStream = "/api/v1/userDataStream";
        public static readonly string CloseUserStream = "/api/v1/userDataStream";
    }
}
