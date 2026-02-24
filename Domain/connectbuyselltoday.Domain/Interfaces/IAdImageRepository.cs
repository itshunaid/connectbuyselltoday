using ConnectBuySellToday.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectBuySellToday.Domain.Interfaces
{
    public interface IAdImageRepository : IGenericRepository<AdImage>
    {
        Task<IEnumerable<AdImage>> GetImagesByAdIdAsync(Guid adId);
        Task SetMainImageAsync(Guid adId, Guid imageId);
    }
}
