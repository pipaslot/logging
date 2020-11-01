using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Demo.Controllers;

namespace Pipaslot.Logging.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder =>
                {
                    builder.AddRequestLogger();
                    builder.AddFlatLogger("-errors", LogLevel.Error);
                    builder.AddProcessLogger();
                    builder.AddCallLogger( "-controllers", LogLevel.Information, nameof(ValuesController));
                    //TODO SendLogger
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}