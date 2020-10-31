using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Demo;
using Pipaslot.Logging.Demo.Controllers;

namespace Pipaslot.Logging.Benchmark
{
    public class DemoAppBenchmark
    {
        private ValuesController _controller;

        #region Setup

        [GlobalSetup]
        public void GlobalSetup()
        {
            var services = BuildServiceProvider();

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
            
            var lb = new LoggingBuilder(services);
            lb.AddRequestLogger(Directory.GetCurrentDirectory());

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