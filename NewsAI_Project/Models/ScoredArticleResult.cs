namespace NewsAI_Project.Models
{
    public class ScoredArticleResult
    {
        public NewsItem NewsItem { get; set; } = new NewsItem();
        public ArticleAnalysisResult Analysis { get; set; } = new ArticleAnalysisResult();
        public int SourceScore { get; set; }
        public int OfficialityScore { get; set; }
        public int SpecificityScore { get; set; }
        public int DuplicateScore { get; set; }
        public int PenaltyScore { get; set; }
        public int ReliabilityScore { get; set; }
        public double ImpactScore { get; set; }
    }
}
