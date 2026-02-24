using ConnectBuySellToday.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectBuySellToday.Domain.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetConversationAsync(string user1Id, string user2Id, Guid adId);
        Task<IEnumerable<Message>> GetUserInboxAsync(string userId);
    }
}
