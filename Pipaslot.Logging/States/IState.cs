using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Logging.States
{
    interface IState
    {
        string FormatMessage(string categoryName, string message);

        bool HasData { get; }
    }
}
