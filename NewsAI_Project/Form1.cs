using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewsAI_Project
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        // --- [설정 정보] ---
        private string clientId = "ezd6Ej_nSqMVHtWrspPR".Trim();
        private string clientSecret = "ZJTLFzujub".Trim();
        private string accessToken = "";
        private readonly string appKey = "PS3cORhXtubzXcNHC4l447c4l3GXhINDcb5P";
        private readonly string appSecret = "dogOsk//DBT1z383vA/4aosMJ5ImdWEuhL8zPSe2md6uRHWTVoogemdeSqAZNZ9aln3BsXFa2PuIP5O8705Yjv0rPdGizh0CyOqC3KGEP1nqmweaosn34fGkL+aE8iv2cUK2TEGAcCg2VTzBKN2UxCYQqkQGGyufhDZIS/pXdFVDyqtIpIw=";
        private readonly string domain = "https://openapivts.koreainvestment.com:29443";

        // --- [레이아웃 패널 선언] ---
        private Panel? pnlStockPrice;
        private Panel? pnlResults; // 이제 코드로 생성할 것이므로 필드로 선언합니다.
        private Panel? pnlNewsList;

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1250, 800);
            TestDBConnection();
            CreateTable();
        }

        // --- [메서드: 섹션 패널 생성] ---
        private Panel CreateSectionPanel(int x, int y, int w, int h, string titleText)
        {
            Panel p = new Panel();
            p.Location = new Point(x, y);
            p.Size = new Size(w, h);
            p.BackColor = Color.White;
            p.BorderStyle = BorderStyle.FixedSingle;
            p.AutoScroll = true;
            p.Visible = false;

            Label lbl = new Label();
            lbl.Text = titleText;
            lbl.Dock = DockStyle.Top;
            lbl.Height = 40;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.BackColor = Color.FromArgb(245, 245, 245);
            lbl.Font = new Font("맑은 고딕", 10, FontStyle.Bold);
            lbl.AutoSize = false;

            p.Controls.Add(lbl);
            return p;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await GetAccessToken();

            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            pnlSearchBg.Left = (this.ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = (this.ClientSize.Height - pnlSearchBg.Height) / 2 - 50;
            lblTitle.Top = pnlSearchBg.Top - 60;

            int spacing = 15;
            int totalWidth = 1180;
            int panelWidth = (totalWidth - (spacing * 2)) / 3;
            int panelHeight = 450;
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            int startY = 220;

            // 1. 왼쪽: 실시간 시세 패널 생성
            pnlStockPrice = CreateSectionPanel(startX, startY, panelWidth, panelHeight, "📊 실시간 시세");
            this.Controls.Add(pnlStockPrice);

            // 2. 가운데: AI 투자전략 패널 (코드로 동적 생성)
            pnlResults = CreateSectionPanel(startX + panelWidth + spacing, startY, panelWidth, panelHeight, "🤖 AI 투자전략");
            this.Controls.Add(pnlResults);

            // 3. 오른쪽: 뉴스 리스트 패널 생성
            pnlNewsList = CreateSectionPanel(startX + (panelWidth + spacing) * 2, startY, panelWidth, panelHeight, "📰 관련 뉴스");
            this.Controls.Add(pnlNewsList);

            pnlSearchBg.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlSearchBg.Width, pnlSearchBg.Height, 25, 25));
        }

        private async void txtStockSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string stockName = txtStockSearch.Text.Trim();

                if (!string.IsNullOrEmpty(stockName))
                {
                    lblTitle.Top = 30;
                    pnlSearchBg.Top = 90;

                    // 모든 패널을 보이게 설정
                    if (pnlStockPrice != null) pnlStockPrice.Visible = true;
                    if (pnlResults != null) pnlResults.Visible = true;
                    if (pnlNewsList != null) pnlNewsList.Visible = true;

                    await SearchNaverNews(stockName);
                }
            }
        }

        private async Task SearchNaverNews(string stockName)
        {
            string query = WebUtility.UrlEncode(stockName + " 실적 호재 악재");
            string url = $"https://openapi.naver.com/v1/search/news.json?query={query}&display=10&sort=date";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Naver-Client-Id", clientId);
                    client.DefaultRequestHeaders.Add("X-Naver-Client-Secret", clientSecret);
                    string responseBody = await client.GetStringAsync(url);
                    JObject json = JObject.Parse(responseBody);
                    var items = json["items"];

                    this.Invoke(new Action(() =>
                    {
                        if (pnlNewsList != null)
                        {
                            pnlNewsList.Controls.Clear();
                            Label lbl = new Label { Text = "📰 관련 뉴스", Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.FromArgb(245, 245, 245), Font = new Font("맑은 고딕", 10, FontStyle.Bold), AutoSize = false };
                            pnlNewsList.Controls.Add(lbl);

                            if (items != null && items.Any())
                            {
                                int currentY = 50;
                                foreach (var item in items)
                                {
                                    string title = item["title"]?.ToString().Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"") ?? "제목 없음";
                                    string description = item["description"]?.ToString().Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"") ?? "내용 없음";
                                    string link = item["link"]?.ToString() ?? "";

                                    Panel newsItemPanel = new Panel { Size = new Size(pnlNewsList.Width - 30, 100), Location = new Point(10, currentY), BackColor = Color.FromArgb(250, 250, 250) };
                                    Label lblTitleItem = new Label { Text = "📌 " + title, Font = new Font("맑은 고딕", 9, FontStyle.Bold), ForeColor = Color.Blue, Cursor = Cursors.Hand, Location = new Point(5, 5), Size = new Size(newsItemPanel.Width - 10, 35) };
                                    lblTitleItem.Click += (s, e) => { if (!string.IsNullOrEmpty(link)) System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = link, UseShellExecute = true }); };
                                    Label lblDesc = new Label { Text = description, Font = new Font("맑은 고딕", 8), Location = new Point(5, 40), Size = new Size(newsItemPanel.Width - 10, 50), AutoEllipsis = true };

                                    newsItemPanel.Controls.Add(lblTitleItem);
                                    newsItemPanel.Controls.Add(lblDesc);
                                    pnlNewsList.Controls.Add(newsItemPanel);
                                    currentY += 110;
                                }
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("네이버 API 호출 실패: " + ex.Message);
                    await Task.CompletedTask;
                }
            }
        }

        private async Task GetAccessToken()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{domain}/oauth2/tokenP";
                    var requestData = new { grant_type = "client_credentials", appkey = appKey, appsecret = appSecret };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        JObject jo = JObject.Parse(result);
                        accessToken = jo["access_token"]?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("한투 API 접속 오류: " + ex.Message);
                await Task.CompletedTask;
            }
        }

        private void TestDBConnection()
        {
            string connString = "Host=localhost;Username=user123;Password=password123;Database=stock_news";
            using (var conn = new NpgsqlConnection(connString))
            {
                try { conn.Open(); }
                catch (Exception ex) { MessageBox.Show("DB 연결 실패: " + ex.Message); }
            }
        }

        private void CreateTable()
        {
            string connString = "Host=localhost;Username=user123;Password=password123;Database=stock_news";
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string sql = @"CREATE TABLE IF NOT EXISTS News (
                            id SERIAL PRIMARY KEY,
                            title TEXT,
                            content TEXT,
                            ai_score INT,
                            ai_analysis TEXT,
                            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                          );";
                    using (var cmd = new NpgsqlCommand(sql, conn)) { cmd.ExecuteNonQuery(); }
                }
                catch (Exception ex) { MessageBox.Show("테이블 생성 실패: " + ex.Message); }
            }
        }
    }
}