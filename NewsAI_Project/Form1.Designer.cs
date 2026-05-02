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
            pnlResults = new FlowLayoutPanel();
            pnlSearchBg.SuspendLayout();
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
            // pnlResults
            // 
            pnlResults.AutoScroll = true;
            pnlResults.BackColor = Color.Transparent;
            pnlResults.FlowDirection = FlowDirection.TopDown;
            pnlResults.Location = new Point(109, 183);
            pnlResults.Name = "pnlResults";
            pnlResults.Size = new Size(600, 269);
            pnlResults.TabIndex = 3;
            pnlResults.WrapContents = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1480, 810);
            Controls.Add(pnlResults);
            Controls.Add(lblTitle);
            Controls.Add(pnlSearchBg);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += Form1_Load;
            pnlSearchBg.ResumeLayout(false);
            pnlSearchBg.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel pnlSearchBg;
        private TextBox txtStockSearch;
        private Label lblTitle;
        private FlowLayoutPanel pnlResults;
    }
}
