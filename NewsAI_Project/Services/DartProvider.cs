using Newtonsoft.Json.Linq;
using NewsAI_Project.Models;

namespace NewsAI_Project.Services
{
    public class DartProvider
    {
        private readonly string _apiKey;

        public DartProvider(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<List<DartDisclosure>> SearchDisclosureAsync(string corpCode)
        {
            List<DartDisclosure> result = new();

            try
            {
                if (string.IsNullOrWhiteSpace(corpCode))
                    return result;

                string beginDate = DateTime.Today.AddYears(-1).ToString("yyyyMMdd");
                string endDate = DateTime.Today.ToString("yyyyMMdd");

                string url =
                    $"https://opendart.fss.or.kr/api/list.json" +
                    $"?crtfc_key={_apiKey}" +
                    $"&corp_code={corpCode}" +
                    $"&bgn_de={beginDate}" +
                    $"&end_de={endDate}" +
                    $"&last_reprt_at=Y" +
                    $"&page_count=20";

                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true
                };

                using HttpClient client = new(handler);

                client.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0");

                var response = await client.GetAsync(url);


                string json =
                    await response.Content.ReadAsStringAsync();

                JObject obj = JObject.Parse(json);

                string status =
                    obj["status"]?.ToString() ?? "";

                if (status == "013")
                    return result;

                if (status != "000")
                {
                    MessageBox.Show(
                        $"DART 오류 : {obj["message"]}");
                    return result;
                }

                if (obj["list"] == null)
                    return result;

                foreach (var item in obj["list"]!)
                {
                    result.Add(new DartDisclosure
                    {
                        CorpName =
                            item["corp_name"]?.ToString() ?? "",

                        ReportName =
                            item["report_nm"]?.ToString() ?? "",

                        ReceiptNo =
                            item["rcept_no"]?.ToString() ?? "",

                        ReportDate =
                            DateTime.TryParse(
                                item["rcept_dt"]?.ToString(),
                                out DateTime dt)
                                ? dt
                                : DateTime.MinValue
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "DART 오류");
            }

            return result;
        }
    }
}
