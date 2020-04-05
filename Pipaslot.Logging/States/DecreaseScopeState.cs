using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Logging.States
{
    public class DecreaseScopeState : IState
    {
        public bool HasData => false;

        public string FormatMessage(string categoryName, string message)
        {
            return "";
        }
    }
}
