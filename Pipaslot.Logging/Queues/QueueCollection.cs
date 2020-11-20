using System;
using System.Collections.Generic;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    ///     Queue collection carrying about concurrent queue creating
    /// </summary>
    public class QueueCollection : IDisposable
    {
        private readonly object _queueLock = new object();
        private readonly Dictionary<string, GrowingQueue> _queues = new Dictionary<string, GrowingQueue>();

        public void Dispose()
        {
            _queues.Clear();
        }

        public void Remove(string traceIdentifier)
        {
            lock (_queueLock){
                _queues.Remove(traceIdentifier);
            }
        }

        /// <summary>
        ///     Returns existing queue or return null if does not exists
        /// </summary>
        /// <param name="traceIdentifier">Queue unique identifier</param>
        public GrowingQueue? GetQueueOrNull(string traceIdentifier)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (_queues.TryGetValue(traceIdentifier, out var queue)) return queue;
            return null;
        }

        /// <summary>
        ///     Returns existing queue or create new with prevention for concurrency creation
        /// </summary>
        /// <param name="traceIdentifier">Queue unique identifier</param>
        public GrowingQueue GetOrCreateQueue(string traceIdentifier)
        {
            // Try read without locking to improve performance
            // This approach is 2x faster in comparison to using concurrent dictionary
            // ReSharper disable once InconsistentlySynchronizedField
            if (_queues.TryGetValue(traceIdentifier, out var queue)) return queue;
            lock (_queueLock){
                if (_queues.TryGetValue(traceIdentifier, out var queue2)) return queue2;

                var request = new GrowingQueue(traceIdentifier);
                _queues.Add(traceIdentifier, request);
                return request;
            }
        }

        /// <summary>
        ///     Get all registered queues
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, GrowingQueue> GetAllQueues()
        {
            lock (_queueLock){
                return _queues;
            }
        }
    }
}