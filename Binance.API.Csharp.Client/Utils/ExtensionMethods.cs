using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Binance.API.Csharp.Client.Utils
{
    public static class ExtensionMethods
    {
        public static string GetDescription(this Enum value) => ((DescriptionAttribute)Attribute.GetCustomAttribute((MemberInfo)((IEnumerable<FieldInfo>)value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public)).Single<FieldInfo>((Func<FieldInfo, bool>)(x => x.GetValue((object)null).Equals((object)value))), typeof(DescriptionAttribute)))?.Description ?? value.ToString();

        public static string GetUnixTimeStamp(this DateTime baseDateTime) => new DateTimeOffset(baseDateTime).ToUnixTimeMilliseconds().ToString();

        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                return false;
            string json = stringValue.Trim();
            if ((!json.StartsWith("{") || !json.EndsWith("}")) && (!json.StartsWith("[") || !json.EndsWith("]")))
                return false;
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException ex)
            {
                return false;
            }
        }
    }
}
