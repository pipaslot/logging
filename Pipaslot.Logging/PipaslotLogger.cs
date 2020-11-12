using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging
{
    public class PipaslotLogger : ILogger, IDisposable
    {
        private readonly string _categoryName;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnumerable<IQueueAggregator> _queues;

        public PipaslotLogger(IEnumerable<IQueueAggregator> queues, IHttpContextAccessor httpContextAccessor, string categoryName)
        {
            _queues = queues;
            _httpContextAccessor = httpContextAccessor;
            _categoryName = categoryName;
        }

        public void Dispose()
        {
            foreach (var writer in _queues){
                writer.Dispose();
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
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

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


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
            foreach (var writer in _queues){
                writer.WriteLog(identifier, _categoryName, severity, message, state);
            }
        }

        private void WriteScopeChange<TState>(TState state)
        {
            var context = _httpContextAccessor.HttpContext;
            var identifier = context?.TraceIdentifier ?? GetProcessIdentifier();
            foreach (var writer in _queues){
                writer.WriteScopeChange(identifier, _categoryName, state);
            }
        }

        private string GetProcessIdentifier()
        {
            var process = Process.GetCurrentProcess();

            return $"{Constants.CliTraceIdentifierPrefix}ID:{process.Id}_Started:{process.StartTime:HH}:{process.StartTime:mm}:{process.StartTime:ss}";
        }
    }
}