namespace NewsApp.DTOs
{
    public class NewsResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SectionName { get; set; } = string.Empty;
    }
}
