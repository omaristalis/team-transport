using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    using System.Security.Cryptography;

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        static readonly int[] Values = new int[8];

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            IList<string> values = new List<string>();
            foreach (var value in Values)
            {
                values.Add(value.ToString());
            }
            return values;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            if (id < 0 || id >= Values.Length)
            {
                return string.Empty;
            }

            return Values[id].ToString();
        }

        // POST api/values/5
        [HttpPost]
        public void Post(int id, [FromBody]string value)
        {
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
