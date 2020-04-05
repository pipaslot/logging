using System;

namespace Pipaslot.Logging
{
    /// <summary>
    /// Operation/Task tracker 
    /// </summary>
    public class Operation : IDisposable
    {
        private readonly Action _onDispose;

        public Operation(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose();
        }
    }
}
