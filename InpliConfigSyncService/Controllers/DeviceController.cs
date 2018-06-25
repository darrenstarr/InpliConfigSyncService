namespace InpliConfigSyncService.Controllers
{
    using InpliDataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/Device")]
    public class DeviceController : ControllerBase<Device>
    {
        public DeviceController(IConfiguration configuration) :
            base(configuration)
        {
        }

        [HttpGet]
        public new Task<List<Device>> Get() => base.Get();

        [HttpPost]
        public new Task Post([FromBody]Device item) => base.Post(item);

        [HttpDelete("{id}")]
        public new Task Delete(Guid id) => base.Delete(id);

        [HttpPut("{id}")]
        public new Task Put(int id, [FromBody]Device item) => base.Put(id, item);
    }
}
