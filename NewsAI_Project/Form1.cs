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
        private StockPriceInfo? _stockPrice;
        private OverallAnalysisResult? _lastAnalysisResult;

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
            lblPopularTitle.Visible = true;
            flowPopularStocks.Visible = true;
        }

        private async Task SearchNaverNews(string stockName)
        {
            lblPopularTitle.Visible = false;
            flowPopularStocks.Visible = false;
            HideStockSuggestions();

            try
            {
                flowTrust.Controls.Clear();
                flowImpact.Controls.Clear();
                flowTrust.Controls.Add(
                    CreateTextPanel(
                        "분석 중입니다. 뉴스를 수집하고 기사 신뢰도를 계산하고 있습니다.",
                        70,
                        flowTrust.Width));

                NaverNewsProvider naverNewsProvider =
                    new(Config.NaverId ?? "", Config.NaverSecret ?? "");
                GeminiAnalysisService geminiAnalysisService =
                    new(Config.GeminiKey ?? "");
                ReliabilityScorer reliabilityScorer = new();

                List<NewsItem> newsItems =
                    await naverNewsProvider.SearchAsync(stockName);

                if (newsItems.Count == 0)
                {
                    flowTrust.Controls.Clear();
                    flowTrust.Controls.Add(
                        CreateTextPanel(
                            "검색된 뉴스가 없습니다.",
                            70,
                            flowTrust.Width));
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
        {
            Panel panel = new()
            {
                Width = AnalysisCardWidth,
                Height = 125,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235, 242, 255),
                Margin = new Padding(10)
            };

            Label title = new()
            {
                Text = $"종합 판단: {result.Decision}",
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
                AutoEllipsis = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(score);
            panel.Controls.Add(summary);

            return panel;
        }

        private Panel CreateTrustPanel(ScoredArticleResult article)
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

            Label trust = new()
            {
                Text = $"신뢰도: {article.Reliability.FinalScore}점",
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(10, 48),
                AutoSize = true
            };

            Label summary = new()
            {
                Text = "요약: " + article.Analysis.Summary,
                Font = new Font("맑은 고딕", 8),
                Location = new Point(10, 70),
                MaximumSize = new Size(panel.Width - 20, 0),
                AutoSize = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(trust);
            panel.Controls.Add(summary);
            int maxBottom = panel.Controls.Cast<Control>().Max(c => c.Bottom);
            panel.Height = summary.Bottom + 20;

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
            panel.Controls.Add(reason);

            return panel;
        }

        private static Label CreateLinkedTitle(
            string text,
            string link,
            int panelWidth)
        {
            Label title = new()
            {
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
                Height = height,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(10)
            };

            Label label = new()
            {
                Text = text,
                Font = new Font("맑은 고딕", 10),
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

        /*
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
        */

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

            btnHome.Visible = false;

            btnHome.Size = new Size(70,50);

            flowTrust.FlowDirection = FlowDirection.TopDown;
            flowTrust.WrapContents = false;
            flowImpact.FlowDirection = FlowDirection.TopDown;
            flowImpact.WrapContents = false;
            lstStocks.Visible = false;

            lblCurrentPrice.Text = "현재가: -";
            lblChangeRate.Text = "등락률: -";
            lblVolume.Text = "거래량: -";
            lbl52WeekHigh.Text = "52주 최고가: -";

            tabMain.Visible = false;

            LoadPopularStocks();
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

            int searchWidth = Math.Min(500, Math.Max(420, ClientSize.Width - 160));
            int contentWidth = Math.Min(1100, Math.Max(620, ClientSize.Width - 160));
            int contentHeight = Math.Max(360, ClientSize.Height - 260);

            txtStockSearch.Top = 0;

            // 실제 입력창 크기
            txtStockSearch.Width = searchWidth;
            txtStockSearch.Height = 50;

            // 뒤 배경 패널을 입력창보다 조금만 크게
            pnlSearchBg.Width = txtStockSearch.Width + 10;
            pnlSearchBg.Height = txtStockSearch.Height + 10;

            // 가운데 정렬
            txtStockSearch.Left = 5;
            txtStockSearch.Top = 5;

            pnlSearchBg.Left = (ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = tabMain.Visible ? 110 : (ClientSize.Height - pnlSearchBg.Height) / 2 - 50;

            btnHome.Location =
     new Point(
         pnlSearchBg.Right + 15,
         pnlSearchBg.Top
     );

            lblPopularTitle.Left =
     (ClientSize.Width - lblPopularTitle.Width) / 2;

            lblPopularTitle.Top =
                pnlSearchBg.Bottom + 60;

            flowPopularStocks.Width = 400;
            flowPopularStocks.Height = 180;

            flowPopularStocks.Left =
                (ClientSize.Width - flowPopularStocks.Width) / 2;

            flowPopularStocks.Top =
                lblPopularTitle.Bottom + 20;

            lblTitle.Left = (ClientSize.Width - lblTitle.Width) / 2;
            lblTitle.Top = pnlSearchBg.Top - 50;

            lstStocks.Left = pnlSearchBg.Left;
            lstStocks.Top = pnlSearchBg.Bottom + 6;
            lstStocks.Width = pnlSearchBg.Width;

            tabMain.Width = contentWidth;
            tabMain.Height = contentHeight;
            tabMain.Left = (ClientSize.Width - tabMain.Width) / 2;
            tabMain.Top = pnlSearchBg.Bottom + 30;

            lblCurrentPrice.Location = new Point(20, 16);
            lblChangeRate.Location = new Point(Math.Max(260, tabChart.ClientSize.Width / 2), 16);
            lblVolume.Location = new Point(20, 52);
            lbl52WeekHigh.Location = new Point(Math.Max(260, tabChart.ClientSize.Width / 2), 52);

            pnlSearchBg.Region = Region.FromHrgn(
                CreateRoundRectRgn(
                    0,
                    0,
                    pnlSearchBg.Width,
                    pnlSearchBg.Height,
                    25,
                    25));

            if (_lastAnalysisResult != null)
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

                foreach (Control child in control.Controls)
                {
                    if (child is Label label && !label.AutoSize)
                    {
                        label.Width = Math.Max(100, control.Width - 20);
                    }
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

                int visibleCount = Math.Min(_stocks.Count, 8);

                lstStocks.ItemHeight = 28;

                lstStocks.Height =
                    visibleCount * lstStocks.ItemHeight + 4;

                lstStocks.Visible = _stocks.Count > 0;
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

        private async void lstStocks_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (lstStocks.SelectedItem is not StockInfo selectedStock)
            {
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

                    List<DailyPrice> prices =
                    await _kisProvider.GetDailyPricesAsync(
                        _selectedStockCode);

                    var orderedPrices =
                        prices.OrderBy(x => x.Date).ToList();

                    formsPlotChart.Plot.Clear();

                    var candles =
                        orderedPrices
                        .Select((x, i) =>
                            new ScottPlot.OHLC(
                                (double)x.Open,
                                (double)x.High,
                                (double)x.Low,
                                (double)x.Close,
                                DateTime.FromOADate(i),
                                TimeSpan.FromDays(1)))
                        .ToArray();

                    formsPlotChart.Plot.Add.Candlestick(candles);

                    double[] ma5 =
                    CalculateMA(orderedPrices, 5);

                    double[] ma20 =
                        CalculateMA(orderedPrices, 20);

                    double[] ma60 =
                        CalculateMA(orderedPrices, 60);

                    double[] ma120 =
                        CalculateMA(orderedPrices, 120);

                    double[] ma240 =
                        CalculateMA(orderedPrices, 240);


                    double[] positions =
                        Enumerable.Range(0, orderedPrices.Count)
                        .Where(i => i % 7 == 0)
                        .Select(i => (double)i)
                        .ToArray();

                    string[] labels =
                        orderedPrices
                        .Where((x, i) => i % 7 == 0)
                        .Select(x =>
                        {
                            if (x.Date.Month == 1 &&
                                x.Date.Day <= 7)
                            {
                                return x.Date.ToString("yyyy-MM-dd");
                            }

                            return x.Date.ToString("MM-dd");
                        })
                        .ToArray();

                    formsPlotChart.Plot.Axes.Bottom.SetTicks(
                        positions,
                        labels);

                    double[] xs =
                        Enumerable.Range(0, orderedPrices.Count)
                        .Select(i => (double)i)
                        .ToArray();

                    var ma5Line =
                        formsPlotChart.Plot.Add.Scatter(xs, ma5);

                    var ma20Line =
                        formsPlotChart.Plot.Add.Scatter(xs, ma20);

                    var ma60Line =
                        formsPlotChart.Plot.Add.Scatter(xs, ma60);

                    var ma120Line =
                        formsPlotChart.Plot.Add.Scatter(xs, ma120);

                    var ma240Line =
                        formsPlotChart.Plot.Add.Scatter(xs, ma240);

                    ma5Line.Color = ScottPlot.Colors.Red;
                    ma20Line.Color = ScottPlot.Colors.Orange;
                    ma60Line.Color = ScottPlot.Colors.Green;
                    ma120Line.Color = ScottPlot.Colors.Blue;
                    ma240Line.Color = ScottPlot.Colors.Black;

                    formsPlotChart.Plot.Axes.AutoScale();
                    formsPlotChart.Refresh();

                }
                else
                {
                    ShowStockPrice(null);
                }

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
            btnHome.Visible = true;
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



        private void HideStockSuggestions()
        {
            lstStocks.Items.Clear();
            lstStocks.Visible = false;
            lstStocks.SendToBack();
        }

        // 이동평균선
        private double[] CalculateMA(List<DailyPrice> prices, int period)
        {
            double[] result =
                new double[prices.Count];

            for (int i = 0; i < prices.Count; i++)
            {
                if (i < period - 1)
                {
                    result[i] = double.NaN;
                    continue;
                }

                result[i] =
                    prices
                    .Skip(i - period + 1)
                    .Take(period)
                    .Average(x => (double)x.Close);
            }

            return result;
        }
        private void LoadPopularStocks()
        {
            try
            {
                flowPopularStocks.Controls.Clear();

                lblPopularTitle.Text = "자주 검색하는 대표 종목";
                lblPopularTitle.Font =
    new Font("맑은 고딕", 13, FontStyle.Bold);
                lblPopularTitle.Visible = true;
                lblPopularTitle.BringToFront();

                var stocks = new List<string>
{
    "삼성전자",
    "SK하이닉스",
    "NAVER",
    "카카오",
    "현대차",
    "LG에너지솔루션",
    "한화에어로스페이스",
    "셀트리온",
    "알테오젠",
    "두산에너빌리티"
};




                foreach (string stock in stocks.Take(10))
                {
                    LinkLabel link = new LinkLabel();

                    link.Text = stock;

                    link.LinkColor = Color.Blue;
                    link.ActiveLinkColor = Color.Red;
                    link.VisitedLinkColor = Color.Blue;
                    link.LinkBehavior = LinkBehavior.NeverUnderline;

                    link.Cursor = Cursors.Hand;
                    link.Width = 180;
                    link.Height = 35;

                    link.TextAlign =
                        ContentAlignment.MiddleCenter;

                    link.Font =
                        new Font("맑은 고딕", 10, FontStyle.Bold);

                    link.Click += async (s, e) =>
                    {
                        txtStockSearch.Text = stock;

                        btnHome.Visible = true;

                        var result =
                            await _supabaseProvider.SearchStocksAsync(stock);

                        StockInfo? selectedStock =
                            result.FirstOrDefault(
                                x => x.StockName == stock);

                        if (selectedStock != null)
                        {
                            await AnalyzeSelectedStockAsync(selectedStock);
                        }
                    };


                    flowPopularStocks.Controls.Add(link);


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "인기 종목 조회 오류");
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ShowHomeScreen();

            btnHome.Visible = false;
        }
        private void ShowHomeScreen()
        {
            txtStockSearch.Clear();

            flowTrust.Controls.Clear();
            flowImpact.Controls.Clear();

            lblCurrentPrice.Text = "현재가: -";
            lblChangeRate.Text = "등락률: -";
            lblVolume.Text = "거래량: -";
            lbl52WeekHigh.Text = "52주 최고가: -";

            formsPlotChart.Plot.Clear();
            formsPlotChart.Refresh();

            lblPopularTitle.Visible = true;
            flowPopularStocks.Visible = true;

            lstStocks.Visible = false;

            tabMain.Visible = false;

            LayoutControls();
        }
    }
}
