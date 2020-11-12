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
        /// <param name="fileName"></param>
        /// <returns></returns>
        ILogWriter Create(string fileName);
    }
}