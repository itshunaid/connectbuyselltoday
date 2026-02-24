using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;
using ConnectBuySellToday.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Infrastructure.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    private readonly new ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(string user1Id, string user2Id, Guid adId)
    {
        return await _context.Messages
            .Where(x => x.ProductAdId == adId &&
                   ((x.SenderId == user1Id && x.ReceiverId == user2Id) ||
                    (x.SenderId == user2Id && x.ReceiverId == user1Id)))
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetUserInboxAsync(string userId)
    {
        return await _context.Messages
            .Where(x => x.ReceiverId == userId)
            .Include(x => x.ProductAd)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}
