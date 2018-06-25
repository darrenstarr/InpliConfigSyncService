namespace InpliConfigSyncService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Couchbase;
    using Couchbase.Authentication;
    using Couchbase.Configuration.Client;
    using Couchbase.Core.Serialization;
    using InpliDataModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    [Produces("application/json")]
    [Route("api/Site")]
    public class SiteController : ControllerBase<Site>
    {
        public SiteController(IConfiguration configuration) :
            base(configuration)
        {
        }

        [HttpGet]
        public new Task<List<Site>> Get() => base.Get();

        [HttpPost]
        public new Task Post([FromBody]Site item) => base.Post(item);

        [HttpDelete("{id}")]
        public new Task Delete(Guid id) => base.Delete(id);

        [HttpPut("{id}")]
        public new Task Put(int id, [FromBody]Site item) => base.Put(id, item);

    }
}
