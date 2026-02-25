using ConnectBuySellToday.Application.DTOs;

namespace ConnectBuySellToday.Application.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<MessageDto>> GetConversationAsync(string userId, string otherUserId, Guid adId);
    Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId);
    Task<MessageDto> SendMessageAsync(string senderId, string receiverId, Guid adId, string content);
}
