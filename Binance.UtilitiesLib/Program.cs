using System;
using System.Threading.Tasks;

namespace Binance.UtilitiesLib
{
    class Program
    {
        public static void Main(string[] args)
        {
            string a = StringCipher.Encrypt("1856792552:AAGhNnyFPLlQ39-vpl6NDYuCGV7kwuthh84");

            Console.WriteLine(a);

            string b = StringCipher.Encrypt("-526163480");

            Console.WriteLine(b);

            Console.ReadLine();
        }
    }
}
