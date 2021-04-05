using Binance.Generate.OTT;
using System;

namespace ConsoleAppTest
{
    class Program
    {
        public const string sourceDirectory = @"C:\BinanceBot\";
        public const string apiKey = "srhEOc1oqMt4euGiUeVBseXk588iBD4mFUD0k3VcFQQiQdRlA1NvVxVY2x0weXej";
        public const string apiSecret = "obd4UryGMEKgdvb9B84bKGrXxusQUEQ8nYFUba85xst02dq7FNRvdFMNZtze9RDj";
        public const int limit = 1000;

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            GenerateOTTLine.GenerateOTT();

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime.Subtract(startTime);
            Console.WriteLine("Time Difference {0}:{1}:{2} ", span.Hours, span.Minutes, span.Seconds);
            Console.ReadLine();
        }
    }
}
