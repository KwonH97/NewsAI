using System.Reflection.Metadata;

namespace NewsAI_Project.Models
{
    public class ArticleAnalysisResult
    {
        // 기사 번호
        public int ArticleIndex { get; set; }

        // AI 요약
        public string Summary { get; set; } = "";

        // 기사 내 주요 주장
        public List<string> Claims { get; set; } = new List<string>();

        // 핵심 키워드
        public List<string> ClaimKeywords { get; set; } = new List<string>();

        // 주가 방향성
        // positive / negative / neutral
        public string ImpactDirection { get; set; } = "neutral";

        // 영향 강도
        // weak / medium / strong
        public string ImpactStrength { get; set; } = "weak";

        // 공식성 수준
        // official / media / rumor / speculation
        public string OfficialityLevel { get; set; } = "speculation";

        // 구체성 수준
        // low / medium / high
        public string SpecificityLevel { get; set; } = "low";

        // 과장 표현 여부
        public bool HasHypeExpression { get; set; }

        // 제목과 본문 불일치 여부
        public bool TitleContentMismatch { get; set; }

        // 시장 반응 속도
        // Low / Medium / High / Critical
        public MarketReactionSpeed MarketReactionSpeed { get; set; }
            = MarketReactionSpeed.Low;

        // 영향 범위
        // Company / Sector / Market
        public MarketImpactRange MarketImpactRange { get; set; }
              = MarketImpactRange.Company;

        // 신뢰도 점수
        public int ReliabilityScore { get; set; }

        // 예상 주가 영향도 점수
        public int PriceImpactScore { get; set; }

        // 분석 근거
        public string Reason { get; set; } = "";
    }
}