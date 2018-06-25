namespace InpliConfigSyncService.Controllers
{
    using InpliDataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/AuthenticationProfile")]
    public class AuthenticationProfileController : ControllerBase<AuthenticationProfile>
    {
        public AuthenticationProfileController(IConfiguration configuration) : 
            base(configuration)
        {
        }

        [HttpGet]
        public new Task<List<AuthenticationProfile>> Get() => base.Get();

        [HttpPost]
        public new Task Post([FromBody]AuthenticationProfile item) => base.Post(item);

        [HttpDelete("{id}")]
        public new Task Delete(Guid id) => base.Delete(id);

        [HttpPut("{id}")]
        public new Task Put(int id, [FromBody]AuthenticationProfile item) => base.Put(id, item);
    }
}
