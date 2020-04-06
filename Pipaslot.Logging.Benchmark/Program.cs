using System;
using BenchmarkDotNet.Running;

namespace Pipaslot.Logging.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DemoAppBenchmark>();
            Console.WriteLine("Benchmark finished");
            Console.ReadLine();
        }
    }
}
