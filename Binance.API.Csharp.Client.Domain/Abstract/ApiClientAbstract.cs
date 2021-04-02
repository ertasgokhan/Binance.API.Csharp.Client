using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
//using WebSocketSharp;

namespace Binance.API.Csharp.Client.Domain.Abstract
{
    public abstract class ApiClientAbstract
    {
        public readonly string _apiUrl = "";
        public readonly string _apiKey = "";
        public readonly string _apiSecret = "";
        public readonly HttpClient _httpClient;
        public readonly string _webSocketEndpoint = "";
        public List<WebSocket> _openSockets;

        public ApiClientAbstract(
          string apiKey,
          string apiSecret,
          string apiUrl = "https://www.binance.com",
          string webSocketEndpoint = "wss://stream.binance.com:9443/ws/",
          bool addDefaultHeaders = true)
        {
            this._apiUrl = apiUrl;
            this._apiKey = apiKey;
            this._apiSecret = apiSecret;
            this._webSocketEndpoint = webSocketEndpoint;
            this._openSockets = new List<WebSocket>();
            this._httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this._apiUrl)
            };
            if (!addDefaultHeaders)
                return;
            this.ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            this._httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", this._apiKey);
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public delegate void MessageHandler<T>(T messageData);
    }
}
