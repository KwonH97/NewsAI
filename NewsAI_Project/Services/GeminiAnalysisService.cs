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
            
            //Gemini Json Test
            GeminiDebugForm debug = new GeminiDebugForm(modelText);
            debug.ShowDialog();

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
            * event type
            * event keywords
            * key entities
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

            Write summary, claims, and reason in Korean.
            eventKeywords should be Korean.
            keyEntities should preserve original names as they appear in the article.
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

            Critical = 즉시 확인 필요
            High = 장중 확인 필요
            Medium = 오늘 중 확인
            Low = 당장 볼 필요 없음

            marketImpactRange
            Return EXACTLY one of:
            Company
            Sector
            Market

            Company = 해당 기업만 영향
            Sector = 업종 전체 영향
            Market = 시장 전체 영향

            priceImpactScore
            0~20 = 주가 영향 거의 없음
            21~40 = 약한 영향
            41~60 = 보통 영향
            61~80 = 강한 영향
            81~100 = 매우 강한 영향

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
            official_disclosure = DART 공시
            contract_or_earnings = 수주 또는 실적
            company_quote = 회사 공식 발표
            industry_forecast = 산업 전망
            analyst_forecast = 증권사 전망
            speculation = 추측성 기사

            EventType Definitions:

            earnings:
            financial results and earnings announcements

            contract:
            supply contracts, orders, business agreements

            disclosure:
            official regulatory filings and disclosures

            policy:
            government policy or regulation

            product:
            new product or technology announcements

            investment:
            investment, expansion, facility construction

            lawsuit:
            lawsuits, investigations, audits

            management:
            executive changes, governance issues

            analyst_report:
            brokerage opinions and target prices

            market_trend:
            industry-wide trends and market analysis

            rumor:
            unverified claims or speculation

            Choose the single most appropriate category.
            If the article contains multiple event types,
            choose the event that would most strongly
            affect stock price movement.

            Required JSON schema:
            {
              "overallSummary": "2-sentence Korean summary of the overall news flow",
              "articles": [
                {
                  "articleIndex": 0,
                  "summary": "Korean article summary",
                  "claims": [
                    "Korean core claim"
                  ],
                  "claimKeywords": [
                    "삼성전자",
                    "엔비디아",
                    "HBM",
                    "공급계약"
                  ],
                  "eventType": "contract",
                  "eventKeywords": [
                    "공급계약",
                    "수주"
                  ],
                  "keyEntities": [
                    "삼성전자",
                    "엔비디아"
                  ],
                  "impactDirection": "positive | negative | neutral",
                  "impactStrength": "strong | medium | weak",
                  "officialityLevel": "official_disclosure | contract_or_earnings | company_quote | industry_forecast | analyst_forecast | speculation",
                  "specificityLevel": "high | medium | low",
                  "marketReactionSpeed": "Low | Medium | High | Critical",
                  "marketImpactRange": "Company | Sector | Market",
                  "priceImpactScore": 0,
                  "hasHypeExpression": false,
                  "titleContentMismatch": false,
                  "reason": "판단 근거"
                }
              ]
            }

            claimKeywords Rules:
            Extract 3~8 keywords representing the core factual claim of the article.
            Use keywords useful for duplicate detection.
            Do not return generic words.

            Examples:
            삼성전자 공급 계약:
            ["삼성전자", "엔비디아", "HBM", "공급계약"]

            FDA 승인:
            ["FDA", "승인", "신약"]

            eventKeywords Rules:
            Extract keywords that describe the event itself.
            Do not return generic words.

            Examples:
            contract:
            ["공급계약", "수주", "납품"]

            earnings:
            ["실적발표", "영업이익", "매출"]

            policy:
            ["규제", "정책", "지원"]

            lawsuit:
            ["소송", "조사", "검찰"]

            keyEntities Rules:
            Extract important entities appearing in the article.
            Include:
            - company names
            - product names
            - organization names
            - person names
            Do not include generic words.

            Examples:
            ["삼성전자", "엔비디아", "HBM3E"]

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
