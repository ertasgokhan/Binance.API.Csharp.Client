using Binance.API.Csharp.Client.Domain.Interfaces;

namespace Binance.API.Csharp.Client.Domain.Abstract
{
    public abstract class BinanceClientAbstract
    {
        public Binance.API.Csharp.Client.Models.Market.TradingRules.TradingRules _tradingRules;
        public readonly IApiClient _apiClient;

        public BinanceClientAbstract(IApiClient apiClient) => this._apiClient = apiClient;
    }
}
