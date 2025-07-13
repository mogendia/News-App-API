
namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly NewsDbContext _context;
        private readonly IMediaService _mediaService;

        public AdsController(NewsDbContext context, IMediaService mediaService)
        {
            _context = context;
            _mediaService = mediaService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAd([FromForm] CreateAdDto dto)
        {
            var ext = Path.GetExtension(dto.Media.FileName).ToLowerInvariant();
            var mediaType = ext switch
            {
                ".mp4" or ".webm" or ".ogg" => "video",
                ".jpg" or ".jpeg" or ".png" or ".gif" => "image",
                _ => "other"
            };

            var mediaUrl = await _mediaService.SaveMediaAsync(dto.Media);

            var ad = new Ad
            {
                Title = dto.Title,
                Description = dto.Description,
                MediaUrl = mediaUrl,
                MediaType = mediaType,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAd), new { id = ad.Id }, new { ad.Id });
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAd(int id, [FromForm] CreateAdDto dto)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
                return NotFound(new { message = "الإعلان غير موجود" });

            ad.Title = dto.Title;
            ad.Description = dto.Description;

            if (dto.Media != null)
            {
                var ext = Path.GetExtension(dto.Media.FileName).ToLowerInvariant();
                var mediaType = ext switch
                {
                    ".mp4" or ".webm" or ".ogg" => "video",
                    ".jpg" or ".jpeg" or ".png" or ".gif" => "image",
                    _ => "other"
                };

                var mediaUrl = await _mediaService.SaveMediaAsync(dto.Media);
                ad.MediaUrl = mediaUrl;
                ad.MediaType = mediaType;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تعديل الإعلان بنجاح" });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AdResponseDto>> GetAd(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null) return NotFound();

            return new AdResponseDto
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                MediaUrl = ad.MediaUrl,
                MediaType = ad.MediaType,
                CreatedAt = ad.CreatedAt
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdResponseDto>>> GetAds()
        {
            var ads = await _context.Ads
                .OrderByDescending(a => a.CreatedAt)
                .Select(ad => new AdResponseDto
                {
                    Id = ad.Id,
                    Title = ad.Title,
                    Description = ad.Description,
                    MediaUrl = ad.MediaUrl,
                    MediaType = ad.MediaType,
                    CreatedAt = ad.CreatedAt
                }).ToListAsync();

            return Ok(ads);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAd(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
