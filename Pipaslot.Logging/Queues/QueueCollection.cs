using System;
using System.Collections.Generic;

namespace Pipaslot.Logging.Queues
{
    /// <summary>
    /// Queue collection carrying about concurrent queue creating
    /// </summary>
    public class QueueCollection : IDisposable
    {
        private readonly object _queueLock = new object();
        private readonly Dictionary<string, Queue> _queues = new Dictionary<string, Queue>();

        public void Dispose()
        {
            _queues.Clear();
        }

        public void Remove(string traceIdentifier)
        {
            lock (_queueLock)
            {
                _queues.Remove(traceIdentifier);
            }
        }

        /// <returns>Can be null if can not create a new queue</returns>
        public Queue? GetQueue(string traceIdentifier, bool canCreate)
        {
            // Try read without locking to improve performance
            // This approach is 2x faster in comparison to using concurrent dictionary
            // ReSharper disable once InconsistentlySynchronizedField
            if (_queues.TryGetValue(traceIdentifier, out var queue)) return queue;
            if (canCreate)
            {
                lock (_queueLock)
                {
                    if (_queues.TryGetValue(traceIdentifier, out var queue2)) return queue2;

                    var request = new Queue(traceIdentifier);
                    _queues.Add(traceIdentifier, request);
                    return request;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all registered queues
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Queue> GetAllQueues()
        {
            lock (_queueLock)
            {
                return _queues;
            }
        }
    }
}