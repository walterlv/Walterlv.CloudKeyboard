using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Walterlv.CloudTyping
{
    public class CloudKeyboard
    {
        public CloudKeyboard(string baseUrl, string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            _url = $"{baseUrl}/{Token}";
        }

        public string Token { get; }

        public async Task<TypingText> PeekTextAsync()
        {
            // 发送请求。
            using var client = GetHttpClient();
            var responseMessage = await client.GetAsync(_url).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 返回响应。
            var result = JsonConvert.DeserializeObject<TypingText>(response);
            return result;
        }

        public async Task<TypingText> FetchTextAsync()
        {
            // 发送请求。
            using var client = GetHttpClient();
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync(_url, content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 返回响应。
            var result = JsonConvert.DeserializeObject<TypingText>(response);
            return result;
        }

        public async Task<TypingResponse> PutTextAsync(string text,
            int caretStartIndex = -1, int caretEndIndex = -1, bool enter = false)
        {
            // 准备数据。
            var typingText = JsonConvert.SerializeObject(new TypingText(text, caretStartIndex, caretEndIndex, enter));

            // 发送请求。
            using var client = GetHttpClient();
            var content = new StringContent(typingText, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync(_url, content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 返回响应。
            var result = JsonConvert.DeserializeObject<TypingResponse>(response);
            return result;
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(3),
            };

            return client;
        }
        
        private readonly string _url;
    }
}
