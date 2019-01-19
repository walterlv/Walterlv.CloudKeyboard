using Microsoft.AspNetCore.Mvc;
using Walterlv.CloudInputMethod.Models;

namespace Walterlv.CloudInputMethod.Controllers
{
    [Route("api/cloudime")]
    [ApiController]
    public class CloudImeController : ControllerBase
    {
        private static string _typingText = "Test";

        // GET api/cloudime
        [HttpGet]
        public ActionResult<TypingText> Get()
        {
            return new TypingText
            {
                Text = _typingText,
            };
        }

        // GET api/cloudime/7f8d
        [HttpGet("{token}")]
        public ActionResult<TypingText> Get(string token)
        {
            return new TypingText
            {
                Text = _typingText,
            };
        }

        // POST api/cloudime
        [HttpPost]
        public void Post([FromBody] string text)
        {
            _typingText = text;
        }

        // PUT api/cloudime/7f8d
        [HttpPut("{token}")]
        public void Put(string token, [FromBody] string text)
        {
            _typingText = text;
        }

        // DELETE api/cloudime/7f8d
        [HttpDelete("{token}")]
        public void Delete(int token)
        {
        }
    }
}
