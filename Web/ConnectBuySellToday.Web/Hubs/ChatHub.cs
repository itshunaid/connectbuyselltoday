using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Application.DTOs;

namespace ConnectBuySellToday.Web.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatHub(IMessageService messageService, IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _hubContext = hubContext;
    }

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendTyping(string conversationId, string receiverId)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var userName = Context.User?.Identity?.Name ?? "User";
        
        // Broadcast typing status to the conversation group (excluding sender)
        await Clients.OthersInGroup(conversationId).SendAsync("Typing", new
        {
            UserId = userId,
            UserName = userName,
            IsTyping = true
        });
    }

    public async Task SendMessage(string conversationId, string receiverId, string message, string senderId)
    {
        // Get the sender's user ID from claims
        var actualSenderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(actualSenderId))
        {
            return;
        }

        // Parse conversation ID to get adId
        var parts = conversationId.Split('_');
        if (parts.Length < 3 || !Guid.TryParse(parts[2], out var adId))
        {
            return;
        }

        // Save message to database via service
        try
        {
            var messageDto = await _messageService.SendMessageAsync(actualSenderId, receiverId, adId, message);
            
            // Broadcast the message to the conversation group
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", new
            {
                SenderId = actualSenderId,
                SenderName = Context.User?.Identity?.Name ?? "User",
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            // Add user to a group based on their user ID for private messaging
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
