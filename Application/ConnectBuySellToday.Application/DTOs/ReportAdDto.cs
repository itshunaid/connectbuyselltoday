using ConnectBuySellToday.Domain.Enums;

namespace ConnectBuySellToday.Application.DTOs;

public class ReportAdDto
{
    public Guid Id { get; set; }
    public Guid AdId { get; set; }
    public string? AdTitle { get; set; }
    public string ReporterId { get; set; } = string.Empty;
    public string? ReporterName { get; set; }
    public ReportReason Reason { get; set; }
    public string Description { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class CreateReportDto
{
    public Guid AdId { get; set; }
    public ReportReason Reason { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ResolveReportDto
{
    public string? AdminNotes { get; set; }
    public ReportStatus Status { get; set; }
}
