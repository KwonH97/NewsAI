using NewsAI_Project.Models;

namespace NewsAI_Project.Services
{
    public class ReliabilityScorer
    {
        public OverallAnalysisResult Score(List<NewsItem> newsItems, NewsAnalysisResult analysisResult, List<DartDisclosure> disclosures)
        {
            Dictionary<string, int> keywordCounts = CountClaimKeywords(analysisResult.Articles);
            List<ScoredArticleResult> scoredArticles = new List<ScoredArticleResult>();

            for (int i = 0; i < newsItems.Count; i++)
            {
                ArticleAnalysisResult articleAnalysis =
                    FindAnalysisForArticle(
                        analysisResult.Articles,
                        i);

                int sourceScore =
                    GetSourceScore(newsItems[i].SourceType);

                int officialityScore =
                    GetOfficialityScore(
                        articleAnalysis.OfficialityLevel);

                int specificityScore =
                    GetSpecificityScore(
                        articleAnalysis.SpecificityLevel);

                int duplicateScore =
                    GetDuplicateScore(
                        articleAnalysis.ClaimKeywords,
                        keywordCounts);

                int penaltyScore =
                    GetPenaltyScore(articleAnalysis);

                int dartScore =
                    GetDartScore(
                        newsItems[i].Title,
                        disclosures);


                int reliabilityScore =
                    Clamp(
                        sourceScore +
                        officialityScore +
                        specificityScore +
                        duplicateScore +
                        dartScore -
                        penaltyScore,
                        0,
                        100);

                double impactScore =
                    reliabilityScore *
                    GetDirectionMultiplier(
                        articleAnalysis.ImpactDirection) *
                    GetStrengthMultiplier(
                        articleAnalysis.ImpactStrength);

                scoredArticles.Add(new ScoredArticleResult
                {
                    NewsItem = newsItems[i],

                    Analysis = articleAnalysis,

                    Reliability = new ReliabilityBreakdown
                    {
                        SourceScore = sourceScore,

                        OfficialityScore = officialityScore,

                        SpecificityScore = specificityScore,

                        DuplicateScore = duplicateScore,

                        DartScore = dartScore,

                        PenaltyScore = penaltyScore,

                        FinalScore = reliabilityScore
                    },

                    ImpactScore = impactScore
                });
            }

            double totalImpactScore = scoredArticles.Sum(article => article.ImpactScore);
            double averageReliabilityScore = scoredArticles.Count == 0 ? 0 : scoredArticles.Average(article => article.Reliability.FinalScore);

            return new OverallAnalysisResult
            {
                Summary = analysisResult.OverallSummary,
                Decision = GetDecision(totalImpactScore),
                TotalImpactScore = totalImpactScore,
                AverageReliabilityScore = averageReliabilityScore,
                Articles = scoredArticles
            };
        }

        private static Dictionary<string, int> CountClaimKeywords(List<ArticleAnalysisResult> articles)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (ArticleAnalysisResult article in articles)
            {
                foreach (string keyword in article.ClaimKeywords.Select(NormalizeKeyword).Where(keyword => keyword.Length > 0).Distinct())
                {
                    counts[keyword] = counts.GetValueOrDefault(keyword) + 1;
                }
            }

            return counts;
        }

        private static ArticleAnalysisResult FindAnalysisForArticle(List<ArticleAnalysisResult> articles, int articleIndex)
        {
            return articles.FirstOrDefault(article => article.ArticleIndex == articleIndex)
                ?? new ArticleAnalysisResult { ArticleIndex = articleIndex, Summary = "AI 분석 결과가 없습니다." };
        }

        private static int GetSourceScore(NewsSourceType sourceType)
        {
            return sourceType switch
            {
                NewsSourceType.Disclosure => 40,          // DART

                NewsSourceType.FinancialAuthority => 35,  // 금융감독원

                NewsSourceType.EconomicNews => 30,        // 경제지

                NewsSourceType.CompanyOfficial => 25,     // 기업 IR

                NewsSourceType.News => 20,                // 일반 뉴스

                NewsSourceType.Blog => 5,

                NewsSourceType.Community => 5,

                _ => 5
            };
        }

        private static int GetOfficialityScore(string officialityLevel)
        {
            return NormalizeKeyword(officialityLevel) switch
            {
                "official_disclosure" => 25,
                "contract_or_earnings" => 25,
                "company_quote" => 15,
                "industry_forecast" => 5,
                "analyst_forecast" => 5,
                _ => 0
            };
        }

        private static int GetSpecificityScore(string specificityLevel)
        {
            return NormalizeKeyword(specificityLevel) switch
            {
                "high" => 20,
                "medium" => 10,
                "low" => 3,
                _ => 3
            };
        }

        private static int GetDuplicateScore(List<string> claimKeywords, Dictionary<string, int> keywordCounts)
        {
            int maxCount = claimKeywords
                .Select(NormalizeKeyword)
                .Where(keyword => keyword.Length > 0)
                .Select(keyword => keywordCounts.GetValueOrDefault(keyword))
                .DefaultIfEmpty(1)
                .Max();

            if (maxCount >= 3)
            {
                return 15;
            }

            if (maxCount == 2)
            {
                return 8;
            }

            return 0;
        }

        private static int GetPenaltyScore(ArticleAnalysisResult articleAnalysis)
        {
            int penalty = 0;

            if (articleAnalysis.HasHypeExpression)
            {
                penalty += 10;
            }

            if (articleAnalysis.TitleContentMismatch)
            {
                penalty += 10;
            }

            return penalty;
        }

        private static int GetDirectionMultiplier(string impactDirection)
        {
            return NormalizeKeyword(impactDirection) switch
            {
                "positive" => 1,
                "negative" => -1,
                _ => 0
            };
        }

        private static double GetStrengthMultiplier(string impactStrength)
        {
            return NormalizeKeyword(impactStrength) switch
            {
                "strong" => 1.0,
                "medium" => 0.6,
                "weak" => 0.3,
                _ => 0.3
            };
        }

        private static string GetDecision(double totalImpactScore)
        {
            if (totalImpactScore >= 50)
            {
                return "상승 가능성 높음";
            }

            if (totalImpactScore >= 10)
            {
                return "상승 가능성 있음";
            }

            if (totalImpactScore <= -50)
            {
                return "하락 가능성 높음";
            }

            if (totalImpactScore <= -10)
            {
                return "하락 가능성 있음";
            }

            return "판단 보류";
        }



        private static int Clamp(int value, int min, int max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        private static int GetDartScore(
    string newsTitle,
    List<DartDisclosure> disclosures)
        {
            string title =
                newsTitle.ToLower();

            foreach (var disclosure in disclosures)
            {
                string report =
                    disclosure.ReportName.ToLower();
                

                // 자사주
                if (title.Contains("자사주") &&
                    report.Contains("자기주식"))
                    return 20;

                // 실적
                if ((title.Contains("실적") ||
                     title.Contains("영업이익") ||
                     title.Contains("매출")) &&
                    (report.Contains("사업보고서") ||
                     report.Contains("반기보고서") ||
                     report.Contains("분기보고서")))
                    return 20;

                // 공급계약
                if (title.Contains("계약") &&
                    report.Contains("공급계약"))
                    return 20;

                // 투자
                if (title.Contains("투자") &&
                    report.Contains("투자"))
                    return 20;

                // 증자
                if (title.Contains("증자") &&
                    report.Contains("유상증자"))
                    return 20;
            }

            return 0;
        }

        private static string NormalizeKeyword(string? value)
        {
            return (value ?? "").Trim().ToLowerInvariant().Replace(" ", "_");
        }
    }
}
