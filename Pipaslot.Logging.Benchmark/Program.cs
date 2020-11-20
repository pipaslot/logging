using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Pipaslot.Logging.Benchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<DemoAppBenchmark>(new DebugInProcessConfig());
            Console.WriteLine("Benchmark finished");
            Console.ReadLine();
        }
    }
}