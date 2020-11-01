using System;
using System.Collections.Generic;

namespace Pipaslot.Logging.Groups
{
    public class LogGroupCollection : IDisposable
    {
        private readonly object _queueLock = new object();
        private readonly Dictionary<string, LogGroup> _queues = new Dictionary<string, LogGroup>();

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

        /// <returns>Can be null if can not create a new queue</returns>
        public LogGroup GetQueue(string traceIdentifier, bool canCreate)
        {
            //TODO benchmark single request access, multiple request access
            //TODO benchmark using ConcurrentDictionary instead
            if (_queues.TryGetValue(traceIdentifier, out var queue)) return queue;
            lock (_queueLock){
                if (_queues.TryGetValue(traceIdentifier, out var queue2)) return queue2;

                if (canCreate){
                    var request = new LogGroup();
                    _queues.Add(traceIdentifier, request);
                    return request;
                }

                return null;
            }
        }

        public Dictionary<string, LogGroup> GetAllQueues()
        {
            lock (_queueLock){
                return _queues;
            }
        }
    }
}