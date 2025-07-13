
namespace NewsApp.Services
{
    public interface IMediaService
    {
        Task<string> SaveMediaAsync(IFormFile file);
    }
}
