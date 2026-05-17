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

        public async Task<NewsAnalysisResult> AnalyzeAsync(List<NewsItem> newsItems)
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
                                text = CreatePrompt(newsContext)
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

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Gemini API call failed: " + responseBody);
            }

            JObject json = JObject.Parse(responseBody);
            string modelText = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "";
            string analysisJson = ExtractJsonObject(modelText);
            NewsAnalysisResult? result = JsonConvert.DeserializeObject<NewsAnalysisResult>(analysisJson);

            if (result == null)
            {
                throw new InvalidOperationException("Could not parse Gemini analysis result.");
            }

            return result;
        }

        private static string CreatePrompt(string newsContext)
        {
            return """
You are a stock-news analysis assistant.
Use only the news list below. Do not invent facts that are not present in the news.
Analyze each article for claims, market impact, officiality, specificity, hype expressions, and title/content mismatch.
Write Korean text inside summary, claims, and reason fields.

Return only valid JSON. Do not include markdown fences, comments, or explanations.

Required JSON schema:
{
  "overallSummary": "2-sentence Korean summary of the overall news flow",
  "articles": [
    {
      "articleIndex": 0,
      "summary": "Korean article summary",
      "claims": ["Korean core claim"],
      "claimKeywords": ["short duplicate-check keyword"],
      "impactDirection": "positive | negative | neutral",
      "impactStrength": "strong | medium | weak",
      "officialityLevel": "official_disclosure | contract_or_earnings | company_quote | industry_forecast | analyst_forecast | speculation",
      "specificityLevel": "high | medium | low",
      "hasHypeExpression": false,
      "titleContentMismatch": false,
      "reason": "Korean reason"
    }
  ]
}

News list:
""" + newsContext;
        }

        private static string CreateNewsContext(List<NewsItem> newsItems)
        {
            return string.Join("\n\n", newsItems.Select((item, index) =>
                $"[ArticleIndex]: {index}\n[Title]: {item.Title}\n[Description]: {item.Description}\n[Link]: {item.Link}\n[PublishedAt]: {item.PublishedAt?.ToString("yyyy-MM-dd HH:mm") ?? "Unknown"}"));
        }

        private static string ExtractJsonObject(string text)
        {
            int start = text.IndexOf('{');
            int end = text.LastIndexOf('}');

            if (start < 0 || end < start)
            {
                throw new InvalidOperationException("Gemini did not return a JSON object.");
            }

            return text.Substring(start, end - start + 1);
        }
    }
}
