
using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;
using ConnectBuySellToday.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ConnectBuySellToday.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        Ads = new AdRepository(_context);
        Categories = new CategoryRepository(_context);
        AdsImage = new AdImageRepository(_context);
        Messages = new MessageRepository(_context);
        Users = new UserRepository(_userManager);
        Favorites = new FavoriteRepository(_context);
    }

    public IAdRepository Ads { get; private set; } = null!;
    public ICategoryRepository Categories { get; private set; } = null!;
    public IAdImageRepository AdsImage { get; private set; } = null!;
    public IMessageRepository Messages { get; private set; } = null!;
    public IUserRepository Users { get; private set; } = null!;
    public IFavoriteRepository Favorites { get; private set; } = null!;

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
