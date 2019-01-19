using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TypingRepo = System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Concurrent.ConcurrentQueue<Walterlv.CloudTyping.TypingText>>;

namespace Walterlv.CloudTyping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyboardController : ControllerBase
    {
        private static readonly TypingRepo TypingTextRepo = new TypingRepo(new Dictionary<string, string>
        {
            {"0", "Welcome to use walterlv's cloud keyboard."},
        }.ToDictionary(x => x.Key, x => new ConcurrentQueue<TypingText>(new[] {new TypingText(x.Value)})));

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
            if (TypingTextRepo.TryGetValue(token, out var queue)
                && queue.TryPeek(out var value))
            {
                return value;
            }

            return new TypingText("");
        }

        // GET api/keyboard/5
        [HttpPost("{token}")]
        public ActionResult<TypingText> Post(string token)
        {
            if (TypingTextRepo.TryGetValue(token, out var queue))
            {
                queue.TryPeek(out var value);
                if (value.Enter)
                {
                    queue.TryDequeue(out _);
                }

                return value;
            }

            return new TypingText("");
        }

        // PUT api/keyboard/5
        [HttpPost("{token}")]
        public ActionResult<TypingResponse> HttpPost(string token, [FromBody] TypingText value)
        {
            if (TypingTextRepo.TryGetValue(token, out var queue))
            {
                queue.TryPeek(out var originalValue);
                if (originalValue.Enter)
                {
                    queue.Enqueue(value);
                    return new TypingResponse(true, "A new text message has been created.");
                }
                else
                {
                    originalValue.UpdateFrom(value);
                    return new TypingResponse(true, "The message has been updated.");
                }
            }

            return new TypingResponse(false, "Token not found.");
        }

        // DELETE api/keyboard/5
        [HttpDelete("{token}")]
        public void Delete(string token)
        {
            TypingTextRepo.TryRemove(token, out _);
        }
    }
}
