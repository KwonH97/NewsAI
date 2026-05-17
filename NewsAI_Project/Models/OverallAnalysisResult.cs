namespace NewsAI_Project.Models
{
    public class OverallAnalysisResult
    {
        public string Summary { get; set; } = "";
        public string Decision { get; set; } = "";
        public double TotalImpactScore { get; set; }
        public double AverageReliabilityScore { get; set; }
        public List<ScoredArticleResult> Articles { get; set; } = new List<ScoredArticleResult>();
    }
}
