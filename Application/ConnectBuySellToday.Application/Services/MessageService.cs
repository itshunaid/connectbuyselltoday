using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;

namespace ConnectBuySellToday.Application.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MessageDto>> GetConversationAsync(string userId, string otherUserId, Guid adId)
    {
        var messages = await _unitOfWork.Messages.GetConversationAsync(userId, otherUserId, adId);
        
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Content = m.Content,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            ProductAdId = m.ProductAdId,
            CreatedAt = m.CreatedAt
        });
    }

    public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId)
    {
        var allMessages = await _unitOfWork.Messages.GetUserConversationsAsync(userId);
        
        // Group messages by conversation (other user + ad)
        var conversations = allMessages
            .GroupBy(m => new { 
                OtherUserId = m.SenderId == userId ? m.ReceiverId : m.SenderId,
                AdId = m.ProductAdId 
            })
            .Select(g =>
            {
                var otherUserId = g.Key.OtherUserId;
                var adId = g.Key.AdId;
                
                var lastMessage = g.OrderByDescending(m => m.CreatedAt).First();
                var unreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead);
                
                return new ConversationDto
                {
                    OtherUserId = otherUserId,
                    OtherUserName = "User", // Would need to fetch from UserManager
                    AdId = adId,
                    AdTitle = "Ad", // Would need to fetch from Ad
                    LastMessage = lastMessage.Content,
                    LastMessageTime = lastMessage.CreatedAt,
                    UnreadCount = unreadCount
                };
            });

        return conversations.OrderByDescending(c => c.LastMessageTime);
    }

    public async Task<MessageDto> SendMessageAsync(string senderId, string receiverId, Guid adId, string content)
    {
        var message = new Message
        {
            Content = content,
            SenderId = senderId,
            ReceiverId = receiverId,
            ProductAdId = adId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.CompleteAsync();

        return new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            ProductAdId = message.ProductAdId,
            CreatedAt = message.CreatedAt
        };
    }
}
