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
        // [수정] 네이버 개발자 센터에서 발급받은 본인의 ID와 Secret을 여기에 넣으세요.
        private string clientId = "ezd6Ej_nSqMVHtWrspPR".Trim();
        private string clientSecret = "ZJTLFzujub".Trim();

        public Form1()
        {
            InitializeComponent();
            TestDBConnection();
            CreateTable();
        }

        // --- [DB 관련 함수] ---

        private void TestDBConnection()
        {
            // 도커 포스트그레스 설정 (기존 유지)
            string connString = "Host=localhost;Username=user123;Password=password123;Database=stock_news";
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("DB 연결 성공!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DB 연결 실패: " + ex.Message);
                }
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
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("테이블 생성 실패: " + ex.Message);
                }
            }
        }

        // --- [네이버 뉴스 검색 함수] ---

        private async Task SearchNaverNews(string stockName)
        {
            // 검색어 인코딩 (예: 삼성전자 주식 뉴스)
            string query = WebUtility.UrlEncode(stockName + " 실적 호재 악재");

            // 최신순(date)으로 뉴스 1개를 가져오는 네이버 API 주소
            string url = $"https://openapi.naver.com/v1/search/news.json?query={query}&display=10&sort=date";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 네이버 API 인증 헤더 추가 (매우 중요)
                    client.DefaultRequestHeaders.Add("X-Naver-Client-Id", clientId);
                    client.DefaultRequestHeaders.Add("X-Naver-Client-Secret", clientSecret);

                    // 데이터를 가져옵니다.
                    string responseBody = await client.GetStringAsync(url);
                    JObject json = JObject.Parse(responseBody);
                    


                    var items = json["items"];
                    if (items != null && items.Any())
                    {
                        this.Invoke(new Action(() => {
                            pnlResults.Controls.Clear(); // 기존 결과 삭제

                            // 2. 반복문을 돌며 뉴스 아이템을 하나씩 생성합니다.
                            foreach (var item in items)
                            {
                                string title = item["title"]?.ToString()
                                    .Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"") ?? "제목 없음";
                                string description = item["description"]?.ToString()
                                    .Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"") ?? "내용 없음";
                                string link = item["link"]?.ToString() ?? "";

                                // 1. 뉴스 하나를 담을 컨테이너 패널
                                Panel newsItemPanel = new Panel();
                                newsItemPanel.Size = new Size(pnlResults.Width - 50, 100); // 높이를 명시적으로 지정
                                newsItemPanel.Margin = new Padding(10, 10, 10, 20);
                                newsItemPanel.BorderStyle = BorderStyle.FixedSingle; // 영역 확인을 위해 테두리 추가
                                newsItemPanel.BackColor = Color.FromArgb(245, 245, 245); // 연한 회색 배경

                                // 2. 제목 라벨
                                Label lblTitle = new Label();
                                lblTitle.Text = "📌 " + title;
                                lblTitle.Font = new Font("맑은 고딕", 11, FontStyle.Bold);
                                lblTitle.ForeColor = Color.Blue;
                                lblTitle.Cursor = Cursors.Hand;
                                lblTitle.Location = new Point(10, 10); // 위치 직접 지정
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
                                lblDesc.Location = new Point(10, 40); // 제목 아래에 위치
                                lblDesc.Size = new Size(newsItemPanel.Width - 20, 50);
                                lblDesc.AutoEllipsis = true; // 내용이 길면 줄임표(...) 처리

                                // [중요!] 컨트롤 추가 순서와 BringToFront
                                newsItemPanel.Controls.Add(lblTitle);
                                newsItemPanel.Controls.Add(lblDesc);

                                pnlResults.Controls.Add(newsItemPanel); // 최종 결과 패널에 추가
                                newsItemPanel.BringToFront();
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