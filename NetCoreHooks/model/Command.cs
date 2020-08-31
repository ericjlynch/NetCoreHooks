using AutoMapper.Mappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace NetCoreHooks.model
{
    public class Command
    {
        public string type { get; set; }

        public Dictionary<String, String> value;   
        
    }
}
