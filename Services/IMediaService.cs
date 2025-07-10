using Microsoft.AspNetCore.Http;

namespace NewsApp.Services
{
    public interface IMediaService
    {
        Task<string> SaveMediaAsync(IFormFile file);
    }
}
