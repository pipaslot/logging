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
        private readonly string _categoryName;
        private readonly QueueAggregator _aggregator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PipaslotLogger(IHttpContextAccessor httpContextAccessor, QueueAggregator aggregator, string categoryName)
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
            if (state is IState)
                WriteScopeChange(state);
            else
                WriteScopeChange(new IncreaseScopeState("", state));

            return new DisposeCallback(() => { WriteScopeChange(new DecreaseScopeState()); });
        }

        private void Write<TState>(LogLevel severity, string message, TState state)
        {
            var context = _httpContextAccessor.HttpContext;
            var identifier = context?.TraceIdentifier ?? GetProcessIdentifier();
            _aggregator.WriteLog(identifier, _categoryName, severity, message, state);
        }

        private void WriteScopeChange<TState>(TState state)
        {
            var context = _httpContextAccessor.HttpContext;
            var identifier = context?.TraceIdentifier ?? GetProcessIdentifier();
            _aggregator.WriteScopeChange(identifier, _categoryName, state);
            
        }

        private string GetProcessIdentifier()
        {
            var process = Process.GetCurrentProcess();

            return $"{Constants.CliTraceIdentifierPrefix}ID:{process.Id}_Started:{process.StartTime:HH}:{process.StartTime:mm}:{process.StartTime:ss}";
        }
    }
}