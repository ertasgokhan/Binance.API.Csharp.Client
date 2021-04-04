using System;
using System.Collections.Generic;
using System.Linq;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.WebSocket;
using Newtonsoft.Json.Linq;

namespace Binance.API.Csharp.Client.Utils
{
	public class CustomParser
	{
		public OrderBook GetParsedOrderBook(dynamic orderBookData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Expected O, but got Unknown
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Expected O, but got Unknown
			//OrderBook val = new OrderBook();
		//	val.set_LastUpdateId((long)orderBookData.lastUpdateId.Value);
			OrderBook val2 = new OrderBook();
			//List<OrderBookOffer> list = new List<OrderBookOffer>();
			//List<OrderBookOffer> list2 = new List<OrderBookOffer>();
			//JToken[] array = ((IEnumerable<JToken>)(JArray)orderBookData.bids).ToArray();
			//foreach (JToken val3 in array)
			//{
			//	OrderBookOffer val4 = new OrderBookOffer();
			//	val4.set_Price(decimal.Parse(((object)val3.get_Item((object)0)).ToString()));
			//	val4.set_Quantity(decimal.Parse(((object)val3.get_Item((object)1)).ToString()));
			//	list.Add(val4);
			//}
			//JToken[] array2 = ((IEnumerable<JToken>)(JArray)orderBookData.asks).ToArray();
			//foreach (JToken val5 in array2)
			//{
			//	OrderBookOffer val6 = new OrderBookOffer();
			//	val6.set_Price(decimal.Parse(((object)val5.get_Item((object)0)).ToString()));
			//	val6.set_Quantity(decimal.Parse(((object)val5.get_Item((object)1)).ToString()));
			//	list2.Add(val6);
			//}
			//val2.set_Bids((IEnumerable<OrderBookOffer>)list);
			//val2.set_Asks((IEnumerable<OrderBookOffer>)list2);
			return val2;
		}

		public IEnumerable<Candlestick> GetParsedCandlestick(dynamic candlestickData)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Expected O, but got Unknown
			List<Candlestick> list = new List<Candlestick>();
            JToken[] array = ((IEnumerable<JToken>)(JArray)candlestickData).ToArray();
            foreach (JToken val in array)
            {
				Candlestick val2 = new Candlestick()
				{
					OpenTime = long.Parse(val[0].ToString()),
					Open = (decimal)(Decimal.Parse(val[1].ToString()) / 100000000),
					High = (decimal)(Decimal.Parse(val[2].ToString()) / 100000000),
					Low = (decimal)(Decimal.Parse(val[3].ToString()) / 100000000),
					Close = (decimal)(Decimal.Parse(val[4].ToString()) / 100000000),
					Volume = Decimal.Parse(val[5].ToString()),
					CloseTime = long.Parse(val[6].ToString()),
					QuoteAssetVolume = Decimal.Parse(val[7].ToString()),
					NumberOfTrades = int.Parse(val[8].ToString()),
					TakerBuyBaseAssetVolume = Decimal.Parse(val[9].ToString()),
					TakerBuyQuoteAssetVolume = Decimal.Parse(val[10].ToString())
				};
				double timestamp = Double.Parse(val2.OpenTime.ToString());
				System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
				val2.OpenDateTime = dateTime.AddMilliseconds(timestamp);
				timestamp = Double.Parse(val2.CloseTime.ToString());
				dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
				val2.CloseDateTime = dateTime.AddMilliseconds(timestamp);

				//val2.set_OpenTime(long.Parse(((object)val.get_Item((object)0)).ToString()));
				//val2.set_Open(decimal.Parse(((object)val.get_Item((object)1)).ToString()));
				//val2.set_High(decimal.Parse(((object)val.get_Item((object)2)).ToString()));
				//val2.set_Low(decimal.Parse(((object)val.get_Item((object)3)).ToString()));
				//val2.set_Close(decimal.Parse(((object)val.get_Item((object)4)).ToString()));
				//val2.set_Volume(decimal.Parse(((object)val.get_Item((object)5)).ToString()));
				//val2.set_CloseTime(long.Parse(((object)val.get_Item((object)6)).ToString()));
				//val2.set_QuoteAssetVolume(decimal.Parse(((object)val.get_Item((object)7)).ToString()));
				//val2.set_NumberOfTrades(int.Parse(((object)val.get_Item((object)8)).ToString()));
				//val2.set_TakerBuyBaseAssetVolume(decimal.Parse(((object)val.get_Item((object)9)).ToString()));
				//val2.set_TakerBuyQuoteAssetVolume(decimal.Parse(((object)val.get_Item((object)10)).ToString()));
				list.Add(val2);
            }
            return list;
		}

		public DepthMessage GetParsedDepthMessage(dynamic messageData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Expected O, but got Unknown
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Expected O, but got Unknown
			DepthMessage val = new DepthMessage();
			//val.set_EventType((string)messageData.e);
			//val.set_EventTime((long)messageData.E);
			//val.set_Symbol((string)messageData.s);
			//val.set_UpdateId((int)messageData.u);
			DepthMessage val2 = val;
			//List<OrderBookOffer> list = new List<OrderBookOffer>();
			//List<OrderBookOffer> list2 = new List<OrderBookOffer>();
			//JToken[] array = ((IEnumerable<JToken>)(JArray)messageData.b).ToArray();
			//foreach (JToken val3 in array)
			//{
			//	OrderBookOffer val4 = new OrderBookOffer();
			//	val4.set_Price(decimal.Parse(((object)val3.get_Item((object)0)).ToString()));
			//	val4.set_Quantity(decimal.Parse(((object)val3.get_Item((object)1)).ToString()));
			//	list.Add(val4);
			//}
			//JToken[] array2 = ((IEnumerable<JToken>)(JArray)messageData.a).ToArray();
			//foreach (JToken val5 in array2)
			//{
			//	OrderBookOffer val6 = new OrderBookOffer();
			//	val6.set_Price(decimal.Parse(((object)val5.get_Item((object)0)).ToString()));
			//	val6.set_Quantity(decimal.Parse(((object)val5.get_Item((object)1)).ToString()));
			//	list2.Add(val6);
			//}
			//val2.set_Bids((IEnumerable<OrderBookOffer>)list);
			//val2.set_Asks((IEnumerable<OrderBookOffer>)list2);
			return val2;
		}
	}

}