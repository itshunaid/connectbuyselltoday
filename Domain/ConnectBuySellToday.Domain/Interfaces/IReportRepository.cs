using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Enums;

namespace ConnectBuySellToday.Domain.Interfaces;

public interface IReportRepository
{
    Task<ReportAd?> GetByIdAsync(Guid id);
    Task<IEnumerable<ReportAd>> GetAllAsync();
    Task<IEnumerable<ReportAd>> GetByAdIdAsync(Guid adId);
    Task<IEnumerable<ReportAd>> GetByStatusAsync(ReportStatus status);
    Task<IEnumerable<ReportAd>> GetPendingReportsAsync();
    Task AddAsync(ReportAd report);
    void Update(ReportAd report);
    Task<bool> HasUserReportedAdAsync(string userId, Guid adId);
}
