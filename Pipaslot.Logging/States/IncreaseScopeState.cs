using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pipaslot.Logging.States
{
    public class IncreaseScopeState : IState
    {
        public string CallerMemberName { get; }
        private readonly object _data;
        public bool HasData => _data != null;

        public IncreaseScopeState(string callerMemberName, object data = null)
        {
            CallerMemberName = callerMemberName;
            _data = data;
        }

        public string FormatMessage(string categoryName, string message)
        {
            var sb = new StringBuilder();
            sb.Append(categoryName);
            sb.Append(" > ");
            if (!string.IsNullOrWhiteSpace(CallerMemberName))
            {
                sb.Append(CallerMemberName);
                sb.Append(" > ");
            }

            if (_data != null)
            {
                if (_data is string)
                {
                    sb.Append(_data);
                }
                else
                {
                    sb.Append(JsonConvert.SerializeObject(_data));
                }
            }
            else
            {
                sb.Append(message);
            }
            return sb.ToString();
        }
    }
}
