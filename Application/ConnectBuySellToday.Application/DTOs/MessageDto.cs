namespace ConnectBuySellToday.Application.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public Guid ProductAdId { get; set; }
    public DateTime CreatedAt { get; set; }
}
