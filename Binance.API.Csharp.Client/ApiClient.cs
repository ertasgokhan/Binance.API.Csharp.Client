using Binance.API.Csharp.Client.Domain.Abstract;
using Binance.API.Csharp.Client.Domain.Interfaces;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.WebSocket;
using Binance.API.Csharp.Client.Utils;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
//using WebSocketSharp;

namespace Binance.API.Csharp.Client
{
	public class ApiClient : ApiClientAbstract, IApiClient
	{
		public ApiClient(
	  string apiKey,
	  string apiSecret,
	  string apiUrl = "https://www.binance.com",
	  string webSocketEndpoint = "wss://stream.binance.com:9443/ws/",
	  bool addDefaultHeaders = true)
	  : base(apiKey, apiSecret, apiUrl, webSocketEndpoint, addDefaultHeaders)
		{
		}

		public async Task<T> CallAsync<T>(
	  ApiMethod method,
	  string endpoint,
	  bool isSigned = false,
	  string parameters = null)
		{
			string finalEndpoint = endpoint + (string.IsNullOrWhiteSpace(parameters) ? "" : string.Format("?{0}", (object)parameters));
			if (isSigned)
			{
				parameters = parameters + (!string.IsNullOrWhiteSpace(parameters) ? "&timestamp=" : "timestamp=") + Utilities.GenerateTimeStamp(DateTime.Now.ToUniversalTime());
				string signature = Utilities.GenerateSignature(this._apiSecret, parameters);
				finalEndpoint = string.Format("{0}?{1}&signature={2}", (object)endpoint, (object)parameters, (object)signature);
				signature = (string)null;
			}
			HttpRequestMessage request = new HttpRequestMessage(Utilities.CreateHttpMethod(method.ToString()), finalEndpoint);
			HttpResponseMessage response = await this._httpClient.SendAsync(request).ConfigureAwait(false);
			if (response.IsSuccessStatusCode)
			{
				response.EnsureSuccessStatusCode();
				string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonConvert.DeserializeObject<T>(result);
			}
			if (response.StatusCode == HttpStatusCode.GatewayTimeout)
				throw new Exception("Api Request Timeout.");
			string e = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			int eCode = 0;
			string eMsg = "";
			if (e.IsValidJson())
			{
				try
				{
					JObject i = JObject.Parse(e);
					JToken jtoken1 = i["code"];
					eCode = jtoken1 != null ? jtoken1.Value<int>() : 0;
					JToken jtoken2 = i["msg"];
					eMsg = jtoken2 != null ? jtoken2.Value<string>() : (string)null;
					i = (JObject)null;
				}
				catch
				{
				}
			}
			throw new Exception(string.Format("Api Error Code: {0} Message: {1}", (object)eCode, (object)eMsg));
		}

		public void ConnectToWebSocket<T>(string parameters, MessageHandler<T> messageHandler, bool useCustomParser = false)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//string text = base._webSocketEndpoint + parameters;
			//WebSocket ws = new WebSocket(text, Array.Empty<string>());
			//ws.add_OnMessage((EventHandler<MessageEventArgs>)delegate (object sender, MessageEventArgs e)
			//{
			//	object obj;
			//	if (useCustomParser)
			//	{
			//		CustomParser customParser = new CustomParser();
			//		obj = customParser.GetParsedDepthMessage((dynamic)JsonConvert.DeserializeObject<object>(e.get_Data()));
			//	}
			//	else
			//	{
			//		obj = JsonConvert.DeserializeObject<T>(e.get_Data());
			//	}
			//	messageHandler((dynamic)obj);
			//});
			//ws.add_OnClose((EventHandler<CloseEventArgs>)delegate
			//{
			//	base._openSockets.Remove(ws);
			//});
			//ws.add_OnError((EventHandler<ErrorEventArgs>)delegate
			//{
			//	base._openSockets.Remove(ws);
			//});
			//ws.Connect();
			//base._openSockets.Add(ws);
		}

		public void ConnectToUserDataWebSocket(string parameters, MessageHandler<AccountUpdatedMessage> accountHandler, MessageHandler<OrderOrTradeUpdatedMessage> tradeHandler, MessageHandler<OrderOrTradeUpdatedMessage> orderHandler)
		{
			////IL_0038: Unknown result type (might be due to invalid IL or missing references)
			////IL_0042: Expected O, but got Unknown
			//string text = base._webSocketEndpoint + parameters;
			//WebSocket ws = new WebSocket(text, Array.Empty<string>());
			//ws.add_OnMessage((EventHandler<MessageEventArgs>)delegate (object sender, MessageEventArgs e)
			//{
			//	object obj = JsonConvert.DeserializeObject<object>(e.get_Data());
			//	object obj2 = ((dynamic)obj).e;
			//	object obj3 = obj2;
			//	if (obj3 != null)
			//	{
			//		switch (obj3 as string)
			//		{
			//			case "outboundAccountInfo":
			//				accountHandler.Invoke(JsonConvert.DeserializeObject<AccountUpdatedMessage>(e.get_Data()));
			//				break;
			//			case "executionReport":
			//				if (((string)((dynamic)obj).x).ToLower() == "trade")
			//				{
			//					tradeHandler.Invoke(JsonConvert.DeserializeObject<OrderOrTradeUpdatedMessage>(e.get_Data()));
			//				}
			//				else
			//				{
			//					orderHandler.Invoke(JsonConvert.DeserializeObject<OrderOrTradeUpdatedMessage>(e.get_Data()));
			//				}
			//				break;
			//		}
			//	}
			//});
			//ws.add_OnClose((EventHandler<CloseEventArgs>)delegate
			//{
			//	base._openSockets.Remove(ws);
			//});
			//ws.add_OnError((EventHandler<ErrorEventArgs>)delegate
			//{
			//	base._openSockets.Remove(ws);
			//});
			//ws.Connect();
			//base._openSockets.Add(ws);
		}
	}
}
