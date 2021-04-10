using Binance.API.Csharp.Client.Domain.Abstract;
using Binance.API.Csharp.Client.Domain.Interfaces;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.General;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.Market.TradingRules;
using Binance.API.Csharp.Client.Models.UserStream;
using Binance.API.Csharp.Client.Models.WebSocket;
using Binance.API.Csharp.Client.Utils;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Binance.API.Csharp.Client.Domain.Abstract.ApiClientAbstract;

namespace Binance.API.Csharp.Client
{
    public class BinanceClient : BinanceClientAbstract, IBinanceClient
    {
        public BinanceClient(IApiClient apiClient, bool loadTradingRules = false)
      : base(apiClient)
        {
            if (!loadTradingRules)
                return;
            this.LoadTradingRules();
        }


        private void ValidateOrderValue(
      string symbol,
      OrderType orderType,
      Decimal unitPrice,
      Decimal quantity,
      Decimal icebergQty)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Invalid symbol. ", nameof(symbol));
            if (quantity <= 0M)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            if (orderType == OrderType.LIMIT && unitPrice <= 0M)
                throw new ArgumentException("Price must be greater than zero.", "price");
            if (this._tradingRules == null)
                return;
            Symbol symbol1 = this._tradingRules.Symbols.Where<Symbol>((Func<Symbol, bool>)(r => r.SymbolName.ToUpper() == symbol.ToUpper())).FirstOrDefault<Symbol>();
            Binance.API.Csharp.Client.Models.Market.TradingRules.Filter filter1 = symbol1.Filters.Where<Binance.API.Csharp.Client.Models.Market.TradingRules.Filter>((Func<Binance.API.Csharp.Client.Models.Market.TradingRules.Filter, bool>)(r => r.FilterType == "PRICE_FILTER")).FirstOrDefault<Binance.API.Csharp.Client.Models.Market.TradingRules.Filter>();
            Binance.API.Csharp.Client.Models.Market.TradingRules.Filter filter2 = symbol1.Filters.Where<Binance.API.Csharp.Client.Models.Market.TradingRules.Filter>((Func<Binance.API.Csharp.Client.Models.Market.TradingRules.Filter, bool>)(r => r.FilterType == "LOT_SIZE")).FirstOrDefault<Binance.API.Csharp.Client.Models.Market.TradingRules.Filter>();
            if (symbol1 == null)
                throw new ArgumentException("Invalid symbol. ", nameof(symbol));
            if (quantity < filter2.MinQty)
                throw new ArgumentException(string.Format("Quantity for this symbol is lower than allowed! Quantity must be greater than: {0}", (object)filter2.MinQty), nameof(quantity));
            if (icebergQty > 0M && !symbol1.IcebergAllowed)
                throw new Exception("Iceberg orders not allowed for this symbol.");
            if (orderType == OrderType.LIMIT && unitPrice < filter1.MinPrice)
                throw new ArgumentException(string.Format("Price for this symbol is lower than allowed! Price must be greater than: {0}", (object)filter1.MinPrice), "price");
        }
        private void LoadTradingRules()
        {
            ApiClient apiClient = new ApiClient("", "", EndPoints.TradingRules, "wss://stream.binance.com:9443/ws/", addDefaultHeaders: false);
            base._tradingRules = apiClient.CallAsync<TradingRules>((ApiMethod)1, "").Result;
        }

        public async Task<dynamic> TestConnectivity()
        {
            return await base._apiClient.CallAsync<object>((ApiMethod)1, EndPoints.TestConnectivity, false, (string)null);
        }

        public async Task<ServerInfo> GetServerTime()
        {
            return await base._apiClient.CallAsync<ServerInfo>((ApiMethod)1, EndPoints.CheckServerTime, false, (string)null);
        }

