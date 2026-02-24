namespace ConnectBuySellToday.Application.DTOs;

public class AdImageDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
