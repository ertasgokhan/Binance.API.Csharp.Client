using Binance.API.Csharp.Client.Domain.Abstract;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.WebSocket;
using System.Threading.Tasks;

namespace Binance.API.Csharp.Client.Domain.Interfaces
{
    public interface IApiClient
    {
        Task<T> CallAsync<T>(ApiMethod method, string endpoint, bool isSigned = false, string parameters = null);

        void ConnectToWebSocket<T>(
          string parameters,
          ApiClientAbstract.MessageHandler<T> messageDelegate,
          bool useCustomParser = false);

        void ConnectToUserDataWebSocket(
          string parameters,
          ApiClientAbstract.MessageHandler<AccountUpdatedMessage> accountHandler,
          ApiClientAbstract.MessageHandler<OrderOrTradeUpdatedMessage> tradeHandler,
          ApiClientAbstract.MessageHandler<OrderOrTradeUpdatedMessage> orderHandler);
    }
}
