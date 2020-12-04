using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.Configuration;

namespace Pipaslot.Logging
{
    [ProviderAlias("Pipaslot")]
    public class PipaslotLoggerProvider : ILoggerProvider
    {
        private readonly QueueAggregator _aggregator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConcurrentDictionary<string, PipaslotLogger> _sessions = new ConcurrentDictionary<string, PipaslotLogger>();

        public PipaslotLoggerProvider(IHttpContextAccessor httpContextAccessor, IEnumerable<Pipe> pipes, IOptions<PipaslotLoggerOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _aggregator = new QueueAggregator(pipes, options);
        }

        public void Dispose()
        {
            _aggregator.Dispose();
        }

        /// <summary>
        ///     Create logger for every scope. All these scopes has shared writers
        /// </summary>
        public ILogger CreateLogger(string categoryName)
        {
            return _sessions.GetOrAdd(categoryName, name => new PipaslotLogger(_httpContextAccessor, _aggregator, categoryName));
        }
    }
}