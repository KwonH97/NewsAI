namespace NewsAI_Project
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using (var loginForm = new ApiKeyInputForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // 입력받은 API 키를 실행 중 메모리에만 저장합니다.
                    Config.GeminiKey = loginForm.GeminiKey;
                    Config.NaverId = loginForm.NaverId;
                    Config.NaverSecret = loginForm.NaverSecret;
                    Config.DartKey = loginForm.DartKey;
                    Config.KisAppKey = loginForm.KisAppKey;
                    Config.KisAppSecret = loginForm.KisAppSecret;

                    Application.Run(new Form1());
                }
                else
                {
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
        public static string? DartKey { get; set; }

        public static string SupabaseUrl =
        "https://swwcetsfagpjlagjttyw.supabase.co";

        public static string SupabaseKey =
            "sb_publishable_jarYi82c3uYc3yAFMJ2ahg_NGCDu17H";
        public static string? KisAppKey { get; set; }
        public static string? KisAppSecret { get; set; }
    }
}
