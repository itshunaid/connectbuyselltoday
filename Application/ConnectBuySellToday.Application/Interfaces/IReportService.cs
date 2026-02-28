using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Domain.Enums;

namespace ConnectBuySellToday.Application.Interfaces;

public interface IReportService
{
    Task<bool> ReportAdAsync(string reporterId, CreateReportDto createReportDto);
    Task<IEnumerable<ReportAdDto>> GetReportsByAdIdAsync(Guid adId);
    Task<IEnumerable<ReportAdDto>> GetAllReportsAsync(ReportStatus? status = null);
    Task<IEnumerable<ReportAdDto>> GetPendingReportsAsync();
    Task<bool> ResolveReportAsync(Guid reportId, string resolvedBy, ResolveReportDto resolveReportDto);
    Task<ReportAdDto?> GetReportByIdAsync(Guid reportId);
}
