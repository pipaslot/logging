using System;
using System.Collections.Generic;

namespace Pipaslot.Logging.Writers.Queues
{
    public class LoggedQueueCollection : IDisposable
    {
        private readonly object _queueLock = new object();
        private readonly Dictionary<string, LoggedQueue> _queues = new Dictionary<string, LoggedQueue>();

        public void Remove(string traceIdentifier)
        {
            lock (_queueLock)
            {
                _queues.Remove(traceIdentifier);
            }
        }
        /// <returns>Can be null if can not create a new queue</returns>
        public LoggedQueue GetQueue(string traceIdentifier, bool canCreate)
        {
            lock (_queueLock)
            {
                if (_queues.ContainsKey(traceIdentifier))
                {
                    return _queues[traceIdentifier];
                }

                if (canCreate)
                {
                    var request = new LoggedQueue();
                    _queues.Add(traceIdentifier, request);
                    return request;
                }

                return null;
            }
        }

        public Dictionary<string, LoggedQueue> GetAllQueues()
        {
            lock (_queueLock)
            {
                return _queues;
            }
        }

        public void Dispose()
        {
            _queues.Clear();
        }
    }
}
