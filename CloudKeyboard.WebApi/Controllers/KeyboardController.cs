using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Walterlv.CloudTyping.Models;

namespace Walterlv.CloudTyping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyboardController : ControllerBase
    {
        private readonly KeyboardContext _context;

        public KeyboardController(KeyboardContext context)
        {
            _context = context;
        }

        // GET api/keyboard
        /// <summary>
        /// 获取默认页面。
        /// </summary>
        [HttpGet]
        public ActionResult<TypingText> Get()
        {
            return Get("0");
        }

        // GET api/keyboard/5
        /// <summary>
        /// 获取指定 <paramref name="token"/> 下正在输入的文本。
        /// 为了保持幂等性，即使输入操作结束并开始输入下一条文本，此操作也不会得到下一条输入的文本。
        /// 这样，即使不断在浏览器中访问网址，也不会导致 App 中的获取失效。
        /// 在 App 中请使用 POST 方法以便在消息上屏后可以清除上屏的消息并获取到下一条新消息。
        /// </summary>
        [HttpGet("{token}")]
        public ActionResult<TypingText> Get(string token)
        {
            if (_context.TypingTextRepo.TryGetValue(token, out var queue))
            {
                if (queue.TryPeek(out var value))
                {
                    return value;
                }
                else
                {
                    return NotFound(new TypingResponse(false, $"Token {token} has no texts."));
                }
            }

            return NotFound(new TypingResponse(false, $"Token {token} not found."));
        }

        // GET api/keyboard/5
        /// <summary>
        /// 获取指定 <paramref name="token"/> 下正在输入的文本。
        /// 如果此 <paramref name="token"/> 不存在，将创建 Token。
        /// 在获取此消息之后，如果此消息已经上屏，那么此条消息将会被删除，下次访问将返回新输入的一条消息。
        /// </summary>
        [HttpPost("{token}")]
        public ActionResult<TypingText> Post(string token)
        {
            if (_context.TypingTextRepo.TryGetValue(token, out var queue))
            {
                if (queue.TryPeek(out var value))
                {
                    if (value.Enter)
                    {
                        queue.TryDequeue(out _);
                        return value;
                    }
                    else
                    {
                        return value;
                    }
                }
                else
                {
                    return new TypingText("");
                }
            }
            else
            {
                _context.TypingTextRepo[token] = new ConcurrentQueue<TypingText>();
                return new TypingText("");
            }
        }

        // PUT api/keyboard/5
        /// <summary>
        /// 使用指定的 <paramref name="value"/> 替换指定 <paramref name="token"/> 下的输入文本。
        /// </summary>
        [HttpPut("{token}")]
        public ActionResult<TypingResponse> Put(string token, [FromBody] TypingText value)
        {
            if (_context.TypingTextRepo.TryGetValue(token, out var queue))
            {
                var lastValue = queue.LastOrDefault();
                if (lastValue == null || lastValue.Enter)
                {
                    if (!string.IsNullOrEmpty(value.Text) || value.Enter)
                    {
                        queue.Enqueue(value);
                        return new TypingResponse(true, "A new text message has been created.");
                    }
                    else
                    {
                        return new TypingResponse(true, "There is no need to update text message.");
                    }
                }
                else
                {
                    lastValue.UpdateFrom(value);
                    return new TypingResponse(true, "The message has been updated.");
                }
            }
            else
            {
                return NotFound(new TypingResponse(false, $"Token {token} not found."));
            }
        }

        // DELETE api/keyboard/5
        /// <summary>
        /// 删除指定的 <paramref name="token"/>，这样就 GET 不到了。
        /// </summary>
        [HttpDelete("{token}")]
        public void Delete(string token)
        {
            _context.TypingTextRepo.TryRemove(token, out _);
        }
    }
}
