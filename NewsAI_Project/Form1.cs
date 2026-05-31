using NewsAI_Project.Models;
using NewsAI_Project.Services;
using System.Runtime.InteropServices;

namespace NewsAI_Project
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public Form1()
        {
            InitializeComponent();
        }

        private async Task SearchNaverNews(string stockName)
        {
            try
            {


                pnlResults.Controls.Clear();
                pnlResults.Controls.Add(CreateTextPanel("분석 중입니다. 뉴스를 수집하고 기사 신뢰도를 계산하고 있습니다.", 70));

                NaverNewsProvider naverNewsProvider = new NaverNewsProvider(Config.NaverId ?? "", Config.NaverSecret ?? "");
                GeminiAnalysisService geminiAnalysisService = new GeminiAnalysisService(Config.GeminiKey ?? "");
                ReliabilityScorer reliabilityScorer = new ReliabilityScorer();

                List<NewsItem> newsItems = await naverNewsProvider.SearchAsync(stockName);

                if (newsItems.Count == 0)
                {
                    pnlResults.Controls.Clear();
                    pnlResults.Controls.Add(CreateTextPanel("검색된 뉴스가 없습니다.", 70));
                    return;
                }

                // DART 기업코드 조회
                var corpProvider =
                    new DartCorpCodeProvider(@"Data\CORPCODE.xml");

                string? corpCode =
                    corpProvider.FindCorpCode(stockName);

                // DART 공시 조회
                var dartProvider =
                    new DartProvider(Config.DartKey ?? "");

                var disclosures =
                    corpCode == null
                        ? new List<DartDisclosure>()
                        : await dartProvider.SearchDisclosureAsync(corpCode);

                NewsAnalysisResult analysisResult =
    await geminiAnalysisService.AnalyzeAsync(newsItems);

                OverallAnalysisResult scoredResult =
                    reliabilityScorer.Score(
                        newsItems,
                        analysisResult,
                        disclosures);

                ShowAnalysisResult(scoredResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("뉴스 검색 또는 AI 분석 중 오류 발생: " + ex.Message +
                                "\nAPI 키와 네트워크 상태를 확인해 보세요.");
            }
        }

        private void ShowAnalysisResult(OverallAnalysisResult result)
        {
            pnlResults.Controls.Clear();
            pnlResults.AutoScroll = true;

            pnlResults.Controls.Add(CreateSummaryPanel(result));

            foreach (ScoredArticleResult article in result.Articles)
            {
                pnlResults.Controls.Add(CreateArticlePanel(article));
            }
        }

        private Panel CreateSummaryPanel(OverallAnalysisResult result)
        {
            Panel panel = new Panel
            {
                Width = pnlResults.Width - 40,
                Height = 130,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235, 242, 255),
                Margin = new Padding(10)
            };

            Label title = new Label
            {
                Text = $"종합 판단: {result.Decision}",
                Font = new Font("맑은 고딕", 13, FontStyle.Bold),
                Location = new Point(12, 10),
                AutoSize = true
            };

            Label score = new Label
            {
                Text = $"총 영향 점수: {result.TotalImpactScore:F1} / 평균 신뢰도: {result.AverageReliabilityScore:F1}점",
                Font = new Font("맑은 고딕", 10, FontStyle.Regular),
                Location = new Point(12, 45),
                AutoSize = true
            };

            Label summary = new Label
            {
                Text = string.IsNullOrWhiteSpace(result.Summary) ? "전체 요약이 없습니다." : result.Summary,
                Font = new Font("맑은 고딕", 9, FontStyle.Regular),
                Location = new Point(12, 75),
                Size = new Size(panel.Width - 24, 45),
                AutoEllipsis = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(score);
            panel.Controls.Add(summary);

            return panel;
        }

        private Panel CreateArticlePanel(ScoredArticleResult article)
        {
            Panel panel = new Panel
            {
                Width = pnlResults.Width - 40,
                Height = 320,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(10)
            };

            Label title = new Label
            {
                Text = article.NewsItem.Title,
                Font = new Font("맑은 고딕", 11, FontStyle.Bold),
                ForeColor = Color.Blue,
                Cursor = Cursors.Hand,
                Location = new Point(10, 10),
                AutoSize = true,
                MaximumSize = new Size(panel.Width - 20, 0)
            };
            title.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(article.NewsItem.Link))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = article.NewsItem.Link,
                        UseShellExecute = true
                    });
                }
            };

            Label score = new Label
            {
                Text =
        $"신뢰도 : {article.Reliability.FinalScore}점 | 영향도 : {article.Analysis.PriceImpactScore}점\n" +
        $"판단 : {ToKoreanDirection(article.Analysis.ImpactDirection)} | 강도 : {ToKoreanStrength(article.Analysis.ImpactStrength)}\n" +
        $"시장 반응 : {ToReactionText(article.Analysis.MarketReactionSpeed)}\n" +
        $"영향 범위 : {ToImpactRangeText(article.Analysis.MarketImpactRange)}",

                Font = new Font("맑은 고딕", 9, FontStyle.Bold),
                Location = new Point(10, 48),
                Size = new Size(panel.Width - 20, 80)
            };

            Label reason = new Label
            {
                Text = "AI 판단 이유 : " + article.Analysis.Reason,
                Font = new Font("맑은 고딕", 8, FontStyle.Italic),
                Location = new Point(10, 140),
                Size = new Size(panel.Width - 20, 40)
            };

            Label detail = new Label
            {
                Text = $"점수 근거: 출처 {article.Reliability.SourceScore}, 공식성 {article.Reliability.OfficialityScore}, 구체성 {article.Reliability.SpecificityScore}, 중복확인 {article.Reliability.DuplicateScore}, 감점 {article.Reliability.PenaltyScore}",
                Font = new Font("맑은 고딕", 8, FontStyle.Regular),
                Location = new Point(10, 195),
                AutoSize = true
            };

            Label summary = new Label
            {
                Text = "요약: " + article.Analysis.Summary,
                Font = new Font("맑은 고딕", 9, FontStyle.Regular),
                Location = new Point(10, 220),
                Size = new Size(panel.Width - 20, 38),
                AutoEllipsis = true
            };

            string claimsText = article.Analysis.Claims.Count == 0
                ? "핵심 주장: 없음"
                : "핵심 주장: " + string.Join(", ", article.Analysis.Claims.Take(3));

            Label claims = new Label
            {
                Text = claimsText,
                Font = new Font("맑은 고딕", 8, FontStyle.Regular),
                Location = new Point(10, 265),
                Size = new Size(panel.Width - 20, 25),
                AutoEllipsis = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(score);
            panel.Controls.Add(reason);
            panel.Controls.Add(detail);
            panel.Controls.Add(summary);
            panel.Controls.Add(claims);

            return panel;
        }

        private Panel CreateTextPanel(string text, int height)
        {
            Panel panel = new Panel
            {
                Width = pnlResults.Width - 40,
                Height = height,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(10)
            };

            Label label = new Label
            {
                Text = text,
                Font = new Font("맑은 고딕", 10, FontStyle.Regular),
                Location = new Point(12, 18),
                AutoSize = true
            };

            panel.Controls.Add(label);
            return panel;
        }

        private static string ToKoreanDirection(string direction)
        {
            return direction.Trim().ToLowerInvariant() switch
            {
                "positive" => "호재",
                "negative" => "악재",
                _ => "중립"
            };
        }

        private static string ToKoreanStrength(string strength)
        {
            return strength.Trim().ToLowerInvariant() switch
            {
                "strong" => "강함",
                "medium" => "보통",
                _ => "약함"
            };
        }

        private static string ToReactionText(MarketReactionSpeed speed)
        {
            return speed switch
            {
                MarketReactionSpeed.Critical => "즉시 확인 필요",
                MarketReactionSpeed.High => "장중 확인 필요",
                MarketReactionSpeed.Medium => "오늘 중 확인",
                _ => "당장 볼 필요 없음"
            };
        }

        private static string ToImpactRangeText(MarketImpactRange range)
        {
            return range switch
            {
                MarketImpactRange.Market => "시장 전체",
                MarketImpactRange.Sector => "업종 전체",
                _ => "해당 기업"
            };
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            AlignControls();

            pnlSearchBg.Left = (ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = (ClientSize.Height - pnlSearchBg.Height) / 2 - 50;

            lblTitle.Left = (ClientSize.Width - lblTitle.Width) / 2;
            lblTitle.Top = pnlSearchBg.Top - 50;

            pnlResults.Width = 900;
            pnlResults.Height = 560;
            pnlResults.Left = (ClientSize.Width - pnlResults.Width) / 2;
            pnlResults.Top = pnlSearchBg.Bottom + 20;

            lblTitle.Visible = true;
            lblTitle.BringToFront();
            pnlSearchBg.BackColor = Color.White;

            pnlSearchBg.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlSearchBg.Width, pnlSearchBg.Height, 25, 25));
        }

        private void AlignControls()
        {
            pnlSearchBg.Left = (ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = (ClientSize.Height - pnlSearchBg.Height) / 2 - 100;

            lblTitle.Left = (ClientSize.Width - lblTitle.Width) / 2;
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
