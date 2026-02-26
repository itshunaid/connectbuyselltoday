using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConnectBuySellToday.Infrastructure.Services;

public class FeaturedAdExpiryService : BackgroundService
{
    private readonly ILogger<FeaturedAdExpiryService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

    public FeaturedAdExpiryService(
        ILogger<FeaturedAdExpiryService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Featured Ad Expiry Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UnfeatureExpiredAdsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unfeaturing expired ads.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Featured Ad Expiry Service is stopping.");
    }

    private async Task UnfeatureExpiredAdsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        
        // Get all featured ads that have expired
        var allAds = await unitOfWork.Ads.GetAllAdsAsync();
        var expiredAds = allAds
            .Where(a => a.IsFeatured && a.FeaturedExpiryDate.HasValue && a.FeaturedExpiryDate.Value <= now)
            .ToList();


        if (expiredAds.Any())
        {
            _logger.LogInformation("Found {Count} expired featured ads to unfeature.", expiredAds.Count);

            foreach (var ad in expiredAds)
            {
                ad.IsFeatured = false;
                ad.FeaturedExpiryDate = null;
                unitOfWork.Ads.Update(ad);
                
                _logger.LogInformation("Unfeatured ad {AdId} - '{Title}' as it expired on {ExpiryDate}.", 
                    ad.Id, ad.Title, ad.FeaturedExpiryDate);
            }

            await unitOfWork.CompleteAsync();
            _logger.LogInformation("Successfully unfeatured {Count} expired ads.", expiredAds.Count);
        }
        else
        {
            _logger.LogDebug("No expired featured ads found.");
        }
    }
}
