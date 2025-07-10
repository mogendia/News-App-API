namespace NewsApp.Models
{
    public class Ad : BaseEntity
    {
    
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string MediaUrl { get; set; } = null!;
        public string MediaType { get; set; } = null!; 
        public DateTime CreatedAt { get; set; }
    }

}
