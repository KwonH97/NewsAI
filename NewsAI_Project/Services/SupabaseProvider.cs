using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NewsAI_Project.Services
{
    public class SupabaseProvider
    {
        private readonly HttpClient _client;

        public SupabaseProvider()
        {
            _client = new HttpClient();

            _client.DefaultRequestHeaders.Add(
                "apikey",
                Config.SupabaseKey);

            _client.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {Config.SupabaseKey}");
        }

        public async Task<List<string>>
            SearchStocksAsync(string keyword)
        {
            List<string> result = new();

            try
            {
                string encodedKeyword =
                    Uri.EscapeDataString(
                        $"{keyword}%");

                string url =
                    $"{Config.SupabaseUrl}/rest/v1/StockCodes" +
                    $"?stock_name=ilike.{encodedKeyword}" +
                    $"&select=stock_name" +
                    $"&limit=20";

                string json =
                    await _client.GetStringAsync(url);

                JArray arr =
                    JArray.Parse(json);

                foreach (var item in arr)
                {
                    result.Add(
                        item["stock_name"]?
                        .ToString() ?? "");
                }
            }
            catch
            {
            }

            return result;
        }
    }
}
