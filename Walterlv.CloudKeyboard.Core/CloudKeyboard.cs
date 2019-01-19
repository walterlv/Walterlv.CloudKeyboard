using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Walterlv.CloudTyping
{
    public class CloudKeyboard
    {
        public CloudKeyboard(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            _url = $"{BaseUrl}/{Token}";
        }

        public string Token { get; }

        public async Task<TypingText> GetText()
        {
            // 发送请求。
            var client = new HttpClient();
            var response = await (await client.GetAsync(_url).ConfigureAwait(false))
                .Content.ReadAsStringAsync().ConfigureAwait(false);

            // 返回响应。
            var result = JsonConvert.DeserializeObject<TypingText>(response);
            return result;
        }

        public async Task<TypingResponse> PutText(string text,
            int caretStartIndex = -1, int caretEndIndex = -1, bool enter = false)
        {
            // 准备数据。
            var typingText = JsonConvert.SerializeObject(new TypingText(text, caretStartIndex, caretEndIndex, enter));

            // 发送请求。
            var client = new HttpClient();
            var content = new StringContent(typingText, Encoding.UTF8, "application/json");
            var response = await (await client.PutAsync(_url, content).ConfigureAwait(false))
                .Content.ReadAsStringAsync().ConfigureAwait(false);

            // 返回响应。
            var result = JsonConvert.DeserializeObject<TypingResponse>(response);
            return result;
        }

        private const string BaseUrl = "https://localhost:44372/api/keyboard";
        private readonly string _url;
    }
}