        public async Task<OrderBook> GetOrderBook(string symbol, int limit = 100)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            dynamic result = await base._apiClient.CallAsync<object>((ApiMethod)1, EndPoints.OrderBook, false, $"symbol={symbol.ToUpper()}&limit={limit}");
            CustomParser parser = new CustomParser();
            dynamic parsedResult = parser.GetParsedOrderBook(result);
            return (OrderBook)parsedResult;
        }

        public async Task<IEnumerable<AggregateTrade>> GetAggregateTrades(string symbol, int limit = 500)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            return await base._apiClient.CallAsync<IEnumerable<AggregateTrade>>((ApiMethod)1, EndPoints.AggregateTrades, false, $"symbol={symbol.ToUpper()}&limit={limit}");
        }

        public async Task<IEnumerable<Candlestick>> GetCandleSticks(string symbol, TimeInterval interval, DateTime? startTime = null, DateTime? endTime = null, int limit = 500)
        {
            //IL_0015: Unknown result type (might be due to invalid IL or missing references)
            //IL_0016: Unknown result type (might be due to invalid IL or missing references)
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            string args = $"symbol={symbol.ToUpper()}&interval={((Enum)(object)interval).GetDescription()}" + (startTime.HasValue ? $"&startTime={startTime.Value.GetUnixTimeStamp()}" : "") + (endTime.HasValue ? $"&endTime={endTime.Value.GetUnixTimeStamp()}" : "") + $"&limit={limit}";
            dynamic result = await base._apiClient.CallAsync<object>((ApiMethod)1, EndPoints.Candlesticks, false, args);
            CustomParser parser = new CustomParser();
            dynamic parsedResult = parser.GetParsedCandlestick(result);
            return parsedResult;
        }

        public async Task<IEnumerable<PriceChangeInfo>> GetPriceChange24H(string symbol = "")
        {
            string args = (string.IsNullOrWhiteSpace(symbol) ? "" : $"symbol={symbol.ToUpper()}");
            List<PriceChangeInfo> result = new List<PriceChangeInfo>();
            if (!string.IsNullOrEmpty(symbol))
            {
                result.Add(await base._apiClient.CallAsync<PriceChangeInfo>((ApiMethod)1, EndPoints.TickerPriceChange24H, false, args));
            }
            else
            {
                result = await base._apiClient.CallAsync<List<PriceChangeInfo>>((ApiMethod)1, EndPoints.TickerPriceChange24H, false, args);
            }
            return result;
        }

        public async Task<IEnumerable<SymbolPrice>> GetAllPrices()
        {
            return await base._apiClient.CallAsync<IEnumerable<SymbolPrice>>((ApiMethod)1, EndPoints.AllPrices, false, (string)null);
        }

        public async Task<IEnumerable<OrderBookTicker>> GetOrderBookTicker()
        {
            return await base._apiClient.CallAsync<IEnumerable<OrderBookTicker>>((ApiMethod)1, EndPoints.OrderBookTicker, false, (string)null);
        }

        public async Task<NewOrder> PostNewOrder(string symbol, decimal quantity, decimal price, OrderSide side, OrderType orderType = 0, TimeInForce timeInForce = 0, decimal icebergQty = 0m, long recvWindow = 5000L)
        {
            //IL_0023: Unknown result type (might be due to invalid IL or missing references)
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            //IL_002b: Unknown result type (might be due to invalid IL or missing references)
            //IL_002d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_0035: Unknown result type (might be due to invalid IL or missing references)
            ValidateOrderValue(symbol, orderType, price, quantity, icebergQty);
            string args = $"symbol={symbol.ToUpper()}&side={side}&type={orderType}&quantity={quantity}" + (((int)orderType == 0) ? $"&timeInForce={timeInForce}" : "") + (((int)orderType == 0) ? $"&price={price}" : "") + ((icebergQty > 0m) ? $"&icebergQty={icebergQty}" : "") + $"&recvWindow={recvWindow}";
            return await base._apiClient.CallAsync<NewOrder>((ApiMethod)0, EndPoints.NewOrder, true, args.Replace(',', '.'));
        }

        public async Task<dynamic> PostNewOrderTest(string symbol, decimal quantity, decimal price, OrderSide side, OrderType orderType = 0, TimeInForce timeInForce = 0, decimal icebergQty = 0m, long recvWindow = 5000L)
        {
            //IL_0023: Unknown result type (might be due to invalid IL or missing references)
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            //IL_002b: Unknown result type (might be due to invalid IL or missing references)
            //IL_002d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_0035: Unknown result type (might be due to invalid IL or missing references)
            ValidateOrderValue(symbol, orderType, price, quantity, icebergQty);
            string args = $"symbol={symbol.ToUpper()}&side={side}&type={orderType}&quantity={quantity}" + (((int)orderType == 0) ? $"&timeInForce={timeInForce}" : "") + (((int)orderType == 0) ? $"&price={price}" : "") + ((icebergQty > 0m) ? $"&icebergQty={icebergQty}" : "") + $"&recvWindow={recvWindow}";
            return await base._apiClient.CallAsync<object>((ApiMethod)0, EndPoints.NewOrderTest, true, args);
        }

        public async Task<Order> GetOrder(string symbol, long? orderId = null, string origClientOrderId = null, long recvWindow = 5000L)
        {
            string args2 = $"symbol={symbol.ToUpper()}&recvWindow={recvWindow}";
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            if (orderId.HasValue)
            {
                args2 += $"&orderId={orderId.Value}";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(origClientOrderId))
                {
                    throw new ArgumentException("Either orderId or origClientOrderId must be sent.");
                }
                args2 += $"&origClientOrderId={origClientOrderId}";
            }
            return await base._apiClient.CallAsync<Order>((ApiMethod)1, EndPoints.QueryOrder, true, args2);
        }

        public async Task<CanceledOrder> CancelOrder(string symbol, long? orderId = null, string origClientOrderId = null, long recvWindow = 5000L)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            string args2 = $"symbol={symbol.ToUpper()}&recvWindow={recvWindow}";
            if (orderId.HasValue)
            {
                args2 += $"&orderId={orderId.Value}";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(origClientOrderId))
                {
                    throw new ArgumentException("Either orderId or origClientOrderId must be sent.");
                }
                args2 += $"&origClientOrderId={origClientOrderId}";
            }
            return await base._apiClient.CallAsync<CanceledOrder>((ApiMethod)3, EndPoints.CancelOrder, true, args2);
        }

        public async Task<IEnumerable<Order>> GetCurrentOpenOrders(string symbol, long recvWindow = 5000L)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            return await base._apiClient.CallAsync<IEnumerable<Order>>((ApiMethod)1, EndPoints.CurrentOpenOrders, true, $"symbol={symbol.ToUpper()}&recvWindow={recvWindow}");
        }

        public async Task<IEnumerable<Order>> GetAllOrders(string symbol, long? orderId = null, int limit = 500, long recvWindow = 5000L)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            return await base._apiClient.CallAsync<IEnumerable<Order>>((ApiMethod)1, EndPoints.AllOrders, true, $"symbol={symbol.ToUpper()}&limit={limit}&recvWindow={recvWindow}" + (orderId.HasValue ? $"&orderId={orderId.Value}" : ""));
        }

        public async Task<AccountInfo> GetAccountInfo(long recvWindow = 5000L)
        {
            return await base._apiClient.CallAsync<AccountInfo>((ApiMethod)1, EndPoints.AccountInformation, true, $"recvWindow={recvWindow}");
        }

        public async Task<IEnumerable<Trade>> GetTradeList(string symbol, long recvWindow = 5000L)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            return await base._apiClient.CallAsync<IEnumerable<Trade>>((ApiMethod)1, EndPoints.TradeList, true, $"symbol={symbol.ToUpper()}&recvWindow={recvWindow}");
        }

        public async Task<WithdrawResponse> Withdraw(string asset, decimal amount, string address, string addressName = "", long recvWindow = 5000L)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be empty. ", "asset");
            }
            if (amount <= 0m)
            {
                throw new ArgumentException("amount must be greater than zero.", "amount");
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("address cannot be empty. ", "address");
            }
            string args = $"asset={asset.ToUpper()}&amount={amount}&address={address}" + ((!string.IsNullOrWhiteSpace(addressName)) ? $"&name={addressName}" : "") + $"&recvWindow={recvWindow}";
            return await base._apiClient.CallAsync<WithdrawResponse>((ApiMethod)0, EndPoints.Withdraw, true, args);
        }

        public async Task<DepositHistory> GetDepositHistory(string asset, DepositStatus? status = null, DateTime? startTime = null, DateTime? endTime = null, long recvWindow = 5000L)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be empty. ", "asset");
            }
            string args = $"asset={asset.ToUpper()}" + (status.HasValue ? $"&status={(int)status.Value}" : "") + (startTime.HasValue ? $"&startTime={startTime.Value.GetUnixTimeStamp()}" : "") + (endTime.HasValue ? $"&endTime={endTime.Value.GetUnixTimeStamp()}" : "") + $"&recvWindow={recvWindow}";
            return await base._apiClient.CallAsync<DepositHistory>((ApiMethod)0, EndPoints.DepositHistory, true, args);
        }

        public async Task<WithdrawHistory> GetWithdrawHistory(string asset, WithdrawStatus? status = null, DateTime? startTime = null, DateTime? endTime = null, long recvWindow = 5000L)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be empty. ", "asset");
            }
            string args = $"asset={asset.ToUpper()}" + (status.HasValue ? $"&status={(int)status.Value}" : "") + (startTime.HasValue ? $"&startTime={Utilities.GenerateTimeStamp(startTime.Value)}" : "") + (endTime.HasValue ? $"&endTime={Utilities.GenerateTimeStamp(endTime.Value)}" : "") + $"&recvWindow={recvWindow}";
            return await base._apiClient.CallAsync<WithdrawHistory>((ApiMethod)0, EndPoints.WithdrawHistory, true, args);
        }

        public async Task<UserStreamInfo> StartUserStream()
        {
            return await base._apiClient.CallAsync<UserStreamInfo>((ApiMethod)0, EndPoints.StartUserStream, false, (string)null);
        }

        public async Task<dynamic> KeepAliveUserStream(string listenKey)
        {
            if (string.IsNullOrWhiteSpace(listenKey))
            {
                throw new ArgumentException("listenKey cannot be empty. ", "listenKey");
            }
            return await base._apiClient.CallAsync<object>((ApiMethod)2, EndPoints.KeepAliveUserStream, false, $"listenKey={listenKey}");
        }

        public async Task<dynamic> CloseUserStream(string listenKey)
        {
            if (string.IsNullOrWhiteSpace(listenKey))
            {
                throw new ArgumentException("listenKey cannot be empty. ", "listenKey");
            }
            return await base._apiClient.CallAsync<object>((ApiMethod)3, EndPoints.CloseUserStream, false, $"listenKey={listenKey}");
        }

        public void ListenDepthEndpoint(string symbol, MessageHandler<DepthMessage> depthHandler)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            string text = symbol + "@depth";
            base._apiClient.ConnectToWebSocket<DepthMessage>(text, depthHandler, true);
        }

        public void ListenKlineEndpoint(string symbol, TimeInterval interval, MessageHandler<KlineMessage> klineHandler)
        {
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            string text = symbol + $"@kline_{((Enum)(object)interval).GetDescription()}";
            base._apiClient.ConnectToWebSocket<KlineMessage>(text, klineHandler, false);
        }

        public void ListenTradeEndpoint(string symbol, MessageHandler<AggregateTradeMessage> tradeHandler)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }
            string text = symbol + "@aggTrade";
            base._apiClient.ConnectToWebSocket<AggregateTradeMessage>(text, tradeHandler, false);
        }

        public string ListenUserDataEndpoint(
     ApiClientAbstract.MessageHandler<AccountUpdatedMessage> accountInfoHandler,
     ApiClientAbstract.MessageHandler<OrderOrTradeUpdatedMessage> tradesHandler,
     ApiClientAbstract.MessageHandler<OrderOrTradeUpdatedMessage> ordersHandler)
        {
            string listenKey = this.StartUserStream().Result.ListenKey;
            this._apiClient.ConnectToUserDataWebSocket(listenKey, accountInfoHandler, tradesHandler, ordersHandler);
            return listenKey;
        }
    }
}
