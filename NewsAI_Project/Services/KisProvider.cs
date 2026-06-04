using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsAI_Project.Models;
using Newtonsoft.Json;

namespace NewsAI_Project.Services
{
    public class KisProvider
    {
        private readonly HttpClient _client;

        private const string BaseUrl =
            "https://openapivts.koreainvestment.com:29443";

        public KisProvider()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetAccessTokenAsync()
        {
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

            return token?.AccessToken ?? "";
        }
    }
}
