namespace NewsApp.DTOs
{
    public class AdResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string MediaUrl { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
