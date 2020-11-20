using System;

namespace Pipaslot.Logging
{
    /// <summary>
    ///     Provide unified file name format for log files
    /// </summary>
    public interface IFileNameFormatter
    {
        /// <summary>
        ///     Format file name
        /// </summary>
        /// <param name="dateTime">Log time</param>
        /// <param name="name">Queue name</param>
        /// <param name="rollingInterval">Rolling interval</param>
        /// <returns></returns>
        string Format(DateTime dateTime, string name, RollingInterval rollingInterval);

        /// <summary>
        ///     Get name from file name
        /// </summary>
        /// <param name="fileName"></param>
        DateTime? ParseDate(string fileName);
    }
}