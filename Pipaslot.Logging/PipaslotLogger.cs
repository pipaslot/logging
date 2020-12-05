using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging
{
    public class PipaslotLogger : ILogger
    {
        private readonly QueueAggregator _aggregator;
        private readonly string _categoryName;
        private readonly IHttpContextAccessor _httpContextAccessor;

        internal PipaslotLogger(IHttpContextAccessor httpContextAccessor, QueueAggregator aggregator, string categoryName)
        {
            _httpContextAccessor = httpContextAccessor;
            _categoryName = categoryName;
            _aggregator = aggregator;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string>? formatter)
        {
            var message = eventId.Name;
            if (exception != null && formatter != null)
                message = formatter(state, exception) + "\n" + exception;
            else if (exception != null) message = exception.ToString();

            if (string.IsNullOrWhiteSpace(message)){
                message = state?.ToString() ?? "";
                Write<object?>(logLevel, message, null);
            }
            else
                Write(logLevel, message, state);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return state is IState state1 
                ? WriteScopeChange(state1) 
                : WriteScopeChange(new IncreaseScopeState("", state));
        }

        private void Write<TState>(LogLevel severity, string message, TState state)
        {
            var context = _httpContextAccessor.HttpContext;
            var identifier = context?.TraceIdentifier ?? GetProcessIdentifier();
            _aggregator.WriteLog(identifier, _categoryName, severity, message, state);
        }

        private IDisposable WriteScopeChange(IState state)
        {
            var context = _httpContextAccessor.HttpContext;
            var identifier = context?.TraceIdentifier ?? GetProcessIdentifier();
            _aggregator.WriteScopeChange(identifier, _categoryName, state);
            return new DisposeCallback(() => { WriteScopeChange(new DecreaseScopeState(state)); });
        }

        private string GetProcessIdentifier()
        {
            var process = Process.GetCurrentProcess();

            return $"{Constants.CliTraceIdentifierPrefix}ID:{process.Id}_Started:{process.StartTime:HH}:{process.StartTime:mm}:{process.StartTime:ss}";
        }
    }
}