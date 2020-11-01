using System;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Action call when hitting dispose block
    /// </summary>
    public class DisposeCallback : IDisposable
    {
        private readonly Action _onDispose;

        public DisposeCallback(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose();
        }
    }
}