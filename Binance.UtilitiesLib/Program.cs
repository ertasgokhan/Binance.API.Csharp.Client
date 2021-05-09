using System;
using System.Threading.Tasks;

namespace Binance.UtilitiesLib
{
    class Program
    {
        public static void Main(string[] args)
        {
            string a = StringCipher.Encrypt("1731193644:AAFphIGqOB_gYt-PIKi0bjpxSyqTD8YkgiY");

            Console.WriteLine(a);

            string b = StringCipher.Encrypt("1578356310");

            Console.WriteLine(b);

            Console.ReadLine();
        }
    }
}
