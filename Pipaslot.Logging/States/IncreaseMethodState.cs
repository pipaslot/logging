namespace Pipaslot.Logging.States
{
    public class IncreaseMethodState : IncreaseScopeState
    {
        public IncreaseMethodState(string callerMemberName, object? data = null) : base(callerMemberName, data)
        {
        }
    }
}