using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using System.Security.Claims;
using System.Threading.Tasks;

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
                .Where(n => n.Status == "Approved")
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
                    SectionName = n.Section.Name,
                    WrittenBy = n.WrittenBy
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("homepage")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> HomePage()
        {
            var breakingNews = await _context.News
                .Where(n => n.IsHomePage == true && n.Status == "Approved")
                .Include(n => n.Section)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name,
                    WrittenBy = n.WrittenBy

                })
                .ToListAsync();

            return Ok(breakingNews);
        }

        [HttpGet("breaking")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetBreakingNews()
        {
            var breakingNews = await _context.News
                .Where(n => n.IsImportant == true && n.Status == "Approved")
                .Include(n => n.Section)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name,
                    WrittenBy = n.WrittenBy

                })
                .ToListAsync();

            return Ok(breakingNews);
        }

        [HttpGet("section/{sectionId}")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetNewsBySection(int sectionId)
        {
            var news = await _context.News
                .Where(x => x.SectionId == sectionId && x.Status == "Approved")
                .Include(n => n.Section)
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name,
                    WrittenBy = n.WrittenBy

                })
                .ToListAsync();
            return Ok(news);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> SearchNews([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required");

            query = query.ToLower();

            var news = await _context.News
                .Where(n => (n.Title.ToLower().Contains(query) || n.Content.ToLower().Contains(query)) && n.Status == "Approved")
                .Include(n => n.Section)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name,
                    WrittenBy = n.WrittenBy

                })
                .ToListAsync();

            return Ok(news);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsResponseDto>> GetNewsById(int id)
        {
            var result = await _context.News
                .Include(n => n.Section)
                .Where(n => n.Id == id && n.Status == "Approved")
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name,
                    WrittenBy = n.WrittenBy

                })
                .SingleOrDefaultAsync();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateNews([FromForm] CreateNewsDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var isSuperAdmin = User.IsInRole("SuperAdmin");

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
                IsImportant = dto.IsImportant,
                WrittenBy = dto.WrittenBy,
                Status = isSuperAdmin ? "Approved" : "Pending" 
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNewsById), new { id = news.Id }, new { news.Id });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromForm] CreateNewsDto dto)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();

            var isSuperAdmin = User.IsInRole("SuperAdmin");

            news.Title = dto.Title;
            news.Content = dto.Content;
            news.SectionId = dto.SectionId;
            news.IsImportant = dto.IsImportant;
            news.IsHomePage = dto.IsHomePage;
            news.WrittenBy = dto.WrittenBy;
            news.Status = isSuperAdmin ? "Approved" : "Pending"; 

            if (dto.Image != null)
            {
                news.ImageUrl = await _mediaService.SaveMediaAsync(dto.Image);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
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

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetPendingNews()
        {
            var pendingNews = await _context.News
                .Where(n => n.Status == "Pending")
                .Include(n => n.Section)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NewsResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    CreatedAt = n.CreatedAt,
                    SectionName = n.Section.Name,
                    Status = n.Status,
                    WrittenBy = n.WrittenBy
                })
                .ToListAsync();

            return Ok(pendingNews);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();

            news.Status = "Approved";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "News approved successfully" });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();

            news.Status = "Rejected";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "News rejected successfully" });
        }
    }
}