using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Enums;
using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ConnectBuySellToday.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IAdRepository _adRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReportService(
        IReportRepository reportRepository, 
        IAdRepository adRepository,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _reportRepository = reportRepository;
        _adRepository = adRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<bool> ReportAdAsync(string reporterId, CreateReportDto createReportDto)
    {
        // Check if user has already reported this ad
        var alreadyReported = await _reportRepository.HasUserReportedAdAsync(reporterId, createReportDto.AdId);
        if (alreadyReported)
        {
            return false;
        }

        // Check if ad exists
        var ad = await _adRepository.GetByIdAsync(createReportDto.AdId);
        if (ad == null)
        {
            return false;
        }

        // Prevent user from reporting their own ad
        if (ad.SellerId == reporterId)
        {
            return false;
        }

        var report = new ReportAd
        {
            AdId = createReportDto.AdId,
            ReporterId = reporterId,
            Reason = createReportDto.Reason,
            Description = createReportDto.Description,
            Status = ReportStatus.Pending
        };

        await _reportRepository.AddAsync(report);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    public async Task<IEnumerable<ReportAdDto>> GetReportsByAdIdAsync(Guid adId)
    {
        var reports = await _reportRepository.GetByAdIdAsync(adId);
        return await MapReportsToDto(reports);
    }

    public async Task<IEnumerable<ReportAdDto>> GetAllReportsAsync(ReportStatus? status = null)
    {
        IEnumerable<ReportAd> reports;
        
        if (status.HasValue)
        {
            reports = await _reportRepository.GetByStatusAsync(status.Value);
        }
        else
        {
            reports = await _reportRepository.GetAllAsync();
        }
        
        return await MapReportsToDto(reports);
    }

    public async Task<IEnumerable<ReportAdDto>> GetPendingReportsAsync()
    {
        var reports = await _reportRepository.GetPendingReportsAsync();
        return await MapReportsToDto(reports);
    }

    public async Task<bool> ResolveReportAsync(Guid reportId, string resolvedBy, ResolveReportDto resolveReportDto)
    {
        var report = await _reportRepository.GetByIdAsync(reportId);
        if (report == null)
        {
            return false;
        }

        report.Status = resolveReportDto.Status;
        report.AdminNotes = resolveReportDto.AdminNotes;
        report.ResolvedBy = resolvedBy;
        report.ResolvedAt = DateTime.UtcNow;

        _reportRepository.Update(report);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    public async Task<ReportAdDto?> GetReportByIdAsync(Guid reportId)
    {
        var report = await _reportRepository.GetByIdAsync(reportId);
        if (report == null)
        {
            return null;
        }

        var dtos = await MapReportsToDto(new[] { report });
        return dtos.FirstOrDefault();
    }

    private async Task<IEnumerable<ReportAdDto>> MapReportsToDto(IEnumerable<ReportAd> reports)
    {
        var reportList = reports.ToList();
        var reporterIds = reportList.Select(r => r.ReporterId).Distinct().ToList();
        var adIds = reportList.Select(r => r.AdId).Distinct().ToList();

        var reporters = new Dictionary<string, ApplicationUser>();
        foreach (var reporterId in reporterIds)
        {
            var user = await _userManager.FindByIdAsync(reporterId);
            if (user != null)
            {
                reporters[reporterId] = user;
            }
        }

        var ads = new Dictionary<Guid, ProductAd>();
        foreach (var adId in adIds)
        {
            var ad = await _adRepository.GetByIdAsync(adId);
            if (ad != null)
            {
                ads[adId] = ad;
            }
        }

        return reportList.Select(r => new ReportAdDto
        {
            Id = r.Id,
            AdId = r.AdId,
            AdTitle = ads.TryGetValue(r.AdId, out var ad) ? ad.Title : "Unknown",
            ReporterId = r.ReporterId,
            ReporterName = reporters.TryGetValue(r.ReporterId, out var reporter) 
                ? $"{reporter.FirstName} {reporter.LastName}" 
                : "Unknown",
            Reason = r.Reason,
            Description = r.Description,
            Status = r.Status,
            AdminNotes = r.AdminNotes,
            CreatedAt = r.CreatedAt,
            ResolvedAt = r.ResolvedAt
        });
    }
}
