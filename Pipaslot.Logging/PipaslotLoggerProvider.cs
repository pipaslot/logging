using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    public class PipaslotLoggerProvider : ILoggerProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnumerable<IQueue> _queues;
        
        private readonly ConcurrentDictionary<string, PipaslotLogger> _sessions = new ConcurrentDictionary<string, PipaslotLogger>();

        public PipaslotLoggerProvider(IHttpContextAccessor httpContextAccessor, IEnumerable<IQueue> queues)
        {
            Debug.Assert(httpContextAccessor != null, nameof(httpContextAccessor) + " != null");
            _httpContextAccessor = httpContextAccessor;
            _queues = queues;
        }
        
        public void Dispose()
        {
            foreach (var logger in _sessions.Values)
            {
                logger.Dispose();
            }
        }
        
        /// <summary>
        /// Create logger for every scope. All these scopes has shared writers
        /// </summary>
        public ILogger CreateLogger(string categoryName)
        {
            // Reduce logger allocations
            if (_sessions.TryGetValue(categoryName, out var logger))
            {
                return logger;
            }

            var session = new PipaslotLogger(_queues, _httpContextAccessor, categoryName);
            if (_sessions.TryAdd(categoryName, session))
            {
                return session;
            }
            else
            {
                return _sessions[categoryName];
            }
        }
    }
}
