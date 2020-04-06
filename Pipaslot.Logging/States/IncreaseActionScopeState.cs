//using System.Text;
//using Newtonsoft.Json;
//
//namespace Pipaslot.Logging.States
//{
//    //TODO implements
//    public class IncreaseActionScopeState : IState
//    {
//        public string UniqueIdentifier { get; }
//        private readonly object _data;
//        public bool HasData => _data != null;
//
//        public IncreaseActionScopeState(string uniqueIdentifier, object data = null)
//        {
//            UniqueIdentifier = uniqueIdentifier;
//            _data = data;
//        }
//
//        public string FormatMessage(string categoryName, string message)
//        {
//            var sb = new StringBuilder();
//            sb.Append(categoryName);
//            sb.Append(" > ");
//            if (!string.IsNullOrWhiteSpace(UniqueIdentifier))
//            {
//                sb.Append(UniqueIdentifier);
//                sb.Append(" > ");
//            }
//
//            if (_data != null)
//            {
//                if (_data is string)
//                {
//                    sb.Append(_data);
//                }
//                else
//                {
//                    sb.Append(JsonConvert.SerializeObject(_data));
//                }
//            }
//            else
//            {
//                sb.Append(message);
//            }
//            return sb.ToString();
//        }
//    }
//}