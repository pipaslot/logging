namespace Pipaslot.Logging.States
{
    internal interface IState
    {
        string FormatMessage(string categoryName, string message);
    }
}