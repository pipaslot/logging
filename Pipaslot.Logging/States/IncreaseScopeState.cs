using System.Text;
using Newtonsoft.Json;

namespace Pipaslot.Logging.States
{
    public class IncreaseScopeState : IState
    {
        private readonly object _data;

        public IncreaseScopeState(string callerMemberName, object data = null)
        {
            CallerMemberName = callerMemberName;
            _data = data;
        }

        public string CallerMemberName { get; }

        public string FormatMessage(string categoryName, string message)
        {
            var sb = new StringBuilder();
            sb.Append(categoryName);
            sb.Append(" > ");
            if (!string.IsNullOrWhiteSpace(CallerMemberName)){
                sb.Append(CallerMemberName);
                sb.Append(" > ");
            }

            if (_data != null){
                if (_data is string)
                    sb.Append(_data);
                else
                    sb.Append(JsonConvert.SerializeObject(_data));
            }
            else
                sb.Append(message);

            return sb.ToString();
        }
    }
}