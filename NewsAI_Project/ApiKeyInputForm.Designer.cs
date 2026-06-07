namespace NewsAI_Project
{
    partial class ApiKeyInputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtGeminiKey = new TextBox();
            txtNaverId = new TextBox();
            txtNaverSecret = new TextBox();
            Gemini_Key = new Label();
            Naver_Id = new Label();
            Naver_Secret = new Label();
            Btn_Ok = new Button();
            label1 = new Label();
            txtDartKey = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtKisAppKey = new TextBox();
            txtKisAppSecret = new TextBox();
            SuspendLayout();
            // 
            // txtGeminiKey
            // 
            txtGeminiKey.Font = new Font("맑은 고딕", 15F);
            txtGeminiKey.Location = new Point(146, 35);
            txtGeminiKey.Multiline = true;
            txtGeminiKey.Name = "txtGeminiKey";
            txtGeminiKey.Size = new Size(490, 38);
            txtGeminiKey.TabIndex = 0;
            // 
            // txtNaverId
            // 
            txtNaverId.Font = new Font("맑은 고딕", 15F);
            txtNaverId.Location = new Point(146, 95);
            txtNaverId.Multiline = true;
            txtNaverId.Name = "txtNaverId";
            txtNaverId.Size = new Size(490, 37);
            txtNaverId.TabIndex = 1;
            // 
            // txtNaverSecret
            // 
            txtNaverSecret.Font = new Font("맑은 고딕", 15F);
            txtNaverSecret.Location = new Point(146, 165);
            txtNaverSecret.Multiline = true;
            txtNaverSecret.Name = "txtNaverSecret";
            txtNaverSecret.Size = new Size(490, 37);
            txtNaverSecret.TabIndex = 2;
            // 
            // Gemini_Key
            // 
            Gemini_Key.AutoSize = true;
            Gemini_Key.Location = new Point(59, 48);
            Gemini_Key.Name = "Gemini_Key";
            Gemini_Key.Size = new Size(64, 15);
            Gemini_Key.TabIndex = 3;
            Gemini_Key.Text = "GeminiKey";
            // 
            // Naver_Id
            // 
            Naver_Id.AutoSize = true;
            Naver_Id.Location = new Point(75, 107);
            Naver_Id.Name = "Naver_Id";
            Naver_Id.Size = new Size(48, 15);
            Naver_Id.TabIndex = 4;
            Naver_Id.Text = "NaverId";
            // 
            // Naver_Secret
            // 
            Naver_Secret.AutoSize = true;
            Naver_Secret.Location = new Point(52, 178);
            Naver_Secret.Name = "Naver_Secret";
            Naver_Secret.Size = new Size(71, 15);
            Naver_Secret.TabIndex = 5;
            Naver_Secret.Text = "NaverSecret";
            // 
            // Btn_Ok
            // 
            Btn_Ok.Location = new Point(338, 420);
            Btn_Ok.Name = "Btn_Ok";
            Btn_Ok.Size = new Size(107, 36);
            Btn_Ok.TabIndex = 6;
            Btn_Ok.Text = "확인";
            Btn_Ok.UseVisualStyleBackColor = true;
            Btn_Ok.Click += Btn_Ok_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 246);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 7;
            label1.Text = "DART API Key";
            // 
            // txtDartKey
            // 
            txtDartKey.Font = new Font("맑은 고딕", 15F);
            txtDartKey.Location = new Point(146, 233);
            txtDartKey.Multiline = true;
            txtDartKey.Name = "txtDartKey";
            txtDartKey.Size = new Size(490, 37);
            txtDartKey.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 308);
            label2.Name = "label2";
            label2.Size = new Size(100, 15);
            label2.TabIndex = 9;
            label2.Text = "한국투자 API Key";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(9, 365);
            label3.Name = "label3";
            label3.Size = new Size(114, 15);
            label3.TabIndex = 10;
            label3.Text = "한국투자 API Secret";
            // 
            // txtKisAppKey
            // 
            txtKisAppKey.Font = new Font("맑은 고딕", 15F);
            txtKisAppKey.Location = new Point(146, 296);
            txtKisAppKey.Multiline = true;
            txtKisAppKey.Name = "txtKisAppKey";
            txtKisAppKey.Size = new Size(490, 37);
            txtKisAppKey.TabIndex = 11;
            // 
            // txtKisAppSecret
            // 
            txtKisAppSecret.Font = new Font("맑은 고딕", 15F);
            txtKisAppSecret.Location = new Point(146, 354);
            txtKisAppSecret.Multiline = true;
            txtKisAppSecret.Name = "txtKisAppSecret";
            txtKisAppSecret.Size = new Size(490, 37);
            txtKisAppSecret.TabIndex = 12;
            // 
            // ApiKeyInputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 494);
            Controls.Add(txtKisAppSecret);
            Controls.Add(txtKisAppKey);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtDartKey);
            Controls.Add(label1);
            Controls.Add(Btn_Ok);
            Controls.Add(Naver_Secret);
            Controls.Add(Naver_Id);
            Controls.Add(Gemini_Key);
            Controls.Add(txtNaverSecret);
            Controls.Add(txtNaverId);
            Controls.Add(txtGeminiKey);
            Name = "ApiKeyInputForm";
            Text = "ApiKeyInputForm";
            Load += ApiKeyInputForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtGeminiKey;
        private TextBox txtNaverId;
        private TextBox txtNaverSecret;
        private Label Gemini_Key;
        private Label Naver_Id;
        private Label Naver_Secret;
        private Button Btn_Ok;
        private Label label1;
        private TextBox txtDartKey;
        private Label label2;
        private Label label3;
        private TextBox txtKisAppKey;
        private TextBox txtKisAppSecret;
    }
}