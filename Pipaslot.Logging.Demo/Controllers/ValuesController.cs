using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Demo.Services;

namespace Pipaslot.Logging.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ServiceLevel1 _serviceLevel1;

        public ValuesController(ILogger<ValuesController> logger, ServiceLevel1 serviceLevel1)
        {
            _logger = logger;
            _serviceLevel1 = serviceLevel1;
        }

        [HttpGet]
        public ActionResult<string> PerformComplexAction()
        {
            using var methodScope = _logger.BeginMethod(new {FakeData = "Some fake"});
            _logger.LogError("Fake error with code '{0}' from controller", 123);
            _serviceLevel1.LogScopeAndCriticalMessage();
            _logger.LogInformation($"returned string value ");
            _logger.LogInformationWithData("returned string value ", new {Message = "No Error"});
            
            // var message1 = "No Error";
            // var message2 = "Second";
            // _logger.LogInformation($"returned string value {message1} {message2}");
            // _logger.LogInformation("returned string value {0}", message1, message2);
            // _logger.LogInformation("returned string value {message}", message1, message2);
            // _logger.LogInformationWithData("returned string value ", message1);

            // ActionWitLoggerParameters("First value", "Second value");
            // var structuredMessage = new {Message = "No Error"};
            // var structuredMessage2 = new {Second = "message"};
            // _logger.LogInformation($"returned object value {structuredMessage} {structuredMessage2}");
            // _logger.LogInformation("returned object value {0}", structuredMessage, structuredMessage2, "123");
            // _logger.LogInformation("returned object value {message}", structuredMessage, structuredMessage2);
            //
            // _logger.LogInformationWithData("returned object value {0}", structuredMessage);
            // _logger.LogInformationWithData("returned object value {message}", structuredMessage);
            // _logger.LogInformationWithData("returned object value ", structuredMessage);
            return "Completed";
        }

        private void ActionWitLoggerParameters(string firstParameter, string secondParameter)
        {
            using (_logger.BeginMethod(new {firstParameter, secondParameter})){
                _logger.LogInformation("Hello from logged method");
            }
        }

        [HttpGet("{repeat}")]
        public ActionResult<string> PerformActionWithLogging(int repeat)
        {
            _serviceLevel1.LogMessage(repeat);
            return "Completed";
        }
    }
}