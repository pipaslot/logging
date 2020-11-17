using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Pipaslot.Logging.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DemoAppBenchmark>(/*new DebugInProcessConfig()*/);
            Console.WriteLine("Benchmark finished");
            Console.ReadLine();
        }
    }
}
