using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreHooks.Contracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreHooks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private decimal PI = 3.14m;

        public HomeController(ILoggerService loggerService)
        {
            _logger = loggerService;
        }
        // GET: api/<HomeController>
        [HttpGet]
        public string Get()
        {
            Debug.WriteLine("Home/Get action entered");
            _logger.LogInfo("Home Collection Get Action entered");
            return "NetCoreHooks Project has loaded successfully";
        }        
    }
}
