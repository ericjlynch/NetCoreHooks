using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCoreHooks.Contracts;
using NetCoreHooks.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreHooks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IEmployeeRepository _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IRegistrantRepository _registrantRepository;

        public HomeController(ILoggerService loggerService, 
            IEmployeeRepository employeeRepository, IMapper mapper, 
            IConfiguration configuration, IRegistrantRepository registrantRepository)
        {
            _logger = loggerService;
            _db = employeeRepository;
            _mapper = mapper;
            _config = configuration;
            _registrantRepository = registrantRepository;
        }
        // GET: api/<HomeController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInfo("NetCoreHooks Home Get Action entered.");
                var employees = await _registrantRepository.FindAll();
                if (employees != null)
                {
                    var response = _mapper.Map<IList<RegistrantDTO>>(employees);
                    if (response != null)
                    {
                        StringBuilder sb = new StringBuilder("Employees returned:" + Environment.NewLine);
                        foreach (var employee in employees)
                        {
                            sb.Append(employee.FirstName + " " + employee.LastName + Environment.NewLine);
                        }
                        return Ok(sb.ToString());
                    }
                    else
                    {
                        return NoContent();
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(_config.GetConnectionString("LocalDBConnectionString"));
                return BadRequest();
            }
        }        
    }
}
