using NewsAI_Project.Models;
using NewsAI_Project.Services;
using System.Runtime.InteropServices;
using System.Net;

namespace NewsAI_Project
{
    public partial class Form1 : Form
    {
        private List<StockInfo> _stocks = new();

        private readonly SupabaseProvider _supabaseProvider =
                                                new SupabaseProvider();

        private readonly KisProvider _kisProvider =
        new KisProvider();

        private bool _isSearching = false;

        private string _selectedStockCode = "";
        private string _selectedMarketType = "";

        private StockPriceInfo? _stockPrice;

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
            lstStocks.Items.Clear();
            lstStocks.Visible = false;
            lstStocks.SendToBack();

            try
            {


                flowTrust.Controls.Clear();
                flowTrust.Controls.Add(CreateTextPanel("분석 중입니다. 뉴스를 수집하고 기사 신뢰도를 계산하고 있습니다.", 70));

                NaverNewsProvider naverNewsProvider = new NaverNewsProvider(Config.NaverId ?? "", Config.NaverSecret ?? "");
                GeminiAnalysisService geminiAnalysisService = new GeminiAnalysisService(Config.GeminiKey ?? "");
                ReliabilityScorer reliabilityScorer = new ReliabilityScorer();

                List<NewsItem> newsItems = await naverNewsProvider.SearchAsync(stockName);

                if (newsItems.Count == 0)
                {
                    flowTrust.Controls.Clear();
                    flowTrust.Controls.Add(CreateTextPanel("검색된 뉴스가 없습니다.", 70));
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
            // 신뢰도 탭
            flowTrust.Controls.Clear();

            foreach (ScoredArticleResult article in result.Articles)
            {
                flowTrust.Controls.Add(
                    CreateTrustPanel(article));
            }

            // 중요도 탭
            flowImpact.Controls.Clear();

            foreach (ScoredArticleResult article in result.Articles)
            {
                flowImpact.Controls.Add(
                    CreateImpactPanel(article));
            }
        }

        private Panel CreateTrustPanel(
    ScoredArticleResult article)
        {
            Panel panel = new Panel
            {
                Width = flowTrust.Width - 30,
                Height = 90,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            Label title = new Label
            {
                Text = article.NewsItem.Title,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = Color.Blue,
                Location = new Point(10, 10),
                Size = new Size(panel.Width - 20, 30),

                Cursor = Cursors.Hand
            };

            title.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = article.NewsItem.Link,
                        UseShellExecute = true
                    });
            };

            Label trust = new Label
            {
                Text = $"신뢰도 : {article.ReliabilityScore}점",
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(10, 45),
                AutoSize = true
            };

            panel.Controls.Add(title);
            panel.Controls.Add(trust);

            return panel;
        }

