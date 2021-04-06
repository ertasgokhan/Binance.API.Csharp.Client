using Binance.Generate.OTT;
using System;

namespace ConsoleAppTest
{
    class Program
    {
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
