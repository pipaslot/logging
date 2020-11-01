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
            services.AddSingleton<IConfiguration>(s => configuration);
            
            var lb = new LoggingBuilder(services);
            lb.AddRequestLogger();

            return services.BuildServiceProvider();
        }
        
        #endregion

        [Benchmark]
        public void Without_Logging()
        {
            _controller.PerformActionWithoutLogging();
        }
        
        /// <summary>
        /// Keep in mind that this measurement will be distorted by Writing into file or to another output (IO operations)
        /// </summary>
        // Original: 21,000ns
        [Benchmark]
        public void With_Logging()
        {
            _controller.PerformActionWithSingleLog();
        }

        /// <summary>
        /// This measurement will reduce impact of IO operations to duration
        /// </summary>
        [Benchmark]
        public void With_Logging_1000x()
        {
            _controller.PerformActionWithLogging(1000);
        }
    }
}