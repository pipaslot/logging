using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Logging.States
{
    interface IState
    {
        //TODO Get rid of formatting
        string FormatMessage(string categoryName, string message);

        bool HasData { get; }
    }
}
