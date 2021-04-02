using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Binance.API.Csharp.Client.Utils
{
    public static class Utilities
    {
        public static string GenerateSignature(string apiSecret, string message)
        {
            string str;
            using (HMACSHA256 hmacshA256 = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
                str = BitConverter.ToString(hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", "");
            return str;
        }

        public static string GenerateTimeStamp(DateTime baseDateTime) => new DateTimeOffset(baseDateTime).ToUnixTimeMilliseconds().ToString();

        public static HttpMethod CreateHttpMethod(string method)
        {
            string upper = method.ToUpper();
            if (upper == "DELETE")
                return HttpMethod.Delete;
            if (upper == "POST")
                return HttpMethod.Post;
            if (upper == "PUT")
                return HttpMethod.Put;
            if (upper == "GET")
                return HttpMethod.Get;
            throw new NotImplementedException();
        }
    }
}
