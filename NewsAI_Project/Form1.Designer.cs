namespace NewsAI_Project
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlSearchBg = new Panel();
            txtStockSearch = new TextBox();
            lblTitle = new Label();
            lstStocks = new ListBox();
            tabMain = new TabControl();
            tabTrust = new TabPage();
            flowTrust = new FlowLayoutPanel();
            tabImpact = new TabPage();
            flowImpact = new FlowLayoutPanel();
            tabChart = new TabPage();
            formsPlotChart = new ScottPlot.WinForms.FormsPlot();
            lblChangeRate = new Label();
            lbl52WeekHigh = new Label();
            lblVolume = new Label();
            lblCurrentPrice = new Label();
            pnlSearchBg.SuspendLayout();
            tabMain.SuspendLayout();
            tabTrust.SuspendLayout();
            tabImpact.SuspendLayout();
            tabChart.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSearchBg
            // 
            pnlSearchBg.Anchor = AnchorStyles.None;
            pnlSearchBg.BackColor = Color.White;
            pnlSearchBg.Controls.Add(txtStockSearch);
            pnlSearchBg.Location = new Point(109, 68);
            pnlSearchBg.Name = "pnlSearchBg";
            pnlSearchBg.Padding = new Padding(10, 5, 10, 5);
            pnlSearchBg.Size = new Size(600, 50);
            pnlSearchBg.TabIndex = 1;
            // 
            // txtStockSearch
            // 
            txtStockSearch.Anchor = AnchorStyles.None;
            txtStockSearch.BorderStyle = BorderStyle.None;
            txtStockSearch.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            txtStockSearch.Location = new Point(10, 5);
            txtStockSearch.Multiline = true;
            txtStockSearch.Name = "txtStockSearch";
            txtStockSearch.PlaceholderText = "종목명을 입력하세요";
            txtStockSearch.Size = new Size(580, 40);
            txtStockSearch.TabIndex = 0;
            txtStockSearch.TextChanged += txtStockSearch_TextChanged;
            txtStockSearch.KeyDown += txtStockSearch_KeyDown;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.None;
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("맑은 고딕", 18F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblTitle.ForeColor = Color.Black;
            lblTitle.Location = new Point(262, 9);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(257, 32);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "오늘의 궁금한 종목은?";
            // 
            // lstStocks
            // 
            lstStocks.FormattingEnabled = true;
            lstStocks.ItemHeight = 15;
            lstStocks.Location = new Point(102, 144);
            lstStocks.Name = "lstStocks";
            lstStocks.Size = new Size(614, 124);
            lstStocks.TabIndex = 4;
            lstStocks.SelectedIndexChanged += lstStocks_SelectedIndexChanged;
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabTrust);
            tabMain.Controls.Add(tabImpact);
            tabMain.Controls.Add(tabChart);
            tabMain.ItemSize = new Size(100, 35);
            tabMain.Location = new Point(102, 274);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(602, 308);
            tabMain.SizeMode = TabSizeMode.Fixed;
            tabMain.TabIndex = 5;
            // 
            // tabTrust
            // 
            tabTrust.Controls.Add(flowTrust);
            tabTrust.Location = new Point(4, 39);
            tabTrust.Name = "tabTrust";
            tabTrust.Padding = new Padding(3);
            tabTrust.Size = new Size(594, 265);
            tabTrust.TabIndex = 0;
            tabTrust.Text = "신뢰도";
            tabTrust.UseVisualStyleBackColor = true;
            // 
            // flowTrust
            // 
            flowTrust.AutoScroll = true;
            flowTrust.Dock = DockStyle.Fill;
            flowTrust.Location = new Point(3, 3);
            flowTrust.Name = "flowTrust";
            flowTrust.Size = new Size(588, 259);
            flowTrust.TabIndex = 0;
            // 
            // tabImpact
            // 
            tabImpact.Controls.Add(flowImpact);
            tabImpact.Location = new Point(4, 39);
            tabImpact.Name = "tabImpact";
            tabImpact.Padding = new Padding(3);
            tabImpact.Size = new Size(594, 265);
            tabImpact.TabIndex = 1;
            tabImpact.Text = "중요도";
            tabImpact.UseVisualStyleBackColor = true;
            // 
            // flowImpact
            // 
            flowImpact.AutoScroll = true;
            flowImpact.Dock = DockStyle.Fill;
            flowImpact.Location = new Point(3, 3);
            flowImpact.Name = "flowImpact";
            flowImpact.Size = new Size(588, 259);
            flowImpact.TabIndex = 0;
            // 
            // tabChart
            // 
            tabChart.Controls.Add(formsPlotChart);
            tabChart.Controls.Add(lblChangeRate);
            tabChart.Controls.Add(lbl52WeekHigh);
            tabChart.Controls.Add(lblVolume);
            tabChart.Controls.Add(lblCurrentPrice);
            tabChart.Location = new Point(4, 39);
            tabChart.Name = "tabChart";
            tabChart.Padding = new Padding(3);
            tabChart.Size = new Size(594, 265);
            tabChart.TabIndex = 2;
            tabChart.Text = "차트";
            tabChart.UseVisualStyleBackColor = true;
            // 
            // formsPlotChart
            // 
            formsPlotChart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlotChart.Location = new Point(-1, 143);
            formsPlotChart.Name = "formsPlotChart";
            formsPlotChart.Size = new Size(594, 122);
            formsPlotChart.TabIndex = 5;
            // 
            // lblChangeRate
            // 
            lblChangeRate.AutoSize = true;
            lblChangeRate.Font = new Font("맑은 고딕", 18F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblChangeRate.Location = new Point(18, 44);
            lblChangeRate.Name = "lblChangeRate";
            lblChangeRate.Size = new Size(78, 32);
            lblChangeRate.TabIndex = 4;
            lblChangeRate.Text = "label5";
            // 
            // lbl52WeekHigh
            // 
            lbl52WeekHigh.AutoSize = true;
            lbl52WeekHigh.Font = new Font("맑은 고딕", 18F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lbl52WeekHigh.Location = new Point(18, 108);
            lbl52WeekHigh.Name = "lbl52WeekHigh";
            lbl52WeekHigh.Size = new Size(78, 32);
            lbl52WeekHigh.TabIndex = 3;
            lbl52WeekHigh.Text = "label4";
            // 
            // lblVolume
            // 
            lblVolume.AutoSize = true;
            lblVolume.Font = new Font("맑은 고딕", 18F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblVolume.Location = new Point(18, 76);
            lblVolume.Name = "lblVolume";
            lblVolume.Size = new Size(78, 32);
            lblVolume.TabIndex = 2;
            lblVolume.Text = "label3";
            // 
            // lblCurrentPrice
            // 
            lblCurrentPrice.AutoSize = true;
            lblCurrentPrice.Font = new Font("맑은 고딕", 18F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblCurrentPrice.Location = new Point(18, 12);
            lblCurrentPrice.Name = "lblCurrentPrice";
            lblCurrentPrice.Size = new Size(78, 32);
            lblCurrentPrice.TabIndex = 0;
            lblCurrentPrice.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1480, 810);
            Controls.Add(tabMain);
            Controls.Add(lstStocks);
            Controls.Add(lblTitle);
            Controls.Add(pnlSearchBg);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += Form1_Load;
            pnlSearchBg.ResumeLayout(false);
            pnlSearchBg.PerformLayout();
            tabMain.ResumeLayout(false);
            tabTrust.ResumeLayout(false);
            tabImpact.ResumeLayout(false);
            tabChart.ResumeLayout(false);
            tabChart.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel pnlSearchBg;
        private TextBox txtStockSearch;
        private Label lblTitle;
        private ListBox lstStocks;
        private TabControl tabMain;
        private TabPage tabTrust;
        private TabPage tabImpact;
        private TabPage tabChart;
        private FlowLayoutPanel flowTrust;
        private FlowLayoutPanel flowImpact;
        private Label lblChangeRate;
        private Label lbl52WeekHigh;
        private Label lblVolume;
        private Label lblCurrentPrice;
        private ScottPlot.WinForms.FormsPlot formsPlotChart;
    }
}
