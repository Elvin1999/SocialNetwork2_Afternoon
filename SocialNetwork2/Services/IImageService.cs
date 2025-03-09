namespace SocialNetwork2.Services
{
    public interface IImageService
    {
        Task<string> SaveFile(IFormFile file);
    }
}
