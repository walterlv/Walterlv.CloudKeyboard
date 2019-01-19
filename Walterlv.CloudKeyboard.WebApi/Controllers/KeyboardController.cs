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
                {"0", "Welcome to use walterlv's cloud keyboard."},
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
        public ActionResult<TypingResponse> Put([FromBody] TypingText value)
        {
            return Put("0", value);
        }

        // PUT api/keyboard/5
        [HttpPut("{token}")]
        public ActionResult<TypingResponse> Put(string token, [FromBody] TypingText value)
        {
            TypingTextRepo[token] = value.Text ?? "";
            return new TypingResponse(true);
        }

        // DELETE api/keyboard/5
        [HttpDelete("{token}")]
        public void Delete(string token)
        {
            TypingTextRepo.TryRemove(token, out _);
        }
    }
}
