using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Writers;

namespace Pipaslot.Logging
{
    public class LoggerProvider : ILoggerProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnumerable<IWriter> _writers;
        
        private readonly ConcurrentDictionary<string, Logger> _sessions = new ConcurrentDictionary<string, Logger>();

        public LoggerProvider(IHttpContextAccessor httpContextAccessor, IEnumerable<IWriter> writers)
        {
            Debug.Assert(httpContextAccessor != null, nameof(httpContextAccessor) + " != null");
            _httpContextAccessor = httpContextAccessor;
            _writers = writers;
        }
        public void Dispose()
        {
            //Do nothing
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

            var session = new Logger(_writers, _httpContextAccessor, categoryName);
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
