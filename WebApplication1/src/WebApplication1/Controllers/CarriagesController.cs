using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class CarriagesController : Controller
    {
        static readonly IDictionary<int, CarriageDto> Repository = new Dictionary<int, CarriageDto>();

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok(Repository.Values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!Repository.ContainsKey(id))
            {
                return this.NotFound();
            }

            return this.Ok(Repository[id]);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]CarriageDto value)
        {
            Repository[value.Id] = value;
            return this.Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]CarriageDto value)
        {
            Repository[value.Id] = value;

            return this.Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!Repository.ContainsKey(id))
            {
                return this.NotFound();
            }

            Repository.Remove(id);
            return this.Ok();
        }
    }
}
