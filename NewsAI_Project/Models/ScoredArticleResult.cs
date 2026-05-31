namespace NewsAI_Project.Models
{
    public class ScoredArticleResult
    {
        public NewsItem NewsItem { get; set; }
            = new NewsItem();

        public ArticleAnalysisResult Analysis { get; set; }
            = new ArticleAnalysisResult();

        public ReliabilityBreakdown Reliability { get; set; }
            = new ReliabilityBreakdown();

        public double ImpactScore { get; set; }
    }
}
