using NewsAI_Project.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAI_Project.Services
{
    public class KisProvider
    {
        private readonly HttpClient _client;
        private string? _accessToken;
        private const string BaseUrl =
            "https://openapivts.koreainvestment.com:29443";

        public KisProvider()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            }

            var body = new
            {
                grant_type = "client_credentials",
                appkey = Config.KisAppKey,
                appsecret = Config.KisAppSecret
            };

            string json =
                JsonConvert.SerializeObject(body);

            var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    $"{BaseUrl}/oauth2/tokenP",
                    content);

            string result =
                await response.Content.ReadAsStringAsync();

            var token =
                JsonConvert.DeserializeObject<KisTokenResponse>(result);

            _accessToken =
                token?.AccessToken ?? "";

            return _accessToken;
        }

        public async Task<StockPriceInfo?>
    GetCurrentPriceAsync(string stockCode)
        {
            string token =
                await GetAccessTokenAsync();

            using HttpClient client = new();

            client.DefaultRequestHeaders.Add(
                "authorization",
                $"Bearer {token}");

            client.DefaultRequestHeaders.Add(
                "appkey",
                Config.KisAppKey);

            client.DefaultRequestHeaders.Add(
                "appsecret",
                Config.KisAppSecret);

            client.DefaultRequestHeaders.Add(
                "tr_id",
                "FHKST01010100");

            string url =
                $"https://openapivts.koreainvestment.com:29443" +
                $"/uapi/domestic-stock/v1/quotations/inquire-price" +
                $"?fid_cond_mrkt_div_code=J" +
                $"&fid_input_iscd={stockCode}";

            string json =
    await client.GetStringAsync(url);

            JObject obj =
                JObject.Parse(json);

            if (obj["output"] == null)
            {
                return null;
            }

            JObject output =
                (JObject)obj["output"]!;

            return new StockPriceInfo
            {
                CurrentPrice =
                    output["stck_prpr"]?.ToString() ?? "",

                ChangeRate =
                    output["prdy_ctrt"]?.ToString() ?? "",

                Volume =
                    output["acml_vol"]?.ToString() ?? "",

                High52Week =
                    output["w52_hgpr"]?.ToString() ?? ""
            };
        }
    }
}
