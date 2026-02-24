using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace ConnectBuySellToday.Infrastructure.Services;

public class LocalFileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/ads");
        if (!Directory.Exists(uploadsFolder)) 
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        return "/uploads/ads/" + uniqueFileName;
    }

    public void DeleteFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            // Convert URL to file path
            var relativePath = filePath.TrimStart('/');
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch (Exception)
        {
            // Log error if needed, but don't throw to prevent breaking the application
        }
    }
}
