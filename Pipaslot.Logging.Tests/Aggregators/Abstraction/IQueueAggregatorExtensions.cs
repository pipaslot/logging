using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Aggregators;
using Pipaslot.Logging.States;

namespace Pipaslot.Logging.Tests.Aggregators.Abstraction
{
    internal static class IQueueAggregatorExtensions
    {
        public const string TraceId = "trace";
        public const string Category = "Category";

        public static void WriteIncreaseScope(this IQueueAggregator aggregator)
        {
            aggregator.WriteScopeChange(TraceId, Category, new IncreaseScopeState("method"));
        }

        public static void WriteIncreaseScope(this IQueueAggregator aggregator, string traceId)
        {
            aggregator.WriteScopeChange(traceId, Category, new IncreaseScopeState("method"));
        }

        public static void WriteIncreaseScope(this IQueueAggregator aggregator, string traceId, string category)
        {
            aggregator.WriteScopeChange(traceId, category, new IncreaseScopeState("method"));
        }

        public static void WriteIncreaseMethod(this IQueueAggregator aggregator)
        {
            aggregator.WriteScopeChange(TraceId, Category, new IncreaseMethodState("method"));
        }

        public static void WriteIncreaseMethod(this IQueueAggregator aggregator, string traceId)
        {
            aggregator.WriteScopeChange(traceId, Category, new IncreaseMethodState("method"));
        }

        public static void WriteIncreaseMethod(this IQueueAggregator aggregator, string traceId, string category)
        {
            aggregator.WriteScopeChange(traceId, category, new IncreaseMethodState("method"));
        }

        public static void WriteDecreaseScope(this IQueueAggregator aggregator)
        {
            aggregator.WriteScopeChange(TraceId, Category, new DecreaseScopeState());
        }

        public static void WriteDecreaseScope(this IQueueAggregator aggregator, string traceId)
        {
            aggregator.WriteScopeChange(traceId, Category, new DecreaseScopeState());
        }

        public static void WriteDecreaseScope(this IQueueAggregator aggregator, string traceId, string category)
        {
            aggregator.WriteScopeChange(traceId, category, new DecreaseScopeState());
        }

        public static void WriteLog(this IQueueAggregator aggregator, LogLevel level)
        {
            aggregator.WriteLog(TraceId, Category, level, "message", new { });
        }

        public static void WriteLog(this IQueueAggregator aggregator, LogLevel level, string traceId)
        {
            aggregator.WriteLog(traceId, Category, level, "message", new { });
        }

        public static void WriteLog(this IQueueAggregator aggregator, LogLevel level, string traceId, string category)
        {
            aggregator.WriteLog(traceId, category, level, "message", new { });
        }
    }
}