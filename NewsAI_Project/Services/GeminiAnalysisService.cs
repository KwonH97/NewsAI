using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewsAI_Project.Models;

namespace NewsAI_Project.Services
{
    public class GeminiAnalysisService
    {
        private readonly string _apiKey;
        private readonly string _model;

        public GeminiAnalysisService(string apiKey, string model = "gemini-3.1-flash-lite")
        {
            _apiKey = apiKey;
            _model = model;
        }

        public async Task<string> AnalyzeAsync(List<NewsItem> newsItems)
        {
            string newsContext = CreateNewsContext(newsItems);
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = $"너는 주식 분석 전문가야. 다음 뉴스들을 읽고 주가에 영향이 큰 핵심 내용만 3줄 요약해줘:\n\n{newsContext}"
                            }
                        }
                    }
                }
            };

            using HttpClient client = new HttpClient();
            string jsonPayload = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(responseBody);
            return json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "분석 결과가 없습니다.";
        }

        private static string CreateNewsContext(List<NewsItem> newsItems)
        {
            return string.Join("\n\n", newsItems.Select(item => $"[제목]: {item.Title}\n[내용]: {item.Description}"));
        }
    }
}
