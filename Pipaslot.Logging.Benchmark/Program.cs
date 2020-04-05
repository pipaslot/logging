using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.WebApp;
using Pipaslot.Logging.WebApp.Controllers;

namespace Pipaslot.Logging.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ApiBenchmark>();
            Console.WriteLine("Benchmark finished");
            Console.ReadLine();
        }
    }

    public class ApiBenchmark
    {
        private ValuesController _controller;

        #region Setup

        [GlobalSetup]
        public void GlobalSetup()
        {
            var services = BuildServiceProvider();

            // Configure logger
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var loggerProvider = services.GetRequiredService<LoggerProvider>();
            loggerFactory.AddProvider(loggerProvider);

            _controller = services.GetRequiredService<ValuesController>();
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();
            var configuration = configurationBuilder.Build();
            var startup = new Startup(configuration);
            startup.ConfigureServices(services);
            services.AddLogging();
            services.AddTransient<ValuesController>();
            return services.BuildServiceProvider();
        }
        
        #endregion

        // Original: 21,000ns
        [Benchmark]
        public void With_Logging()
        {
            _controller.PerformActionWithLogging();
        }

        [Benchmark]
        public void Without_Logging()
        {
            _controller.PerformActionWithoutLogging();
        }
    }
}
