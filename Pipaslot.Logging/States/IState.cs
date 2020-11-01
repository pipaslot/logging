namespace Pipaslot.Logging.States
{
    internal interface IState
    {
        bool HasData { get; }

        //TODO Get rid of formatting
        string FormatMessage(string categoryName, string message);
    }
}