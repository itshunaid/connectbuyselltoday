using ConnectBuySellToday.Domain.Entities;

namespace ConnectBuySellToday.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAdRepository Ads { get; }
    ICategoryRepository Categories { get; }
    IAdImageRepository AdsImage { get; }
    IMessageRepository Messages { get; }
    IUserRepository Users { get; }
    IFavoriteRepository Favorites { get; }
    IReportRepository Reports { get; }

    Task<int> CompleteAsync();
}
