using System;
using System.IO;
using Microsoft.Extensions.Options;

namespace Pipaslot.Logging
{
    public class FileEraser
    {
        private readonly IFileNameFormatter _nameFormatter;
        private readonly IOptions<PipaslotLoggerOptions> _options;

        public FileEraser(IOptions<PipaslotLoggerOptions> options, IFileNameFormatter nameFormatter)
        {
            _options = options;
            _nameFormatter = nameFormatter;
        }

        /// <summary>
        ///     Erase files by date
        /// </summary>
        /// <param name="minAge">Minimal file age to be removed</param>
        /// <returns></returns>
        public int Run(TimeSpan minAge)
        {
            var directory = _options.Value.OutputPath;
            if (!Directory.Exists(directory)) return 0;

            var erased = 0;
            var minDate = DateTime.Now - minAge;
            foreach (var filePath in Directory.GetFiles(directory)){
                var date = _nameFormatter.ParseDate(Path.GetFileName(filePath));
                if (date.HasValue && date.Value < minDate){
                    try{
                        File.Delete(filePath);
                        erased++;
                    }
                    catch (UnauthorizedAccessException){
                        // Ignore
                    }
                    catch (IOException){
                        // Ignore
                    }
                }
            }

            return erased;
        }
    }
}