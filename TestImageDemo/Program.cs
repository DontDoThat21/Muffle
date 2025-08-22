using System;
using System.Threading.Tasks;
using Muffle.Tests;

namespace TestImageDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ImageMessageDemo.RunDemo();
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}