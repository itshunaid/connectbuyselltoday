using ConnectBuySellToday.Domain.Common;
using ConnectBuySellToday.Domain.Enums;

namespace ConnectBuySellToday.Domain.Entities;

public class ReportAd : BaseEntity
{
    public Guid AdId { get; set; }
    public ProductAd Ad { get; set; } = null!;

    public string ReporterId { get; set; } = string.Empty;
    public ApplicationUser Reporter { get; set; } = null!;

    public ReportReason Reason { get; set; }
    public string Description { get; set; } = string.Empty;

    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public string? AdminNotes { get; set; }

    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
