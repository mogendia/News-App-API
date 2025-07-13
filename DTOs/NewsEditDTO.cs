namespace NewsApp.DTOs
{
    public class NewsEditDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int SectionId { get; set; }
        public bool IsImportant { get; set; }
        public bool IsHomePage { get; set; }
    }
}
