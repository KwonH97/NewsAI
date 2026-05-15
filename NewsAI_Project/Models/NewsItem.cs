namespace NewsAI_Project.Models
{
    public class NewsItem
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Link { get; set; } = "";
        public string SourceName { get; set; } = "";
        public NewsSourceType SourceType { get; set; }
        public DateTime? PublishedAt { get; set; }

    }
}
