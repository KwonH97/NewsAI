namespace NewsAI_Project.Models
{
    public class ArticleAnalysisResult
    {
        public int ArticleIndex { get; set; }
        public string Summary { get; set; } = "";
        public List<string> Claims { get; set; } = new List<string>();
        public List<string> ClaimKeywords { get; set; } = new List<string>();
        public string ImpactDirection { get; set; } = "neutral";
        public string ImpactStrength { get; set; } = "weak";
        public string OfficialityLevel { get; set; } = "speculation";
        public string SpecificityLevel { get; set; } = "low";
        public bool HasHypeExpression { get; set; }
        public bool TitleContentMismatch { get; set; }
        public string Reason { get; set; } = "";
    }
}
