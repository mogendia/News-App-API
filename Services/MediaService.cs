using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace NewsApp.Services
{
    public class MediaService : IMediaService
    {
        private readonly IWebHostEnvironment _env;

        public MediaService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveMediaAsync(IFormFile file)
        {

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var folderType = ext switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" => "images",
                ".mp4" or ".webm" or ".ogg" => "videos",
                _ => "misc"
            };

            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads", folderType);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + ext;
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"uploads/{folderType}/{fileName}";
        }
    }
}
