namespace NewsApp.Services
{
    public class BreakingNewsJob
    {
        private readonly NewsDbContext _context;

        public BreakingNewsJob(NewsDbContext context)
        {
            _context = context;
        }

        public async Task UnmarkExpiredBreakingNews()
        {
            var now = DateTime.UtcNow;

            var expiredNews = await _context.News
                .Where(n => n.IsImportant && n.CreatedAt <= now.AddHours(-2))
                .ToListAsync();

            foreach (var news in expiredNews)
            {
                news.IsImportant = false;
            }

            if (expiredNews.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}

