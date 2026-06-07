using NewsAI_Project.Models;
using NewsAI_Project.Services;
using System.Globalization;
using System.Runtime.InteropServices;

namespace NewsAI_Project
{
    public partial class Form1 : Form
    {
        private const int AnalysisCardHeight = 235;

        private int AnalysisCardWidth =>
            Math.Max(300, tabMain.ClientSize.Width - 44);

        private List<StockInfo> _stocks = new();
        private readonly SupabaseProvider _supabaseProvider = new();
        private readonly KisProvider _kisProvider = new();

        private bool _isSearching;
        private string _selectedStockCode = "";
        private string _selectedMarketType = "";
<<<<<<< HEAD
        private StockPriceInfo? _stockPrice;
        private OverallAnalysisResult? _lastAnalysisResult;
=======
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14

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
            HideStockSuggestions();

            try
            {
<<<<<<< HEAD
                flowTrust.Controls.Clear();
                flowImpact.Controls.Clear();
                flowTrust.Controls.Add(
                    CreateTextPanel(
                        "분석 중입니다. 뉴스를 수집하고 기사 신뢰도를 계산하고 있습니다.",
                        70,
                        flowTrust.Width));
=======


                pnlResults.Controls.Clear();
                pnlResults.Controls.Add(CreateTextPanel("분석 중입니다. 뉴스를 수집하고 기사 신뢰도를 계산하고 있습니다.", 70));
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14

                NaverNewsProvider naverNewsProvider =
                    new(Config.NaverId ?? "", Config.NaverSecret ?? "");
                GeminiAnalysisService geminiAnalysisService =
                    new(Config.GeminiKey ?? "");
                ReliabilityScorer reliabilityScorer = new();

                List<NewsItem> newsItems =
                    await naverNewsProvider.SearchAsync(stockName);

                if (newsItems.Count == 0)
                {
<<<<<<< HEAD
                    flowTrust.Controls.Clear();
                    flowTrust.Controls.Add(
                        CreateTextPanel(
                            "검색된 뉴스가 없습니다.",
                            70,
                            flowTrust.Width));
=======
                    pnlResults.Controls.Clear();
                    pnlResults.Controls.Add(CreateTextPanel("검색된 뉴스가 없습니다.", 70));
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                    return;
                }

                List<DartDisclosure> disclosures =
                    await LoadDartDisclosuresAsync(stockName);

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
                MessageBox.Show(
                    "뉴스 검색 또는 AI 분석 중 오류가 발생했습니다.\n\n" + ex.Message,
                    "분석 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static async Task<List<DartDisclosure>> LoadDartDisclosuresAsync(
            string stockName)
        {
            if (string.IsNullOrWhiteSpace(Config.DartKey))
            {
                return new List<DartDisclosure>();
            }

            string corpCodePath = Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "CORPCODE.xml");

            DartCorpCodeProvider corpProvider = new(corpCodePath);
            string? corpCode = corpProvider.FindCorpCode(stockName);

            if (string.IsNullOrWhiteSpace(corpCode))
            {
                return new List<DartDisclosure>();
            }

            DartProvider dartProvider = new(Config.DartKey);
            return await dartProvider.SearchDisclosureAsync(corpCode);
        }

        private void ShowAnalysisResult(OverallAnalysisResult result)
        {
<<<<<<< HEAD
            _lastAnalysisResult = result;

            flowTrust.Controls.Clear();
            flowImpact.Controls.Clear();

            flowTrust.Controls.Add(CreateOverallPanel(result));
            flowImpact.Controls.Add(CreateOverallPanel(result));

            foreach (ScoredArticleResult article in result.Articles)
            {
                flowTrust.Controls.Add(CreateTrustPanel(article));
                flowImpact.Controls.Add(CreateImpactPanel(article));
            }
        }

        private Panel CreateOverallPanel(OverallAnalysisResult result)
=======
            pnlResults.Controls.Clear();
            pnlResults.AutoScroll = true;

            pnlResults.Controls.Add(CreateSummaryPanel(result));

            foreach (ScoredArticleResult article in result.Articles)
            {
                pnlResults.Controls.Add(CreateArticlePanel(article));
            }
        }

        private Panel CreateSummaryPanel(OverallAnalysisResult result)
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
        {
            Panel panel = new()
            {
<<<<<<< HEAD
                Width = AnalysisCardWidth,
                Height = 125,
=======
                Width = pnlResults.Width - 40,
                Height = 130,
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235, 242, 255),
                Margin = new Padding(10)
            };

