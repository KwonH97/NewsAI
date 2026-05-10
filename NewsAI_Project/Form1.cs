using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewsAI_Project
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        public Form1()
        {
            InitializeComponent();
        }

        // --- [네이버 뉴스 검색 함수] ---

        private async Task SearchNaverNews(string stockName)
        {
            // 검색어 인코딩 (예: 삼성전자 주식 뉴스)
            string query = WebUtility.UrlEncode(stockName + " 주가 실적 호재 악재");
            string n_url = $"https://openapi.naver.com/v1/search/news.json?query={query}&display=10&sort=date";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 네이버 API 인증 헤더 추가 (매우 중요)
                    client.DefaultRequestHeaders.Add("X-Naver-Client-Id", Config.NaverId);
                    client.DefaultRequestHeaders.Add("X-Naver-Client-Secret", Config.NaverSecret);

                    // 데이터를 가져옵니다.
                    string responseBody = await client.GetStringAsync(n_url);
                    JObject json = JObject.Parse(responseBody);
                    var items = json["items"];

                    if (items != null && items.Any())
                    {
                        // [추가된 로직 1] 모든 뉴스 제목과 내용을 하나로 합칩니다.
                        string allNewsText = "";
                        foreach (var item in items)
                        {
                            string title = item["title"]?.ToString().Replace("<b>", "").Replace("</b>", "") ?? "";
                            string desc = item["description"]?.ToString().Replace("<b>", "").Replace("</b>", "") ?? "";
                            allNewsText += $"[제목]: {title}\n[내용]: {desc}\n\n";
                        }

                        // [추가된 로직 2] 제미나이에게 분석을 요청합니다.
                        string aiAnalysisResult = await GetGeminiAnalysis(allNewsText);

                        // [추가된 로직 3] UI에 표시 (기존 뉴스 리스트 출력 전이나 후에)
                        this.Invoke(new Action(() => {
                            MessageBox.Show("🤖 AI 분석 결과:\n\n" + aiAnalysisResult);
                            pnlResults.Controls.Clear(); // 기존 결과 삭제
                            pnlResults.AutoScroll = true; // [추가] 뉴스 양이 많으면 스크롤 생성

                            int currentY = 10; // [추가] 뉴스가 배치될 세로 위치 시작점

                            foreach (var item in items)
                            {
                                string title = item["title"]?.ToString()
                                    .Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"") ?? "제목 없음";
                                string description = item["description"]?.ToString()
                                    .Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"") ?? "내용 없음";
                                string link = item["link"]?.ToString() ?? "";

                                // 1. 뉴스 컨테이너 패널
                                Panel newsItemPanel = new Panel();
                                newsItemPanel.Size = new Size(pnlResults.Width - 40, 100);
                                newsItemPanel.Location = new Point(10, currentY); // [수정] currentY 위치에 배치
                                newsItemPanel.BorderStyle = BorderStyle.FixedSingle;
                                newsItemPanel.BackColor = Color.FromArgb(245, 245, 245);

                                // 2. 제목 라벨
                                Label lblTitle = new Label();
                                lblTitle.Text = "📌 " + title;
                                lblTitle.Font = new Font("맑은 고딕", 11, FontStyle.Bold);
                                lblTitle.ForeColor = Color.Blue;
                                lblTitle.Cursor = Cursors.Hand;
                                lblTitle.Location = new Point(10, 10);
                                lblTitle.AutoSize = true;
                                lblTitle.MaximumSize = new Size(newsItemPanel.Width - 20, 0);
                                lblTitle.Click += (s, e) => {
                                    if (!string.IsNullOrEmpty(link))
                                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = link, UseShellExecute = true });
                                };

                                // 3. 내용 라벨
                                Label lblDesc = new Label();
                                lblDesc.Text = "요약: " + description;
                                lblDesc.Font = new Font("맑은 고딕", 9);
                                lblDesc.Location = new Point(10, 40);
                                lblDesc.Size = new Size(newsItemPanel.Width - 20, 50);
                                lblDesc.AutoEllipsis = true;

                                // 패널에 컨트롤 추가
                                newsItemPanel.Controls.Add(lblTitle);
                                newsItemPanel.Controls.Add(lblDesc);

                                // 결과 패널에 추가
                                pnlResults.Controls.Add(newsItemPanel);

                                // [핵심] 다음 뉴스가 그려질 위치 계산 (높이 100 + 간격 10)
                                currentY += 110;
                            }
                        }));
                    

                }
                    else
                    {
                        MessageBox.Show("검색된 뉴스가 없습니다.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("네이버 API 호출 실패: " + ex.Message +
                                    "\nID와 Secret 값이 정확한지 확인해 보세요.");
                }
            }
        }
        
        // 제미나이 api
        private async Task<string> GetGeminiAnalysis(string newsContext)
        {
            try
            {
                // 1. 설정 (현님의 키와 원하는 모델명으로 교체)
                
                string model = "gemini-3.1-flash-lite";
                string g_url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={Config.GeminiKey}";

                // 2. 요청 바디 구성 (JSON)
                var requestBody = new
                {
                    contents = new[]
                    {
                new { parts = new[] { new { text = $"너는 주식 분석 전문가야. 다음 뉴스들을 읽고 주가에 영향이 큰 핵심 내용만 3줄 요약해줘:\n\n{newsContext}" } } }
                    }
                };

                using (HttpClient client = new HttpClient())
                {
                    string jsonPayload = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(g_url, content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(responseBody);
                    // 제미나이의 답변 텍스트 추출
                    return json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "분석 결과가 없습니다.";
                }
            }
            catch (Exception ex)
            {
                return "AI 분석 중 오류 발생: " + ex.Message;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            AlignControls();

            // 1. 먼저 검색창(패널)의 위치를 중앙으로 잡습니다.
            pnlSearchBg.Left = (this.ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = (this.ClientSize.Height - pnlSearchBg.Height) / 2 - 50;

            // 2. 검색창 위치가 결정된 후, 제목(라벨) 위치를 잡습니다.
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblTitle.Top = pnlSearchBg.Top - 50; // 50보다 조금 더 여유를 주는게 보기 좋습니다.

            // 3. 결과창 위치를 검색창 바로 아래로 잡습니다.
            pnlResults.Width = 800;
            pnlResults.Height = 400;
            pnlResults.Left = (this.ClientSize.Width - pnlResults.Width) / 2;
            pnlResults.Top = pnlSearchBg.Bottom + 20;

            // 4. 시각적 설정 (순서가 중요합니다)
            lblTitle.Visible = true;
            lblTitle.BringToFront();
            pnlSearchBg.BackColor = Color.White;

            // 5. [핵심] 모든 위치 계산이 끝난 후 마지막에 모서리를 깎습니다.
            // 0, 0 좌표를 기준으로 패널의 너비와 높이만큼 정확히 깎아줍니다.
            pnlSearchBg.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlSearchBg.Width, pnlSearchBg.Height, 25, 25));
        }
        
        private void AlignControls()
        {
            pnlSearchBg.Left = (this.ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = (this.ClientSize.Height - pnlSearchBg.Height) / 2 - 100;

            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblTitle.Top = pnlSearchBg.Top - 60;
        }

        private async void txtStockSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                lblTitle.Top = 50;
                pnlSearchBg.Top = 110;
                pnlResults.Top = pnlSearchBg.Bottom + 30;

                string stockName = txtStockSearch.Text.Trim();

                if (!string.IsNullOrEmpty(stockName))
                {
                    pnlResults.BringToFront();
                    // 이제 await를 정상적으로 사용할 수 있습니다.
                    await SearchNaverNews(stockName);
                }
            }
        }
    }
}