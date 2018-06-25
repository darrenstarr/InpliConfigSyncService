namespace InpliConfigSyncService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Net;

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IConfiguration Configuration { get; set; }

        public ValuesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        public class ConfigRequestObject
        {
            public string DeviceId { get; set; }
            public Guid ConfigurationId { get; set; }
        }

        // POST api/values
        [HttpPost]
        public async void Post([FromBody]ConfigRequestObject configRequest)
        {
            // TODO: Database lookup for community relevent to the device.

            var community = "MONKEY";
            var deviceAddress = IPAddress.Parse(configRequest.DeviceId);
            await Services.SnmpTools.SnmpSetAsync(deviceAddress, community, "1.3.6.1.4.1.9.9.96.1.1.1.1.14.111", 6);
            await Services.SnmpTools.SnmpSetAsync(deviceAddress, community, "1.3.6.1.4.1.9.9.96.1.1.1.1.2.111", 1);
            await Services.SnmpTools.SnmpSetAsync(deviceAddress, community, "1.3.6.1.4.1.9.9.96.1.1.1.1.3.111", 4);
            await Services.SnmpTools.SnmpSetAsync(deviceAddress, community, "1.3.6.1.4.1.9.9.96.1.1.1.1.4.111", 1);
            await Services.SnmpTools.SnmpSetAsync(deviceAddress, community, "1.3.6.1.4.1.9.9.96.1.1.1.1.5.111", IPAddress.Parse("10.100.1.105"));
            await Services.SnmpTools.SnmpSetAsync(deviceAddress, community, "1.3.6.1.4.1.9.9.96.1.1.1.1.6.111", configRequest.ConfigurationId.ToString());
            await Services.SnmpTools.SnmpSetAsync(deviceAddress, community, "1.3.6.1.4.1.9.9.96.1.1.1.1.14.111", 1);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
