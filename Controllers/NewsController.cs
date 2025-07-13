
namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsDbContext _context;
        private readonly IMediaService _mediaService;

        public NewsController(NewsDbContext context, IMediaService mediaService)
        {
            _context = context;
            _mediaService = mediaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetNews(
                     [FromQuery] string? search = null,
                     [FromQuery] int? section = null)
        {
            var query = _context.News
                .Include(n => n.Section)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(n =>
                    n.Title.Contains(search) ||
                    n.Content.Contains(search));
            }

            if (section.HasValue)
            {
                query = query.Where(n => n.Section.Id == section);
            }

            var result = await query
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(result);
        }
        [HttpGet("breaking")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetBreakingNews()
        {
            var breakingNews = await _context.News
                .Where(n => n.IsImportant == true)
                .Include(n => n.Section)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name
                })
                .ToListAsync();

            return Ok(breakingNews);
        }

        [HttpGet("section/{sectionId}")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetNewsBySection(int sectionId)
        {
            var news = await _context.News.Where(x => x.SectionId == sectionId)
                .Select(n=>new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name
                })
                .ToListAsync();
            return Ok(news);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetNewsBySearch([FromQuery] string query)
        {
            var news = await _context.News.Where(n => n.Title.Contains(query) || n.Content.Contains(query))
               .Select(n => new NewsResponseDto
               {
                   Id = n.Id,
                   Title = n.Title,
                   Content = n.Content,
                   ImageUrl = n.ImageUrl,
                   CreatedAt = n.CreatedAt,
                   SectionName = n.Section.Name
               })
                .ToListAsync();
               
            if (news == null || !news.Any())
            {
                return NotFound("No news found matching your search.");
            }
            return Ok(news);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsResponseDto>> GetNewsById(int id)
        {
            var result = await _context.News
                .Include(n => n.Section)
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name
                })
                .OrderByDescending(x => x.CreatedAt)
                .SingleOrDefaultAsync(i=>i.Id == id);

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateNews([FromForm] CreateNewsDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            string? mainImageUrl = null;
            if (dto.Image != null)
            {
                mainImageUrl = await _mediaService.SaveMediaAsync(dto.Image);
            }

            var news = new News
            {
                Title = dto.Title,
                Content = dto.Content, 
                ImageUrl = mainImageUrl,
                SectionId = dto.SectionId,
                CreatedAt = DateTime.UtcNow,
                OwnerId = userId,
                IsHomePage = dto.IsHomePage,
                IsImportant = dto.IsImportant
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNews), new { id = news.Id }, new { news.Id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromForm] CreateNewsDto dto)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();

            news.Title = dto.Title;
            news.Content = dto.Content;
            news.SectionId = dto.SectionId;
            news.IsImportant = dto.IsImportant;
            news.IsHomePage = dto.IsHomePage;


            if (dto.Image != null)
            {
                news.ImageUrl = await _mediaService.SaveMediaAsync(dto.Image);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
