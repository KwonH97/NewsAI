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
You are a professional stock market news analysis assistant.

Use only the news articles provided below.
Do not invent facts that are not explicitly mentioned in the articles.

Analyze each article and determine:

* summary
* claims
* claim keywords
* stock impact direction
* stock impact strength
* officiality level
* specificity level
* market reaction speed
* market impact range
* expected stock price impact score
* hype expressions
* title/content mismatch
* reasoning

Write summary, claims and reason in Korean.

Return ONLY valid JSON.

Do not return markdown.
Do not return explanations.
Do not return comments.

Definitions:

impactDirection

positive = 호재
negative = 악재
neutral = 중립

impactStrength

strong = 매우 강한 영향
medium = 보통 영향
weak = 약한 영향

marketReactionSpeed

Return EXACTLY one of:

Low
Medium
High
Critical

Critical
= 즉시 확인 필요

High
= 장중 확인 필요

Medium
= 오늘 중 확인

Low
= 당장 볼 필요 없음

marketImpactRange

Return EXACTLY one of:

Company
Sector
Market

Company
= 해당 기업만 영향

Sector
= 업종 전체 영향

Market
= 시장 전체 영향

priceImpactScore

0~20
주가 영향 거의 없음

21~40
약한 영향

41~60
보통 영향

61~80
강한 영향

81~100
매우 강한 영향

Examples of high scores:

* 유상증자
* 감자
* 거래정지
* 상장폐지
* FDA 승인
* 대규모 수주
* 실적 쇼크
* 대규모 투자
* 핵심 기술 계약

officialityLevel

official_disclosure
= DART 공시

contract_or_earnings
= 수주 또는 실적

company_quote
= 회사 공식 발표

industry_forecast
= 산업 전망

analyst_forecast
= 증권사 전망

speculation
= 추측성 기사

Required JSON schema:

{
"overallSummary": "전체 뉴스 흐름 요약",

"articles": [
{
"articleIndex": 0,

  "summary": "기사 요약",

  "claims": [
    "핵심 주장"
  ],

  "claimKeywords": [
    "키워드"
  ],

  "impactDirection":
  "positive | negative | neutral",

  "impactStrength":
  "strong | medium | weak",

  "officialityLevel":
  "official_disclosure | contract_or_earnings | company_quote | industry_forecast | analyst_forecast | speculation",

  "specificityLevel":
  "high | medium | low",

  "marketReactionSpeed":
  "Low | Medium | High | Critical",

  "marketImpactRange":
  "Company | Sector | Market",

  "priceImpactScore":
  0,

  "hasHypeExpression":
  false,

  "titleContentMismatch":
  false,

  "reason":
  "판단 근거"
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
