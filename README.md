# .NET Core Logging Provider

Logging provider enriching [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Loggin) by aggregated HTTP request logging. 
Supports multiple file writers for grouping by requests, priorities, service calls or processes. Contains adapter for notification sending.

Register logger services by `AddRequestLogger`:
```
public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateWebHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(loggingBuilder =>
            {
                // Log HTTP requests in files by actual date
                loggingBuilder.AddRequestLogger();
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
```

and provide logging options in your `appsettings.json` file:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information" // Use standard logging configuration
    },
    "Pipaslot": {
      "OutputPath": "./logs",
      "IncludeScopes": false, // Log scopes created by ILogger<>.BeginScope()
      "IncludeMethods": true,  // Log method calls created by ILogger<>.BeginMethod()
      "LogLevel": {
        "Default": "Warning", // Define default minimal log level
        "MyApp": "Information", // Log all informations from our application (includes nested namespaces like MyApp.Libs...)
        "Microsoft.AspNetCore.Hosting.Diagnostics": "Information" // Log HTTP requests
      }
    }
  }
}
```

## Example of usage
Boost your logging abilities by logging class and method names, visualize deep of nesting and log serialized data/structures
Create custom controller and service, setup logging and check your log file after few requests
```
public class ValueController
{
    private readonly MyService _service;
    public ValueController(MyService service)
    {
        _service = service;
    }

    public ActionResult GetDefaultValues(string arg1, int arg2)
    {
        // ... do something around
        return _service.MySuperServiceMethod(arg1, arg2);
    }
}

public class MyService
{
    private readonly ILogger<MyClass> _logger;
    public Service(ILogger<MyClass> logger)
    {
        _logger = logger;
    }

    public string MySuperServiceMethod(string arg1, int arg2)
    {
        // Log class full name and invoked method name with some additional data as method scope
        using(_logger.BeginMethod(new{ arg1, arg2}))
        {
            // ... do something ...

            // Log message as you are used to from standar logger
            _logger.LogInformation("Standard log message using string formatter '{0}'", "paramValue1");   

            // Or provide enhanced message with data dump as json
            _logger.LogInformationWithData("Log message with serialized data", new { Key1 = "value1" ... });  

            return ...
        }
    }
}
```

and check your log file:

![Request log sample](readmeImages/logfile.jpg)

## Register loggers by their purpose

### Aggregate only errors or critical messages into separated log file
Register `loggingBuilder.AddFlatLogger("errors", LogLevel.Error, RollingInterval.Day);` to extract only messages with priority Error and higher (includes Critical) to file with suffix "-errors"

### Log single or multiple class usages into separated log file
Register `loggingBuilder.AddTreeLogger("services", RollingInterval.Day, "MyApplication.Controllers", "MyApplication.Services.SpecificService"...);`. All messages and scopes in classes within this namespace will be logged into separated file with suffix "-service".

### Send notification with critical error dump
Register `loggingBuilder.AddSendLogger<MyMailLogSender>(LogLevel.Critical);` and provide class `MyMailLogSender` implementing interface `Pipaslot.Logging.ILogSender`. 
All critical errors will be sent through your sender.

### Log Background worker threads
If you need to log what is happening in thread not related to HTTP request, you can use registration for process logger: `loggingBuilder.AddProcessLogger("processes", RollingInterval.Hour);`

## Automatically erase old logs
Use service `Pipaslot.Logging.FileEraser` registered in dependency injection to erase old files whenever you want:
```
var fileEraser = serviceProvider.GetService<Pipaslot.Logging.FileEraser>();
fileEraser.Run(TimeSpan.FromMonths(3)); //Remove files older than 3 months
```
Or event better you can register HostedService which will run erase job every day with `maxAge` specified in this service.

```
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pipaslot.Logging.Demo.Services
{
    public class LogFileEraseHostedService : BackgroundService
    {
        private readonly ILogger<LogFileEraseHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _maxAge = TimeSpan.FromMonths(3);
        private readonly TimeSpan _repeatInterval = TimeSpan.FromDays(1);

        public LogFileEraseHostedService(ILogger<LogFileEraseHostedService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested){
                using (var scope = _serviceProvider.CreateScope()){
                    var eraser = scope.ServiceProvider.GetService<FileEraser>();
                    var erasedCount = eraser.Run(_maxAge);
                    _logger.LogInformation("Erased {0} log files", erasedCount);
                }
                await Task.Delay(_repeatInterval, stoppingToken);
            }
        }
    }
}
```