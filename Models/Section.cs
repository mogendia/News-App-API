namespace NewsApp.Models
{
    public class Section : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<News> NewsList { get; set; } = new List<News>();
    }
}
