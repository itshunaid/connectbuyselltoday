using ConnectBuySellToday.Domain.Common;
using ConnectBuySellToday.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ProductAd> ProductAds => Set<ProductAd>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<AdImage> AdImages => Set<AdImage>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<ReportAd> ReportAds => Set<ReportAd>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations for relationships, decimal precision, etc.
        modelBuilder.Entity<ProductAd>().Property(p => p.Price).HasPrecision(18, 2);

        // Configure Location as an owned type
        modelBuilder.Entity<ProductAd>().OwnsOne(p => p.Location);

        // SellerId is nullable to support SetNull delete behavior when a user is deleted
        modelBuilder.Entity<ProductAd>()
            .HasOne(p => p.Seller)
            .WithMany()
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.SetNull)  // Set null instead of cascade when user is deleted
            .IsRequired(false);  // Mark as optional (nullable) to support SetNull

        // Favorite entity configuration
        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.ProductAd)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.ProductAdId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure unique constraint on UserId and ProductAdId combination
        modelBuilder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.ProductAdId })
            .IsUnique();

        // ReportAd entity configuration
        modelBuilder.Entity<ReportAd>()
            .HasOne(r => r.Ad)
            .WithMany()
            .HasForeignKey(r => r.AdId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReportAd>()
            .HasOne(r => r.Reporter)
            .WithMany()
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReportAd>()
            .HasIndex(r => r.Status);

        // Seed categories with predefined Guids to match the dropdown values
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name = "Electronics", CreatedAt = DateTime.UtcNow },
            new Category { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name = "Vehicles", CreatedAt = DateTime.UtcNow },
            new Category { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name = "Furniture", CreatedAt = DateTime.UtcNow },
            new Category { Id = new Guid("44444444-4444-4444-4444-444444444444"), Name = "Clothing", CreatedAt = DateTime.UtcNow },
            new Category { Id = new Guid("55555555-5555-5555-5555-555555555555"), Name = "Books", CreatedAt = DateTime.UtcNow },
            new Category { Id = new Guid("66666666-6666-6666-6666-666666666666"), Name = "Sports", CreatedAt = DateTime.UtcNow },
            new Category { Id = new Guid("77777777-7777-7777-7777-777777777777"), Name = "Other", CreatedAt = DateTime.UtcNow }
        );
    }
}
