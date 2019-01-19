using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Walterlv.CloudTyping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyboardController : ControllerBase
    {
        // GET api/keyboard
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/keyboard/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/keyboard
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/keyboard/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/keyboard/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
