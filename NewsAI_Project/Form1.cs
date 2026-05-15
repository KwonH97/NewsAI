using NewsAI_Project.Models;
using NewsAI_Project.Services;
using System;
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

        private async Task SearchNaverNews(string stockName)
        {
            try
            {
                NaverNewsProvider naverNewsProvider = new NaverNewsProvider(Config.NaverId ?? "", Config.NaverSecret ?? "");
                GeminiAnalysisService geminiAnalysisService = new GeminiAnalysisService(Config.GeminiKey ?? "");

                List<NewsItem> newsItems = await naverNewsProvider.SearchAsync(stockName);

                if (newsItems.Count == 0)
                {
                    MessageBox.Show("검색된 뉴스가 없습니다.");
                    return;
                }

                string aiAnalysisResult = await geminiAnalysisService.AnalyzeAsync(newsItems);

                MessageBox.Show("🤖 AI 분석 결과:\n\n" + aiAnalysisResult);
                ShowNewsItems(newsItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show("뉴스 검색 또는 AI 분석 중 오류 발생: " + ex.Message +
                                "\nAPI 키와 네트워크 상태를 확인해 보세요.");
            }
        }

        private void ShowNewsItems(List<NewsItem> newsItems)
        {
            pnlResults.Controls.Clear();
            pnlResults.AutoScroll = true;

            int currentY = 10;

            foreach (NewsItem item in newsItems)
            {
                Panel newsItemPanel = new Panel();
                newsItemPanel.Size = new Size(pnlResults.Width - 40, 100);
                newsItemPanel.Location = new Point(10, currentY);
                newsItemPanel.BorderStyle = BorderStyle.FixedSingle;
                newsItemPanel.BackColor = Color.FromArgb(245, 245, 245);

                Label lblTitle = new Label();
                lblTitle.Text = "📌 " + item.Title;
                lblTitle.Font = new Font("맑은 고딕", 11, FontStyle.Bold);
                lblTitle.ForeColor = Color.Blue;
                lblTitle.Cursor = Cursors.Hand;
                lblTitle.Location = new Point(10, 10);
                lblTitle.AutoSize = true;
                lblTitle.MaximumSize = new Size(newsItemPanel.Width - 20, 0);
                lblTitle.Click += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(item.Link))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = item.Link, UseShellExecute = true });
                    }
                };

                Label lblDesc = new Label();
                lblDesc.Text = "요약: " + item.Description;
                lblDesc.Font = new Font("맑은 고딕", 9);
                lblDesc.Location = new Point(10, 40);
                lblDesc.Size = new Size(newsItemPanel.Width - 20, 50);
                lblDesc.AutoEllipsis = true;

                newsItemPanel.Controls.Add(lblTitle);
                newsItemPanel.Controls.Add(lblDesc);
                pnlResults.Controls.Add(newsItemPanel);

                currentY += 110;
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
                    await SearchNaverNews(stockName);
                }
            }
        }
    }
}
