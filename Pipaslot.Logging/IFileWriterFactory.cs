namespace Pipaslot.Logging
{
    /// <summary>
    /// Factory class providing standard writer instances
    /// </summary>
    public interface IFileWriterFactory
    {
        /// <summary>
        /// Create new file writer instance
        /// </summary>
        /// <param name="name">Partial file name</param>
        /// <param name="rollingInterval">Specifies the frequency at which the log file should roll.</param>
        /// <returns></returns>
        ILogWriter Create(string name, RollingInterval rollingInterval);
    }
}