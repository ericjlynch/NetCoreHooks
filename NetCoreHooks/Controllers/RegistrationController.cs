﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCoreHooks.Contracts;
using NetCoreHooks.DTOs;
using NetCoreHooks.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetCoreHooks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IEmployeeRepository _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;


        public RegistrationController(ILoggerService loggerService,
            IEmployeeRepository employeeRepository, IMapper mapper, IConfiguration configuration)
        {
            _logger = loggerService;
            _db = employeeRepository;
            _mapper = mapper;
            _config = configuration;
        }

        [HttpPost("dbLookup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyEmployeeSSNByUserName()
        {
            string ssnFromOkta = String.Empty;
            string userName = String.Empty;
            string ssnFromDatabase = String.Empty;

            //grab http reques body
            var reader = new StreamReader(Request.Body);
            var payLoad = await reader.ReadToEndAsync();
            Debug.WriteLine(payLoad);           

            //make sure you have a good payload
            if(payLoad != null)
            {
                //convert incoming http request to JSon JObject
                var parsedJson = JObject.Parse(payLoad);

                //get SSN from payload via Linq-to-JSon
                var userProfile = (JObject)parsedJson.SelectToken("data")
                    .SelectToken("userProfile");
                userName = userProfile["login"].ToString();

                /*did you remember to include SSN in Okta User Profile? 
                 * if you didn't, the ssn key will be null.
                */
                if(userProfile["ssn"] != null)
                {
                    ssnFromOkta = userProfile["ssn"].ToString();
                    Debug.WriteLine(ssnFromOkta);
                } else
                {
                    return BadRequest("Incoming SSN was null");
                }

                //get ssn from database for this user
                var employee = await _db.FindByUserName(userName);
                var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
                ssnFromDatabase = employeeDTO.SSN;
                Debug.WriteLine(ssnFromDatabase);

                //do the SSNs match? 
                if(ssnFromOkta == ssnFromDatabase)
                {
                    //you have a match, now construct your response back to Okta
                    OktaHookResponse response = new OktaHookResponse();
                    Dictionary<String, String> dict = new Dictionary<string, string>
                    {
                        { "ssn", String.Empty }                        
                    };

                    Command command = new Command();
                    command.type = "com.okta.user.profile.update";
                    command.value = dict;
                    response.commands.Add(command);

                    Debug.WriteLine("SSN match detected. Returing 200 OK");
                    Debug.WriteLine("Response sent back to Okta:\n " + response);
                    return Ok(response);
                }
                else
                {
                    //no match. Disallow registration
                    return Unauthorized("Customer SSNs do not match");
                }
            } else
            {
                //payLoad was null
                return BadRequest("no incoming payload detected");
            }           
        }        
    }
}
 