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
            SuspendLayout();
            // 
            // txtGeminiKey
            // 
            txtGeminiKey.Font = new Font("맑은 고딕", 20F);
            txtGeminiKey.Location = new Point(215, 78);
            txtGeminiKey.Multiline = true;
            txtGeminiKey.Name = "txtGeminiKey";
            txtGeminiKey.Size = new Size(490, 50);
            txtGeminiKey.TabIndex = 0;
            // 
            // txtNaverId
            // 
            txtNaverId.Font = new Font("맑은 고딕", 20F);
            txtNaverId.Location = new Point(215, 145);
            txtNaverId.Multiline = true;
            txtNaverId.Name = "txtNaverId";
            txtNaverId.Size = new Size(490, 50);
            txtNaverId.TabIndex = 1;
            // 
            // txtNaverSecret
            // 
            txtNaverSecret.Font = new Font("맑은 고딕", 20F);
            txtNaverSecret.Location = new Point(215, 212);
            txtNaverSecret.Multiline = true;
            txtNaverSecret.Name = "txtNaverSecret";
            txtNaverSecret.Size = new Size(490, 50);
            txtNaverSecret.TabIndex = 2;
            // 
            // Gemini_Key
            // 
            Gemini_Key.AutoSize = true;
            Gemini_Key.Location = new Point(59, 81);
            Gemini_Key.Name = "Gemini_Key";
            Gemini_Key.Size = new Size(64, 15);
            Gemini_Key.TabIndex = 3;
            Gemini_Key.Text = "GeminiKey";
            // 
            // Naver_Id
            // 
            Naver_Id.AutoSize = true;
            Naver_Id.Location = new Point(59, 156);
            Naver_Id.Name = "Naver_Id";
            Naver_Id.Size = new Size(48, 15);
            Naver_Id.TabIndex = 4;
            Naver_Id.Text = "NaverId";
            // 
            // Naver_Secret
            // 
            Naver_Secret.AutoSize = true;
            Naver_Secret.Location = new Point(59, 231);
            Naver_Secret.Name = "Naver_Secret";
            Naver_Secret.Size = new Size(71, 15);
            Naver_Secret.TabIndex = 5;
            Naver_Secret.Text = "NaverSecret";
            // 
            // Btn_Ok
            // 
            Btn_Ok.Location = new Point(347, 326);
            Btn_Ok.Name = "Btn_Ok";
            Btn_Ok.Size = new Size(107, 36);
            Btn_Ok.TabIndex = 6;
            Btn_Ok.Text = "확인";
            Btn_Ok.UseVisualStyleBackColor = true;
            Btn_Ok.Click += Btn_Ok_Click;
            // 
            // ApiKeyInputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
    }
}