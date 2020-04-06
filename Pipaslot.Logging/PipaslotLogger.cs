using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Queues;

namespace Pipaslot.Logging
{
    public class PipaslotLogger : ILogger, IDisposable
    {
        private readonly IEnumerable<IQueue> _queues;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _categoryName;

        public PipaslotLogger(IEnumerable<IQueue> queues, IHttpContextAccessor httpContextAccessor, string categoryName)
        {
            _queues = queues;
            _httpContextAccessor = httpContextAccessor;
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = eventId.Name;
            if (exception != null && formatter != null)
            {
                message = formatter(state, exception) + "\n" + exception.ToString();
            }
            else if (exception != null)
            {
                message = exception.ToString();
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = state.ToString();
                state = default(TState);
            }
            Write(logLevel, message, state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            if (state is IState state1)
            {
                var level = state1.HasData ? LogLevel.Debug : LogLevel.Trace;
                Write(level, "", state);
            }
            else
            {
                var level = state != null ? LogLevel.Debug : LogLevel.Trace;
                Write(level, "", new IncreaseScopeState("", state));
            }
            return new DisposeCallback(() =>
            {
                Write(LogLevel.Trace, "", new DecreaseScopeState());
            });
        }

        private void Write<TState>(LogLevel severity, string message, TState state)
        {
            var context = _httpContextAccessor.HttpContext;
            var identifier = context?.TraceIdentifier ?? GetProcessIdentifier();
            foreach (var writer in _queues)
            {
                writer.Write(identifier, _categoryName, severity, message, state);
            }
        }

        private string GetProcessIdentifier()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();

            return $"CLI:{process.Id}:{process.StartTime:HHmmss}";
        }

        public void Dispose()
        {
            foreach (var writer in _queues)
            {
                writer.Dispose();
            }
        }
    }
}
