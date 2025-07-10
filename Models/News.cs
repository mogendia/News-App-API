

namespace NewsApp.Models
{
    public class News : BaseEntity
    {
        
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsImportant { get; set; }
        public bool IsHomePage { get; set; }
        public DateTime CreatedAt { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;
    }
}
