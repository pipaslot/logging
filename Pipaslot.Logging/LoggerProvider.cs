using System;
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
        private readonly IWriter _requestWriter;
        private readonly LogLevel _enabledLogLevel;

        private readonly Dictionary<string, Logger> _sessions = new Dictionary<string, Logger>();
        private readonly object _lock = new object();

        public LoggerProvider(IHttpContextAccessor httpContextAccessor, IWriter requestWriter, LogLevel enabledLogLevel)
        {
            Debug.Assert(httpContextAccessor != null, nameof(httpContextAccessor) + " != null");
            _httpContextAccessor = httpContextAccessor;
            _requestWriter = requestWriter;
            _enabledLogLevel = enabledLogLevel;
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
            lock (_lock)
            {
                if (_sessions.ContainsKey(categoryName))
                {
                    return _sessions[categoryName];
                }

                var session = new Logger(_requestWriter, _httpContextAccessor, _enabledLogLevel, categoryName);
                _sessions.Add(categoryName, session);
                return session;
            }
        }
    }
}
