using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pipaslot.Logging.Benchmark.Mocks;
using Pipaslot.Logging.Demo;
using Pipaslot.Logging.Demo.Controllers;

namespace Pipaslot.Logging.Benchmark
{
    public class DemoAppBenchmark
    {
        private ValuesController _controllerWithStandardLogging;
        private ValuesController _controllerWithRequestLogging;

        #region Setup

        [GlobalSetup]
        public void GlobalSetup()
        {
            var servicesWithStandardLogging = BuildServiceProvider(false);
            var servicesWithRequestLogging = BuildServiceProvider(true);

            _controllerWithStandardLogging = servicesWithStandardLogging.GetRequiredService<ValuesController>();
            _controllerWithRequestLogging = servicesWithRequestLogging.GetRequiredService<ValuesController>();
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
            }
            else
            {
                lb.AddProvider(new NullLoggerProvider());
            }

            return services.BuildServiceProvider();
        }

        #endregion

        [Benchmark]
        public void Standard_OnlyMessage_1000x()
        {
            _controllerWithStandardLogging.PerformActionWithLogging(1000);
        }

        [Benchmark]
        public void Request_OnlyMessage_1000x()
        {
            _controllerWithRequestLogging.PerformActionWithLogging(1000);
        }

        [Benchmark]
        public void Standard_ScopesAndMultipleMessages()
        {
            _controllerWithStandardLogging.PerformComplexAction();
        }

        [Benchmark]
        public void Request_ScopesAndMultipleMessages()
        {
            _controllerWithRequestLogging.PerformComplexAction();
        }

        private class NullLoggerProvider : ILoggerProvider
        {
            public void Dispose()
            {

            }

            public ILogger CreateLogger(string categoryName)
            {
                return NullLogger.Instance;
            }
        }
    }
}