using Newtonsoft.Json.Linq;
using NewsAI_Project.Models;
using System.Globalization;
using System.Net;

namespace NewsAI_Project.Services
{
    public class NaverNewsProvider
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        public NaverNewsProvider(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<List<NewsItem>> SearchAsync(string stockName)
        {
            string query = WebUtility.UrlEncode(stockName + " 주가 실적 호재 악재");
            string url = $"https://openapi.naver.com/v1/search/news.json?query={query}&display=10&sort=date";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Naver-Client-Id", _clientId);
            client.DefaultRequestHeaders.Add("X-Naver-Client-Secret", _clientSecret);

            string responseBody = await client.GetStringAsync(url);
            JObject json = JObject.Parse(responseBody);
            var items = json["items"];

            if (items == null)
            {
                return new List<NewsItem>();
            }

            List<NewsItem> newsItems = new List<NewsItem>();

            foreach (var item in items)
            {
                newsItems.Add(new NewsItem
                {
                    Title = CleanNewsText(item["title"]?.ToString() ?? "제목 없음"),
                    Description = CleanNewsText(item["description"]?.ToString() ?? "내용 없음"),
                    Link = item["link"]?.ToString() ?? "",
                    SourceName = "Naver News",
                    SourceType = NewsSourceType.News,
                    PublishedAt = ParsePublishedAt(item["pubDate"]?.ToString())
                });
            }

            return newsItems;
        }

        private static string CleanNewsText(string text)
        {
            return text
                .Replace("<b>", "")
                .Replace("</b>", "")
                .Replace("&quot;", "\"");
        }

        private static DateTime? ParsePublishedAt(string? pubDate)
        {
            if (DateTimeOffset.TryParse(pubDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset publishedAt))
            {
                return publishedAt.LocalDateTime;
            }

            return null;
        }
    }
}
