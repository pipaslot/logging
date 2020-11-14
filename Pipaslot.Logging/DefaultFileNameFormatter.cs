using System;
using System.Globalization;
using System.Linq;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Default file name formatter providing unified file name format
    /// </summary>
    public class DefaultFileNameFormatter : IFileNameFormatter
    {
        private const char DateAndNameDelimiter = '-';

        public string Format(DateTime dateTime, string name, RollingInterval rollingInterval)
        {
            var date = FormatDate(dateTime, rollingInterval);
            var trimmedName = name.Trim('-'); // Trim for back compatibility with prefixes containing dash on start
            return $"{date}{DateAndNameDelimiter}{trimmedName}.log";
        }

        public DateTime? ParseDate(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            var parts = fileName.Split(DateAndNameDelimiter);
            if (parts.Length > 1){
                var firstPart = parts.First();
                return ParseDateFormatted(firstPart);
            }

            return null;
        }

        private string FormatDate(DateTime dateTime, RollingInterval rollingInterval)
        {
            switch (rollingInterval){
                case RollingInterval.Infinite:
                    return "";
                case RollingInterval.Year:
                    return dateTime.ToString("yyyy");
                case RollingInterval.Month:
                    return dateTime.ToString("yyyyMM");
                case RollingInterval.Day:
                    return dateTime.ToString("yyyyMMdd");
                case RollingInterval.Hour:
                    return dateTime.ToString("yyyyMMddhh");
                case RollingInterval.Minute:
                    return dateTime.ToString("yyyyMMddhhmm");
                default:
                    return dateTime.ToString("yyyyMMdd");
            }
        }

        private DateTime? ParseDateFormatted(string dateString)
        {
            switch (dateString.Length){
                case 4:
                    return ParseDateFromFormat(dateString, "yyyy");
                case 6:
                    return ParseDateFromFormat(dateString, "yyyyMM");
                case 8:
                    return ParseDateFromFormat(dateString, "yyyyMMdd");
                case 10:
                    return ParseDateFromFormat(dateString, "yyyyMMddhh");
                case 12:
                    return ParseDateFromFormat(dateString, "yyyyMMddhhmm");
                default:
                    return null;
            }
        }

        private DateTime? ParseDateFromFormat(string dateString, string format)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) return date;

            return null;
        }
    }
}