        private Panel CreateImpactPanel(
    ScoredArticleResult article)
        {
            Panel panel = new Panel
            {
                Width = flowImpact.Width - 40,
                Height = 220,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            Label title = new Label
            {
                Text = article.NewsItem.Title,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = Color.Blue,
                Location = new Point(10, 10),
                Size = new Size(panel.Width - 20, 35),
                Cursor = Cursors.Hand
            };

            title.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = article.NewsItem.Link,
                        UseShellExecute = true
                    });
            };

            Label impact = new Label
            {
                Text =
                    $"영향도 : {article.Analysis.PriceImpactScore}점",
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                ForeColor = Color.Red,
                Location = new Point(10, 55),
                AutoSize = true
            };

            Label info = new Label
            {
                Text =
                    $"판단 : {ToKoreanDirection(article.Analysis.ImpactDirection)}\n" +
                    $"긴급성 : {ToReactionText(article.Analysis.MarketReactionSpeed)}\n" +
                    $"영향 범위 : {ToImpactRangeText(article.Analysis.MarketImpactRange)}",
                Font = new Font("맑은 고딕", 9),
                Location = new Point(10, 85),
                Size = new Size(panel.Width - 20, 70)
            };

            Label reason = new Label
            {
                Text =
                    "AI 판단 이유 : " +
                    article.Analysis.Reason,
                Font = new Font("맑은 고딕", 9),
                Location = new Point(10, 160),
                Size = new Size(panel.Width - 20, 45)
            };

            panel.Controls.Add(title);
            panel.Controls.Add(impact);
            panel.Controls.Add(info);
            panel.Controls.Add(reason);

            return panel;
        }


        private Panel CreateTextPanel(string text, int height)
        {
            Panel panel = new Panel
            {
                Width = flowTrust.Width - 40,
                Height = height,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(10)
            };

            Label label = new Label
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

            tabMain.Width = 900;
            tabMain.Height = 560;
            tabMain.Left = (ClientSize.Width - tabMain.Width) / 2;
            tabMain.Top = pnlSearchBg.Bottom + 20;

            pnlSearchBg.Left = (ClientSize.Width - pnlSearchBg.Width) / 2;
            pnlSearchBg.Top = (ClientSize.Height - pnlSearchBg.Height) / 2 - 50;

            lblTitle.Left = (ClientSize.Width - lblTitle.Width) / 2;
            lblTitle.Top = pnlSearchBg.Top - 50;

            

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

            lstStocks.Left = pnlSearchBg.Left;
            lstStocks.Top = pnlSearchBg.Bottom + 2;

            lstStocks.Width = pnlSearchBg.Width;
            lstStocks.Height = 120;

            lstStocks.Visible = false;

            pnlSearchBg.Region =
                Region.FromHrgn(
                    CreateRoundRectRgn(
                        0,
                        0,
                        pnlSearchBg.Width,
                        pnlSearchBg.Height,
                        25,
                        25));
            tabMain.Visible = false;

            tabMain.Width = 900;
            tabMain.Height = 560;

            tabMain.Left =
                (ClientSize.Width - tabMain.Width) / 2;

            tabMain.Top =
                pnlSearchBg.Bottom + 30;

            picChart.Left = 20;
            picChart.Top = 170;

            picChart.Width = tabChart.Width - 40;
            picChart.Height = 300;

            lstStocks.BringToFront();
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

                lstStocks.Items.Clear();
                lstStocks.Visible = false;
                lstStocks.SendToBack();

                lblTitle.Top = 50;
                pnlSearchBg.Top = 110;
                tabMain.Top =
     pnlSearchBg.Bottom + 30;

                tabMain.Visible = true;

                lstStocks.Left = pnlSearchBg.Left;
                lstStocks.Top = pnlSearchBg.Bottom + 2;

                string stockName =
                    txtStockSearch.Text.Trim();

                if (!string.IsNullOrEmpty(stockName))
                {
                    _isSearching = true;

                    var selectedStock =
                        _stocks.FirstOrDefault(
                            x => x.StockName.Equals(
                                stockName,
                                StringComparison.OrdinalIgnoreCase));

                    if (selectedStock == null)
                    {
                        MessageBox.Show(
                            "정확한 종목명을 입력하거나\n자동완성 목록에서 선택해주세요.",
                            "종목명 확인");

                        _isSearching = false;
                        return;
                    }

                    _selectedStockCode =
                        selectedStock.StockCode;

                    _selectedMarketType =
                        selectedStock.MarketType;

                    await SearchNaverNews(stockName);

                    _stockPrice =
                        await _kisProvider
                            .GetCurrentPriceAsync(_selectedStockCode);

                    await LoadChartAsync(_selectedStockCode);

                    if (_stockPrice != null)
                    {
                        int currentPrice =
                            int.Parse(_stockPrice.CurrentPrice);

                        int volume =
                            int.Parse(_stockPrice.Volume);

                        int high52Week =
                            int.Parse(_stockPrice.High52Week);

                        lblCurrentPrice.Text =
                            $"현재가 : {currentPrice:N0}원";

                        lblChangeRate.Text =
                            $"등락률 : {_stockPrice.ChangeRate}%";

                        lblVolume.Text =
                            $"거래량 : {volume:N0}";

                        lbl52WeekHigh.Text =
                            $"52주 최고가 : {high52Week:N0}원";
                    }


                    txtStockSearch.SelectionStart =
                        txtStockSearch.Text.Length;

                    lstStocks.Items.Clear();
                    lstStocks.Visible = false;
                    lstStocks.SendToBack();

                    _isSearching = false;
                }
            }
        }

        private async void txtStockSearch_TextChanged(object? sender, EventArgs e)
        {

            if (_isSearching)
                return;

            string keyword =
        txtStockSearch.Text.Trim();

            if (keyword.Length < 2)
            {
                lstStocks.Visible = false;
                return;
            }

            try
            {
                _stocks =
     await _supabaseProvider.SearchStocksAsync(keyword);

                if (_isSearching)
                    return;

                lstStocks.Items.Clear();

                foreach (var stock in _stocks)
                {
                    lstStocks.Items.Add(stock);
                }

                if (!_isSearching)
                {
                    lstStocks.Visible =
                        _stocks.Count > 0;
                }

                lstStocks.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "검색 오류");
            }
        }

        private async void lstStocks_SelectedIndexChanged(
    object? sender,
    EventArgs e)
        {
            lblTitle.Top = 50;

            pnlSearchBg.Top = 110;

            tabMain.Top =
                pnlSearchBg.Bottom + 30;

            tabMain.Visible = true;

            if (lstStocks.SelectedItem == null)
                return;

            try
            {
                _isSearching = true;

                StockInfo selectedStock =
                    (StockInfo)lstStocks.SelectedItem;

                string stockName =
                    selectedStock.StockName;

                txtStockSearch.Text = stockName;

                string stockCode =
                    selectedStock.StockCode;

                await SearchNaverNews(stockName);

                _stockPrice =
                    await _kisProvider
                        .GetCurrentPriceAsync(stockCode);

                await LoadChartAsync(stockCode);

                if (_stockPrice != null)
                {
                    int currentPrice =
                        int.Parse(_stockPrice.CurrentPrice);

                    int volume =
                        int.Parse(_stockPrice.Volume);

                    int high52Week =
                        int.Parse(_stockPrice.High52Week);

                    lblCurrentPrice.Text =
                        $"현재가 : {currentPrice:N0}원";

                    lblChangeRate.Text =
                        $"등락률 : {_stockPrice.ChangeRate}%";

                    lblVolume.Text =
                        $"거래량 : {volume:N0}";

                    lbl52WeekHigh.Text =
                        $"52주 최고가 : {high52Week:N0}원";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _isSearching = false;
            }
        }

        private async Task LoadChartAsync(string stockCode)
        {
            try
            {
                string url =
                    $"https://ssl.pstatic.net/imgfinance/chart/item/area/day/{stockCode}.png";

                using HttpClient client = new();

                byte[] bytes =
                    await client.GetByteArrayAsync(url);

                using MemoryStream ms =
                    new MemoryStream(bytes);

                picChart.Image =
                    Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
