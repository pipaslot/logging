﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Demo.Services;

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
                    builder.AddTreeLogger("-controllers", "Pipaslot.Logging.Demo.Controllers");
                    builder.AddSendLogger<LogSender>(LogLevel.Error);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}