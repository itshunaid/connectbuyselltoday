using Microsoft.AspNetCore.Http;

namespace ConnectBuySellToday.Application.Interfaces;

public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<IEnumerable<string>> UploadImagesAsync(IEnumerable<IFormFile> files, string folder);
    Task DeleteImageAsync(string imageUrl);
    Task<bool> DeleteImagesAsync(IEnumerable<string> imageUrls);
}
