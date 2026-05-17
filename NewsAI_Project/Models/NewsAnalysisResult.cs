namespace NewsAI_Project.Models
{
    public class NewsAnalysisResult
    {
        public string OverallSummary { get; set; } = "";
        public List<ArticleAnalysisResult> Articles { get; set; } = new List<ArticleAnalysisResult>();
    }
}
