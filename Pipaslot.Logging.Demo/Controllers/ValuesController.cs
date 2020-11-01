using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.Demo.Services;

namespace Pipaslot.Logging.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
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
            _logger.LogInformationWithData("Service call finished", new {Message = "No Error"});
            return "Completed";
        }

        [HttpGet("{repeat}")]
        public ActionResult<string> PerformActionWithLogging(int repeat)
        {
             _serviceLevel1.LogMessage(repeat);
             return "Completed";
        }
    }
}