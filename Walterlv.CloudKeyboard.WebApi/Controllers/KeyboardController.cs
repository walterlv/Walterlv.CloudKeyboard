using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Walterlv.CloudTyping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyboardController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, string> TypingTextRepo
            = new ConcurrentDictionary<string, string>(new Dictionary<string, string>
            {
                {"0", "Test for 0"},
            });

        // GET api/keyboard
        [HttpGet]
        public ActionResult<TypingText> Get()
        {
            return Get("0");
        }

        // GET api/keyboard/5
        [HttpGet("{token}")]
        public ActionResult<TypingText> Get(string token)
        {
            TypingTextRepo.TryGetValue(token, out var value);
            return new TypingText(value);
        }

        // POST api/keyboard
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/keyboard/5
        [HttpPut("{token}")]
        public void Put(string token, [FromBody] string value)
        {
            TypingTextRepo[token] = value;
        }

        // DELETE api/keyboard/5
        [HttpDelete("{token}")]
        public void Delete(string token)
        {
            TypingTextRepo.TryRemove(token, out _);
        }
    }
}
