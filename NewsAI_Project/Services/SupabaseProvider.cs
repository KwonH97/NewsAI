using NewsAI_Project.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<StockInfo>>
            SearchStocksAsync(string keyword)
        {
            List<StockInfo> result = new();

            try
            {
                string encodedKeyword =
                    Uri.EscapeDataString($"{keyword}%");

                string url =
                    $"{Config.SupabaseUrl}/rest/v1/StockCodes" +
                    $"?stock_name=ilike.{encodedKeyword}" +
                    $"&select=stock_code,stock_name,market_type" +
                    $"&limit=20";

                string json =
                    await _client.GetStringAsync(url);

                JArray arr =
                    JArray.Parse(json);

                foreach (var item in arr)
                {
                    result.Add(
                        new StockInfo
                        {
                            StockCode =
                                item["stock_code"]?.ToString() ?? "",

                            StockName =
                                item["stock_name"]?.ToString() ?? "",

                            MarketType =
                                item["market_type"]?.ToString() ?? ""
                        });
                }
            }
            catch
            {
            }

            return result;
        }
    }
}