            Label title = new()
            {
                Text = $"종합 판단: {result.Decision}",
<<<<<<< HEAD
                Font = new Font("맑은 고딕", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label score = new()
            {
                Text =
                    $"총 영향 점수: {result.TotalImpactScore:F1} / " +
                    $"평균 신뢰도: {result.AverageReliabilityScore:F1}점",
                Font = new Font("맑은 고딕", 9, FontStyle.Bold),
                Location = new Point(10, 42),
                AutoSize = true
            };

            Label summary = new()
            {
                Text = string.IsNullOrWhiteSpace(result.Summary)
                    ? "전체 뉴스 요약이 없습니다."
                    : result.Summary,
                Font = new Font("맑은 고딕", 9),
                Location = new Point(10, 70),
                Size = new Size(panel.Width - 20, 45),
=======
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
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                AutoEllipsis = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(score);
            panel.Controls.Add(summary);

            return panel;
        }

<<<<<<< HEAD
        private Panel CreateTrustPanel(ScoredArticleResult article)
=======
        private Panel CreateArticlePanel(ScoredArticleResult article)
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
        {
            Panel panel = new()
            {
<<<<<<< HEAD
                Width = AnalysisCardWidth,
                Height = AnalysisCardHeight,
=======
                Width = pnlResults.Width - 40,
                Height = 320,
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(10)
            };

<<<<<<< HEAD
            Label title = CreateLinkedTitle(
                article.NewsItem.Title,
                article.NewsItem.Link,
                panel.Width);

            Label trust = new()
            {
                Text = $"신뢰도: {article.Reliability.FinalScore}점",
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(10, 48),
                AutoSize = true
            };

            Label breakdown = new()
            {
                Text =
                    $"출처 {article.Reliability.SourceScore} / " +
                    $"공식성 {article.Reliability.OfficialityScore} / " +
                    $"구체성 {article.Reliability.SpecificityScore} / " +
                    $"중복 {article.Reliability.DuplicateScore} / " +
                    $"DART {article.Reliability.DartScore} / " +
                    $"감점 {article.Reliability.PenaltyScore}",
                Font = new Font("맑은 고딕", 8),
                Location = new Point(10, 76),
                Size = new Size(panel.Width - 20, 25),
                AutoEllipsis = true
=======
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
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
            };

            Label summary = new()
            {
                Text = "요약: " + article.Analysis.Summary,
                Font = new Font("맑은 고딕", 8),
                Location = new Point(10, 104),
                Size = new Size(panel.Width - 20, 84),
                AutoEllipsis = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(trust);
            panel.Controls.Add(breakdown);
            panel.Controls.Add(summary);

            return panel;
        }

        private Panel CreateImpactPanel(ScoredArticleResult article)
        {
            Panel panel = new()
            {
                Width = AnalysisCardWidth,
                Height = AnalysisCardHeight,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            Label title = CreateLinkedTitle(
                article.NewsItem.Title,
                article.NewsItem.Link,
                panel.Width);

            Label impact = new()
            {
<<<<<<< HEAD
                Text =
                    $"AI 예상 영향도: {article.Analysis.PriceImpactScore}점 / " +
                    $"신뢰도 반영 점수: {article.ImpactScore:F1}",
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = article.ImpactScore < 0
                    ? Color.RoyalBlue
                    : Color.Firebrick,
                Location = new Point(10, 48),
                AutoSize = true
            };

            Label info = new()
            {
                Text =
                    $"판단: {ToKoreanDirection(article.Analysis.ImpactDirection)}\n" +
                    $"이벤트: {ToEventTypeText(article.Analysis.EventType)}\n" +
                    $"반응 속도: {ToReactionText(article.Analysis.MarketReactionSpeed)}\n" +
                    $"영향 범위: {ToImpactRangeText(article.Analysis.MarketImpactRange)}",
                Font = new Font("맑은 고딕", 9),
                Location = new Point(10, 78),
                Size = new Size(panel.Width - 20, 80)
            };

            string keywords = article.Analysis.EventKeywords.Count == 0
                ? "없음"
                : string.Join(", ", article.Analysis.EventKeywords.Take(6));

            Label keywordLabel = new()
            {
                Text = "핵심 키워드: " + keywords,
                Font = new Font("맑은 고딕", 8),
                Location = new Point(10, 160),
                Size = new Size(panel.Width - 20, 25),
                AutoEllipsis = true
            };

            Label reason = new()
            {
                Text = "AI 판단 이유: " + article.Analysis.Reason,
                Font = new Font("맑은 고딕", 8),
                Location = new Point(10, 188),
                Size = new Size(panel.Width - 20, 38),
                AutoEllipsis = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(impact);
            panel.Controls.Add(info);
            panel.Controls.Add(keywordLabel);
=======
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
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
            panel.Controls.Add(reason);
            panel.Controls.Add(detail);
            panel.Controls.Add(summary);
            panel.Controls.Add(claims);

            return panel;
        }

<<<<<<< HEAD
        private static Label CreateLinkedTitle(
            string text,
            string link,
            int panelWidth)
=======
        private Panel CreateTextPanel(string text, int height)
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
        {
            Label title = new()
            {
<<<<<<< HEAD
                Text = text,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = Color.Blue,
                Location = new Point(10, 10),
                Size = new Size(panelWidth - 20, 32),
                Cursor = Cursors.Hand,
                AutoEllipsis = true
            };

            title.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(link))
                {
                    return;
                }

                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = link,
                        UseShellExecute = true
                    });
            };

            return title;
        }

        private static Panel CreateTextPanel(
            string text,
            int height,
            int parentWidth)
        {
            Panel panel = new()
            {
                Width = Math.Max(300, parentWidth - 40),
=======
                Width = pnlResults.Width - 40,
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                Height = height,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(10)
            };

            Label label = new()
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

<<<<<<< HEAD
=======
        private static string ToKoreanStrength(string strength)
        {
            return strength.Trim().ToLowerInvariant() switch
            {
                "strong" => "강함",
                "medium" => "보통",
                _ => "약함"
            };
        }

>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
        private static string ToReactionText(MarketReactionSpeed speed)
        {
            return speed switch
            {
                MarketReactionSpeed.Critical => "즉시 확인 필요",
                MarketReactionSpeed.High => "장중 확인 필요",
                MarketReactionSpeed.Medium => "오늘 중 확인",
                _ => "당장 확인할 필요 없음"
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

        private static string ToEventTypeText(string eventType)
        {
            return eventType.Trim().ToLowerInvariant() switch
            {
                "earnings" => "실적",
                "contract" => "계약/수주",
                "disclosure" => "공시",
                "policy" => "정책/규제",
                "product" => "제품/기술",
                "investment" => "투자/증설",
                "lawsuit" => "소송/조사",
                "management" => "경영/지배구조",
                "analyst_report" => "증권사 리포트",
                "market_trend" => "시장/업황",
                "rumor" => "루머/추측",
                _ => "기타"
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            flowTrust.FlowDirection = FlowDirection.TopDown;
            flowTrust.WrapContents = false;
            flowImpact.FlowDirection = FlowDirection.TopDown;
            flowImpact.WrapContents = false;
            picChart.Dock = DockStyle.None;
            picChart.SizeMode = PictureBoxSizeMode.StretchImage;

<<<<<<< HEAD
            lblCurrentPrice.Text = "현재가: -";
            lblChangeRate.Text = "등락률: -";
            lblVolume.Text = "거래량: -";
            lbl52WeekHigh.Text = "52주 최고가: -";

            tabMain.Visible = false;
            LayoutControls();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutControls();
        }

        private void LayoutControls()
        {
            if (!IsHandleCreated)
            {
                return;
            }

            int searchWidth = Math.Min(700, Math.Max(420, ClientSize.Width - 160));
            int contentWidth = Math.Min(1100, Math.Max(620, ClientSize.Width - 160));
            int contentHeight = Math.Max(360, ClientSize.Height - 260);

            pnlSearchBg.Width = searchWidth;
            txtStockSearch.Width = pnlSearchBg.Width - 20;
=======
            pnlResults.Width = 900;
            pnlResults.Height = 560;
            pnlResults.Left = (ClientSize.Width - pnlResults.Width) / 2;
            pnlResults.Top = pnlSearchBg.Bottom + 20;
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14

            pnlSearchBg.Left = (ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = tabMain.Visible ? 110 : (ClientSize.Height - pnlSearchBg.Height) / 2 - 50;

            lblTitle.Left = (ClientSize.Width - lblTitle.Width) / 2;
            lblTitle.Top = pnlSearchBg.Top - 50;

<<<<<<< HEAD
=======
            pnlResults.Width = 900;
            pnlResults.Height = 560;
            pnlResults.Left = (ClientSize.Width - pnlResults.Width) / 2;
            pnlResults.Top = pnlSearchBg.Bottom + 20;

            lblTitle.Visible = true;
            lblTitle.BringToFront();
            pnlSearchBg.BackColor = Color.White;

            pnlSearchBg.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlSearchBg.Width, pnlSearchBg.Height, 25, 25));

            pnlSearchBg.Left =
    (ClientSize.Width - pnlSearchBg.Width) / 2;

            pnlSearchBg.Top =
                (ClientSize.Height - pnlSearchBg.Height) / 2 - 50;

            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;

            lblTitle.Top =
                pnlSearchBg.Top - 50;

>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
            lstStocks.Left = pnlSearchBg.Left;
            lstStocks.Top = pnlSearchBg.Bottom + 2;
            lstStocks.Width = pnlSearchBg.Width;
            lstStocks.Height = 120;

            tabMain.Width = contentWidth;
            tabMain.Height = contentHeight;
            tabMain.Left = (ClientSize.Width - tabMain.Width) / 2;
            tabMain.Top = pnlSearchBg.Bottom + 30;

<<<<<<< HEAD
            lblCurrentPrice.Location = new Point(20, 16);
            lblChangeRate.Location = new Point(Math.Max(260, tabChart.ClientSize.Width / 2), 16);
            lblVolume.Location = new Point(20, 52);
            lbl52WeekHigh.Location = new Point(Math.Max(260, tabChart.ClientSize.Width / 2), 52);

            picChart.Left = 20;
            picChart.Top = 95;
            picChart.Width = Math.Max(300, tabChart.ClientSize.Width - 40);
            picChart.Height = Math.Max(220, tabChart.ClientSize.Height - picChart.Top - 20);

            pnlSearchBg.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    pnlSearchBg.Width,
                    pnlSearchBg.Height,
                    25,
                    25));

            if (_lastAnalysisResult != null)
=======
            pnlSearchBg.Region =
                Region.FromHrgn(
                    CreateRoundRectRgn(
                        0,
                        0,
                        pnlSearchBg.Width,
                        pnlSearchBg.Height,
                        25,
                        25));
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
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
            {
                ResizeAnalysisPanels(flowTrust);
                ResizeAnalysisPanels(flowImpact);
            }
        }

        private void ResizeAnalysisPanels(FlowLayoutPanel panel)
        {
            foreach (Control control in panel.Controls)
            {
                control.Width = control.Height == 125
                    ? AnalysisCardWidth
                    : AnalysisCardWidth;

<<<<<<< HEAD
                foreach (Control child in control.Controls)
=======
                lblTitle.Top = 50;
                pnlSearchBg.Top = 110;
                pnlResults.Top = pnlSearchBg.Bottom + 30;

                lstStocks.Left = pnlSearchBg.Left;
                lstStocks.Top = pnlSearchBg.Bottom + 2;

                string stockName =
                    txtStockSearch.Text.Trim();

                if (!string.IsNullOrEmpty(stockName))
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                {
                    if (child is Label label && !label.AutoSize)
                    {
                        label.Width = Math.Max(100, control.Width - 20);
                    }
<<<<<<< HEAD
=======

                    _selectedStockCode =
                        selectedStock.StockCode;

                    _selectedMarketType =
                        selectedStock.MarketType;            

                    await SearchNaverNews(stockName);

                    txtStockSearch.SelectionStart =
                        txtStockSearch.Text.Length;

                    lstStocks.Items.Clear();
                    lstStocks.Visible = false;
                    lstStocks.SendToBack();

                    _isSearching = false;
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                }
            }
        }

        private async void txtStockSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            string stockName = txtStockSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(stockName))
            {
                return;
            }

            StockInfo? selectedStock = _stocks.FirstOrDefault(
                stock => stock.StockName.Equals(
                    stockName,
                    StringComparison.OrdinalIgnoreCase));

            if (selectedStock == null)
            {
                MessageBox.Show(
                    "정확한 종목명을 입력하거나 자동완성 목록에서 선택해 주세요.",
                    "종목명 확인",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            await AnalyzeSelectedStockAsync(selectedStock);
        }

        private async void txtStockSearch_TextChanged(object? sender, EventArgs e)
        {
            if (_isSearching)
            {
                return;
            }

            string keyword = txtStockSearch.Text.Trim();

            if (keyword.Length < 2)
            {
                HideStockSuggestions();
                return;
            }

            try
            {
                _stocks = await _supabaseProvider.SearchStocksAsync(keyword);

                if (_isSearching)
                {
                    return;
                }

                lstStocks.Items.Clear();

                foreach (StockInfo stock in _stocks)
                {
                    lstStocks.Items.Add(stock);
                }

                lstStocks.Visible = _stocks.Count > 0;
                lstStocks.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "검색 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

<<<<<<< HEAD
        private async void lstStocks_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (lstStocks.SelectedItem is not StockInfo selectedStock)
            {
=======
        private async void lstStocks_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstStocks.SelectedItem == null)
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
                return;
            }

            await AnalyzeSelectedStockAsync(selectedStock);
        }

        private async Task AnalyzeSelectedStockAsync(StockInfo selectedStock)
        {
            if (_isSearching)
            {
                return;
            }

<<<<<<< HEAD
            try
            {
                _isSearching = true;
                _selectedStockCode = selectedStock.StockCode;
                _selectedMarketType = selectedStock.MarketType;

                txtStockSearch.Text = selectedStock.StockName;
                txtStockSearch.SelectionStart = txtStockSearch.Text.Length;
                HideStockSuggestions();

                tabMain.Visible = true;
                LayoutControls();

                await SearchNaverNews(selectedStock.StockName);

                if (!string.IsNullOrWhiteSpace(Config.KisAppKey) &&
                    !string.IsNullOrWhiteSpace(Config.KisAppSecret))
                {
                    _stockPrice = await _kisProvider.GetCurrentPriceAsync(
                        _selectedStockCode);
                    ShowStockPrice(_stockPrice);
                }
                else
                {
                    ShowStockPrice(null);
                }

                await LoadChartAsync(_selectedStockCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "종목 분석 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _isSearching = false;
            }
        }

        private void ShowStockPrice(StockPriceInfo? stockPrice)
        {
            if (stockPrice == null)
            {
                lblCurrentPrice.Text = "현재가: 조회 불가";
                lblChangeRate.Text = "등락률: 조회 불가";
                lblChangeRate.ForeColor = Color.Black;
                lblVolume.Text = "거래량: 조회 불가";
                lbl52WeekHigh.Text = "52주 최고가: 조회 불가";
                return;
            }

            lblCurrentPrice.Text = int.TryParse(
                stockPrice.CurrentPrice,
                out int currentPrice)
                ? $"현재가: {currentPrice:N0}원"
                : "현재가: 조회 불가";

            lblChangeRate.Text = $"등락률: {stockPrice.ChangeRate}%";
            lblChangeRate.ForeColor = GetChangeRateColor(stockPrice.ChangeRate);

            lblVolume.Text = long.TryParse(
                stockPrice.Volume,
                out long volume)
                ? $"거래량: {volume:N0}"
                : "거래량: 조회 불가";

            lbl52WeekHigh.Text = int.TryParse(
                stockPrice.High52Week,
                out int high52Week)
                ? $"52주 최고가: {high52Week:N0}원"
                : "52주 최고가: 조회 불가";
        }

        private static Color GetChangeRateColor(string changeRateText)
        {
            string normalized = changeRateText
                .Replace("%", "")
                .Replace("+", "")
                .Trim();

            if (!decimal.TryParse(
                    normalized,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out decimal changeRate))
            {
                return Color.Black;
            }

            if (changeRate > 0)
            {
                return Color.Red;
            }

            if (changeRate < 0)
            {
                return Color.Blue;
            }

            return Color.Black;
        }

        private async Task LoadChartAsync(string stockCode)
        {
            try
            {
                string url =
                    $"https://ssl.pstatic.net/imgfinance/chart/item/candle/week/{stockCode}.png";

                using HttpClient client = new();
                byte[] bytes = await client.GetByteArrayAsync(url);
                using MemoryStream stream = new(bytes);
                using Image downloadedImage = Image.FromStream(stream);

                Image? previousImage = picChart.Image;
                picChart.Image = new Bitmap(downloadedImage);
                previousImage?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "차트를 불러오지 못했습니다.\n\n" + ex.Message,
                    "차트 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
=======
            _isSearching = true;

            StockInfo selectedStock = (StockInfo)lstStocks.SelectedItem;

            string stockName =
        selectedStock.StockName;

            string stockCode =
                selectedStock.StockCode;

            string marketType =
                selectedStock.MarketType;

            _selectedStockCode = stockCode;
            _selectedMarketType = marketType;           

            txtStockSearch.Text = stockName;

            lstStocks.Items.Clear();
            lstStocks.Visible = false;
            lstStocks.SendToBack();

            lblTitle.Top = 50;
            pnlSearchBg.Top = 110;
            pnlResults.Top = pnlSearchBg.Bottom + 30;

            lstStocks.Left = pnlSearchBg.Left;
            lstStocks.Top = pnlSearchBg.Bottom + 2;

            await SearchNaverNews(stockName);

            _isSearching = false;
>>>>>>> 0f90dcb5a8589cfce238cdc37e5b62f684d34d14
        }

        private void HideStockSuggestions()
        {
            lstStocks.Items.Clear();
            lstStocks.Visible = false;
            lstStocks.SendToBack();
        }
    }
}
