using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> AskChatGPTAsync(string userMessage)
        {
            var apiKey = _configuration["OpenRouter:ApiKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://saviorr.runasp.net/");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "MyGraduationProject");

            var requestBody = new
            {
                model = "deepseek/deepseek-r1-distill-qwen-32b",
                messages = new[]
     {
        new { role = "user", content = userMessage }
    }
            };


            var response = await _httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return $"OpenRouter Error: {response.StatusCode} - {errorText}";
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var content = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return content;
        }
    }

}