using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Enums;
using ConnectBuySellToday.Domain.Interfaces;
using ConnectBuySellToday.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Infrastructure.Repositories;

public class ReportRepository : GenericRepository<ReportAd>, IReportRepository
{
    private readonly new ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReportAd>> GetAllAsync()
    {
        return await _context.ReportAds
            .Include(r => r.Ad)
            .Include(r => r.Reporter)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReportAd>> GetByAdIdAsync(Guid adId)
    {
        return await _context.ReportAds
            .Where(r => r.AdId == adId)
            .Include(r => r.Reporter)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReportAd>> GetByStatusAsync(ReportStatus status)
    {
        return await _context.ReportAds
            .Where(r => r.Status == status)
            .Include(r => r.Ad)
            .Include(r => r.Reporter)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReportAd>> GetPendingReportsAsync()
    {
        return await _context.ReportAds
            .Where(r => r.Status == ReportStatus.Pending)
            .Include(r => r.Ad)
            .Include(r => r.Reporter)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasUserReportedAdAsync(string userId, Guid adId)
    {
        return await _context.ReportAds
            .AnyAsync(r => r.ReporterId == userId && r.AdId == adId);
    }
}
