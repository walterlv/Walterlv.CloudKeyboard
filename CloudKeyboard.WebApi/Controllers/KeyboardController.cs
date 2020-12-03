using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Walterlv.CloudTyping.Models;

namespace Walterlv.CloudTyping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyboardController : ControllerBase
    {
        private readonly KeyboardContext _context;
        private readonly ILogger _logger;

        public KeyboardController(KeyboardContext context, ILogger<KeyboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/keyboard
        /// <summary>
        /// 获取默认页面。
        /// </summary>
        [HttpGet]
        public ActionResult<TypingText> Get()
        {
            _logger.LogInformation("Get");
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
            _logger.LogInformation("Get Token={0}", token);
            var keyboard = _context.Keyboards.Find(token);
            if (keyboard == null)
            {
                return NotFound(new TypingResponse(false, $"Token {token} not found."));
            }

            var value = _context.Typings.FirstOrDefault(x => x.KeyboardToken == token);

            if (value == null)
            {
                return NotFound(new TypingResponse(false, $"Token {token} has no texts."));
            }

            return value.AsClient();
        }

        // GET api/keyboard/5
        /// <summary>
        /// 获取指定 <paramref name="token"/> 下特定 <paramref name="typingId"/> 的文本。
        /// 有可能这是一条早已用过的文本。
        /// </summary>
        [HttpGet("{token}/{typingId}")]
        public ActionResult<TypingText> Get(string token, int typingId)
        {
            var keyboard = _context.Keyboards.Find(token);
            if (keyboard == null)
            {
                return NotFound(new TypingResponse(false, $"Token {token} not found. CurrentToken={string.Join(";", _context.Keyboards.Select(temp => temp.Token))}"));
            }

            var value = _context.Typings.Find(typingId);

            if (value == null)
            {
                return NotFound(new TypingResponse(false, $"Text {typingId} not found."));
            }

            if (value.KeyboardToken != token)
            {
                return NotFound(new TypingResponse(false, $"Accessing to text {typingId} is denied."));
            }

            return value.AsClient();
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
            _logger.LogInformation("Post token={0}", token);

            var keyboard = _context.Keyboards.Find(token);
            if (keyboard == null)
            {
                _context.Keyboards.Add(new Keyboard { Token = token });
                _context.SaveChanges();
                return new TypingText("");
            }

            var value = _context.Typings.FirstOrDefault(x => x.KeyboardToken == token);
            if (value == null)
            {
                return new TypingText("");
            }

            if (value.Enter)
            {
                _context.Typings.Remove(value);
                _context.SaveChanges();
            }

            return value.AsClient();
        }

        // PUT api/keyboard/5
        /// <summary>
        /// 使用指定的 <paramref name="value"/> 替换指定 <paramref name="token"/> 下的输入文本。
        /// </summary>
        [HttpPut("{token}")]
        public ActionResult<TypingResponse> Put(string token, [FromBody] TypingText value)
        {
            _logger.LogInformation("Put token={0} TypingText={1} Enter={2}", token, value.Text, value.Enter);

            var keyboard = _context.Keyboards.Find(token);
            if (keyboard == null)
            {
                return NotFound(new TypingResponse(false, $"Token {token} not found."));
            }

            var lastValue = _context.Typings.LastOrDefault(x => x.KeyboardToken == token);
            if (lastValue == null || lastValue.Enter)
            {
                if (!string.IsNullOrEmpty(value.Text) || value.Enter)
                {
                    _context.Typings.Add(new Models.TypingText(token, value));
                    _context.SaveChanges();
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
                _context.Entry(lastValue).State = EntityState.Modified;
                _context.SaveChanges();
                return new TypingResponse(true, "The message has been updated.");
            }
        }

        // DELETE api/keyboard/5
        /// <summary>
        /// 删除指定的 <paramref name="token"/>，这样就 GET 不到了。
        /// </summary>
        [HttpDelete("{token}")]
        public void Delete(string token)
        {
            _logger.LogInformation("Delete Token={0}", token);

            var keyboard = _context.Keyboards.Find(token);
            if (keyboard != null)
            {
                _context.Keyboards.Remove(keyboard);
                _context.Typings.RemoveRange(_context.Typings.Where(x => x.KeyboardToken == token).ToArray());
            }
        }
    }
}