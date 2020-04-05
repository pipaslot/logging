using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging.WebApp.Services;

namespace Pipaslot.Logging.WebApp.Controllers
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
            _serviceLevel1.PerformOperationWithLogging();
            _logger.LogDebug("Service called");
            return "Completed";
        }
        

        [HttpGet]
        public ActionResult<string> PerformActionWithoutLogging()
        {
            _serviceLevel1.PerformOperationWithoutLogging();
            return "Completed";
        }

    }
}
