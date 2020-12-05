namespace Pipaslot.Logging.States
{
    internal class DecreaseScopeState : IState
    {
        public DecreaseScopeState(IState increaseScopeState)
        {
            IncreaseScopeState = increaseScopeState;
        }
        public IState IncreaseScopeState { get; }

        public string FormatMessage(string categoryName, string message)
        {
            return "";
        }
    }
}