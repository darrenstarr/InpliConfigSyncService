namespace InpliConfigSyncService.Controllers
{
    using InpliDataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/Tenant")]
    public class TenantController : ControllerBase<Tenant>
    {
        public TenantController(IConfiguration configuration) :
            base(configuration)
        {
        }

        [HttpGet]
        public new Task<List<Tenant>> Get() => base.Get();

        [HttpPost]
        public new Task Post([FromBody]Tenant item) => base.Post(item);

        [HttpDelete("{id}")]
        public new Task Delete(Guid id) => base.Delete(id);

        [HttpPut("{id}")]
        public new Task Put(int id, [FromBody]Tenant item) => base.Put(id, item);
    }
}
