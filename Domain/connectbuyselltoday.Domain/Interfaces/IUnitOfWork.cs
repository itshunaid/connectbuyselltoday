using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace connectbuyselltoday.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAdRepository Ads { get; }
        ICategoryRepository Categories { get; }

        IAdImageRepository AdsImage { get; }

        IMessageRepository Messages { get; }
        // You can add IMessageRepository here later

        Task<int> CompleteAsync(); // Saves all changes to the DB
    }
}
