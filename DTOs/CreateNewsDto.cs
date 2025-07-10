
namespace NewsApp.DTOs
{
    public class CreateNewsDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
       
        public bool IsImportant { get; set; }
        public bool IsHomePage { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public List<IFormFile>? MediaFiles { get; set; }
        [Required]
        public int SectionId { get; set; }
    }
}
