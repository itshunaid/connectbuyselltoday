using ConnectBuySellToday.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Linq;

namespace ConnectBuySellToday.Infrastructure.Services;

public class ImageService : IImageService
{
    private readonly string _webRootPath;
    private readonly ILogger<ImageService> _logger;

    public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger)
    {
        _webRootPath = environment.WebRootPath ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger;
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));

        // Validate image format
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException($"Invalid file format. Allowed: {string.Join(", ", allowedExtensions)}");

        // Create folder if it doesn't exist
        var folderPath = Path.Combine(_webRootPath, "uploads", folder);
        Directory.CreateDirectory(folderPath);

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(folderPath, fileName);

        try
        {
            // Process and save image
            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                // Resize if too large (max 1200px width or height)
                var maxDimension = 1200;
                if (image.Width > maxDimension || image.Height > maxDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(maxDimension, maxDimension)
                    }));
                }

                await image.SaveAsync(filePath);
            }

            _logger.LogInformation("Image uploaded successfully: {FileName}", fileName);
            
            // Return the relative URL
            return $"/uploads/{folder}/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
            throw;
        }
    }

    public async Task<IEnumerable<string>> UploadImagesAsync(IEnumerable<IFormFile> files, string folder)
    {
        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            try
            {
                var url = await UploadImageAsync(file, folder);
                uploadedUrls.Add(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
                // Continue with other images even if one fails
            }
        }

        return uploadedUrls;
    }

    public Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return Task.CompletedTask;

        try
        {
            // Convert URL to file path
            var relativePath = imageUrl.TrimStart('/');
            var filePath = Path.Combine(_webRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Image deleted successfully: {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
        }

        return Task.CompletedTask;
    }

    public async Task<bool> DeleteImagesAsync(IEnumerable<string> imageUrls)
    {
        foreach (var url in imageUrls)
        {
            await DeleteImageAsync(url);
        }
        return true;
    }
}
