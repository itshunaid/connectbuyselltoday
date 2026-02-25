namespace ConnectBuySellToday.Application.DTOs;

public class ConversationDto
{
    public string OtherUserId { get; set; } = string.Empty;
    public string OtherUserName { get; set; } = string.Empty;
    public Guid AdId { get; set; }
    public string AdTitle { get; set; } = string.Empty;
    public string? AdImageUrl { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
}
