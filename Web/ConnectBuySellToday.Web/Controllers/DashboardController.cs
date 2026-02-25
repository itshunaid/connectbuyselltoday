using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ConnectBuySellToday.Web.Hubs;
using System.Security.Claims;

namespace ConnectBuySellToday.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IAdService _adService;
    private readonly IMessageService _messageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IAdService adService, 
        IMessageService messageService,
        IUnitOfWork unitOfWork,
        IHubContext<ChatHub> hubContext,
        ILogger<DashboardController> logger)
    {
        _adService = adService;
        _messageService = messageService;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Get user's ads
        var myAds = await _adService.GetAdsBySellerIdAsync(userId!);
        
        // Get user's conversations
        var conversations = await _messageService.GetUserConversationsAsync(userId!);
        
        var model = new DashboardViewModel
        {
            MyAds = myAds,
            Conversations = conversations
        };
        
        return View(model);
    }

    public async Task<IActionResult> MyAds()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ads = await _adService.GetAdsBySellerIdAsync(userId!);
        return View(ads);
    }

    public async Task<IActionResult> MyChats()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var conversations = await _messageService.GetUserConversationsAsync(userId!);
        return View(conversations);
    }

    public async Task<IActionResult> Chat(Guid adId, string otherUserId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        // Get the ad details
        var ad = await _adService.GetAdByIdAsync(adId);
        if (ad == null)
        {
            return NotFound();
        }

        // Get the other user ID (seller or buyer)
        var receiverId = otherUserId == userId ? ad.SellerId : otherUserId;
        
        // Get conversation messages
        var messages = await _messageService.GetConversationAsync(userId, receiverId, adId);
        
        // Generate conversation ID
        var conversationId = GetConversationId(userId, receiverId, adId);
        
        var model = new ChatViewModel
        {
            AdId = adId,
            AdTitle = ad.Title,
            AdImageUrl = ad.MainImageUrl,
            OtherUserId = receiverId,
            CurrentUserId = userId,
            Messages = messages
        };
        
        ViewBag.ConversationId = conversationId;
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(Guid adId, string receiverId, string message)
    {
        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
        {
            return BadRequest("Invalid sender or receiver");
        }

        // Save message to database
        var messageDto = await _messageService.SendMessageAsync(senderId, receiverId, adId, message);
        
        // Get conversation ID
        var conversationId = GetConversationId(senderId, receiverId, adId);
        
        // Broadcast to SignalR group
        await _hubContext.Clients.Group(conversationId).SendAsync("ReceiveMessage", new
        {
            SenderId = senderId,
            SenderName = User.Identity?.Name ?? "User",
            Message = message,
            Timestamp = DateTime.UtcNow
        });

        return Ok();
    }

    private string GetConversationId(string user1Id, string user2Id, Guid adId)
    {
        var ids = new[] { user1Id, user2Id }.OrderBy(x => x).ToArray();
        return $"{ids[0]}_{ids[1]}_{adId}";
    }
}

public class DashboardViewModel
{
    public IEnumerable<AdDto> MyAds { get; set; } = Enumerable.Empty<AdDto>();
    public IEnumerable<ConversationDto> Conversations { get; set; } = Enumerable.Empty<ConversationDto>();
}

public class ChatViewModel
{
    public Guid AdId { get; set; }
    public string AdTitle { get; set; } = string.Empty;
    public string? AdImageUrl { get; set; }
    public string OtherUserId { get; set; } = string.Empty;
    public string CurrentUserId { get; set; } = string.Empty;
    public IEnumerable<MessageDto> Messages { get; set; } = Enumerable.Empty<MessageDto>();
}
