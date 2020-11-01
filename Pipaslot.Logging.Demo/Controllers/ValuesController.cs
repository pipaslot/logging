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
        public ActionResult<string> PerformActionWithLogging()
        {
            using var methodScope = _logger.BeginMethod(new {FakeData = "Some fake"});
            _serviceLevel1.PerformOperationWithLogging();
            _logger.LogError("Fake error from controller");
            return "Completed";
            
        }

        [HttpGet("no-logging")]
        public ActionResult<string> PerformActionWithoutLogging()
        {
            _serviceLevel1.PerformOperationWithoutLogging();
            return "Completed";
        }
    }
}