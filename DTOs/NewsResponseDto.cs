namespace NewsApp.DTOs
{
    public class NewsResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string WrittenBy { get; set; } = string.Empty;
        public string ImageContent { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public int SectionId { get; set; }
        public bool? IsImportant { get; set; }
        public bool? IsHomePage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string? Status { get; set; }
    }
}
