namespace NewsAI_Project
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            using (var loginForm = new ApiKeyInputForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // 입력받은 키를 전역 변수나 정적 클래스에 저장
                    Config.GeminiKey = loginForm.GeminiKey;
                    Config.NaverId = loginForm.NaverId;
                    Config.NaverSecret = loginForm.NaverSecret;

                    // 메인 폼 실행
                    Application.Run(new Form1());
                }
                else
                {
                    // 취소 시 프로그램 종료
                    Application.Exit();
                }
            }

        }
    }
    public static class Config
    {
        public static string? GeminiKey { get; set; }
        public static string? NaverId { get; set; }
        public static string? NaverSecret { get; set; }
    }
}