using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewsAI_Project
{
    public partial class ApiKeyInputForm : Form
    {
        public string? GeminiKey { get; private set; }
        public string? NaverId { get; private set; }
        public string? NaverSecret { get; private set; }

        public ApiKeyInputForm()
        {
            InitializeComponent();
        }

        private void ApiKeyInputForm_Load(object sender, EventArgs e)
        {

        }

        // 입력값 검증
        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            // 1. 빈칸 검사 (제미나이 키는 필수)
            if (string.IsNullOrWhiteSpace(txtGeminiKey.Text))
            {
                MessageBox.Show("제미나이 API 키를 입력해야 프로그램이 실행됩니다!", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. 입력받은 값을 클래스 속성에 저장
            // 디자인에서 설정한 각 TextBox의 Name이 txtGeminiKey, txtNaverId, txtNaverSecret여야 합니다.
            this.GeminiKey = txtGeminiKey.Text.Trim();
            this.NaverId = txtNaverId.Text.Trim();
            this.NaverSecret = txtNaverSecret.Text.Trim();

            // 3. 결과 성공(OK)을 알리고 창 닫기
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
