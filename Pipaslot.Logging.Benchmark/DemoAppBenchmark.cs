using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Benchmark.Mocks;
using Pipaslot.Logging.Demo;
using Pipaslot.Logging.Demo.Controllers;

namespace Pipaslot.Logging.Benchmark
{
    public class DemoAppBenchmark
    {
        private ValuesController _controllerWithoutLogging;
        private ValuesController _controllerWithRequestLogging;

        #region Setup

        [GlobalSetup]
        public void GlobalSetup()
        {
            _controllerWithoutLogging = BuildServiceProvider(false).GetRequiredService<ValuesController>();
            _controllerWithRequestLogging = BuildServiceProvider(true).GetRequiredService<ValuesController>();
        }

        private IServiceProvider BuildServiceProvider(bool configureLogging)
        {
            var services = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();
            var configuration = configurationBuilder.Build();
            var startup = new Startup(configuration);
            startup.ConfigureServices(services);
            services.AddLogging();
            services.AddTransient<ValuesController>();

            var lb = new LoggingBuilder(services);
            if (configureLogging)
            {
                services.AddSingleton<IConfiguration>(s => configuration);
                services.AddSingleton<IFileWriterFactory, NullFileWriterFactory>();
                lb.AddRequestLogger();
                lb.AddProcessLogger();
                lb.AddFlatLogger("err", LogLevel.Error);
            }

            return services.BuildServiceProvider();
        }
        #endregion
        
        [Benchmark]
        public void MultipleWrites100x_NoLogging()
        {
            _controllerWithoutLogging.PerformActionWithLogging(100);
        }

        [Benchmark]
        public void MultipleWrites100x_Requests()
        {
            _controllerWithRequestLogging.PerformActionWithLogging(100);
        }

        [Benchmark]
        public void SingleWrite100x_NoLogging()
        {
            _controllerWithoutLogging.PerformActionWithLoggingInsideScope(100);
        }

        [Benchmark]
        public void SingleWrite100x_Requests()
        {
            _controllerWithRequestLogging.PerformActionWithLoggingInsideScope(100);
        }

        [Benchmark]
        public void ScopesAndMultipleMessages_NoLogging()
        {
            _controllerWithoutLogging.PerformComplexAction();
        }

        [Benchmark]
        public void ScopesAndMultipleMessages_Requests()
        {
            _controllerWithRequestLogging.PerformComplexAction();
        }
    }
}