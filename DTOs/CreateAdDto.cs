namespace NewsApp.DTOs
{
    public class CreateAdDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile Media { get; set; } = null!;
    }
}
