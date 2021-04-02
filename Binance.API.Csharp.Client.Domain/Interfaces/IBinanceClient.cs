using Binance.API.Csharp.Client.Domain.Abstract;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.General;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.UserStream;
using Binance.API.Csharp.Client.Models.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binance.API.Csharp.Client.Domain.Interfaces
{
    public interface IBinanceClient
    {
        Task<object> TestConnectivity();

        Task<ServerInfo> GetServerTime();

        Task<OrderBook> GetOrderBook(string symbol, int limit = 100);

        Task<IEnumerable<AggregateTrade>> GetAggregateTrades(
          string symbol,
          int limit = 500);

        Task<IEnumerable<Candlestick>> GetCandleSticks(
          string symbol,
          TimeInterval interval,
          DateTime? startTime = null,
          DateTime? endTime = null,
          int limit = 500);

        Task<IEnumerable<PriceChangeInfo>> GetPriceChange24H(string symbol);

        Task<IEnumerable<SymbolPrice>> GetAllPrices();

        Task<IEnumerable<OrderBookTicker>> GetOrderBookTicker();

        Task<NewOrder> PostNewOrder(
          string symbol,
          Decimal quantity,
          Decimal price,
          OrderSide side,
          OrderType orderType = OrderType.LIMIT,
          TimeInForce timeInForce = TimeInForce.GTC,
          Decimal icebergQty = 0M,
          long recvWindow = 6000000);

        Task<object> PostNewOrderTest(
          string symbol,
          Decimal quantity,
          Decimal price,
          OrderSide side,
          OrderType orderType = OrderType.LIMIT,
          TimeInForce timeInForce = TimeInForce.GTC,
          Decimal icebergQty = 0M,
          long recvWindow = 6000000);

        Task<Order> GetOrder(
          string symbol,
          long? orderId = null,
          string origClientOrderId = null,
          long recvWindow = 6000000);

        Task<CanceledOrder> CancelOrder(
          string symbol,
          long? orderId = null,
          string origClientOrderId = null,
          long recvWindow = 6000000);

        Task<IEnumerable<Order>> GetCurrentOpenOrders(
          string symbol,
          long recvWindow = 6000000);

        Task<IEnumerable<Order>> GetAllOrders(
          string symbol,
          long? orderId = null,
          int limit = 500,
          long recvWindow = 6000000);

        Task<AccountInfo> GetAccountInfo(long recvWindow = 6000000);

        Task<IEnumerable<Trade>> GetTradeList(string symbol, long recvWindow = 6000000);

        Task<WithdrawResponse> Withdraw(
          string asset,
          Decimal amount,
          string address,
          string addressName = "",
          long recvWindow = 6000000);

        Task<DepositHistory> GetDepositHistory(
          string asset,
          DepositStatus? status = null,
          DateTime? startTime = null,
          DateTime? endTime = null,
          long recvWindow = 6000000);

        Task<WithdrawHistory> GetWithdrawHistory(
          string asset,
          WithdrawStatus? status = null,
          DateTime? startTime = null,
          DateTime? endTime = null,
          long recvWindow = 6000000);

        Task<UserStreamInfo> StartUserStream();

        Task<object> KeepAliveUserStream(string listenKey);

        Task<object> CloseUserStream(string listenKey);

        void ListenDepthEndpoint(
          string symbol,
          ApiClientAbstract.MessageHandler<DepthMessage> messageHandler);

        void ListenKlineEndpoint(
          string symbol,
          TimeInterval interval,
          ApiClientAbstract.MessageHandler<KlineMessage> messageHandler);

        void ListenTradeEndpoint(
          string symbol,
          ApiClientAbstract.MessageHandler<AggregateTradeMessage> messageHandler);

        string ListenUserDataEndpoint(
          ApiClientAbstract.MessageHandler<AccountUpdatedMessage> accountInfoHandler,
          ApiClientAbstract.MessageHandler<OrderOrTradeUpdatedMessage> tradesHandler,
          ApiClientAbstract.MessageHandler<OrderOrTradeUpdatedMessage> ordersHandler);
    }
}
