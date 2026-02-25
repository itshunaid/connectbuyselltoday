# ConnectBuySellToday

A professional full-stack ASP.NET Core web application for buying and selling products, built with **Clean Architecture** principles, featuring real-time messaging, user authentication, ad management, administrative moderation, and high-performance output caching.

---

## 🏗️ Architecture Overview

This project follows **Clean Architecture (Onion Architecture)** principles with clear separation of concerns across four distinct layers:

```
┌─────────────────────────────────────────────────────────────┐
│                      Web Layer                               │
│  (ASP.NET Core MVC + Razor Pages + SignalR + Output Cache) │
├─────────────────────────────────────────────────────────────┤
│                   Application Layer                         │
│        (Business Logic, DTOs, Service Interfaces)           │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                            │
│     (Entities, Enums, Repository Interfaces, Specifications)│
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                       │
│   (EF Core, Repositories, File Services, Data Context)     │
└─────────────────────────────────────────────────────────────┘
```

---

## 📁 Project Structure

```
ConnectBuySellToday/
├── Domain/                           # Core business entities & interfaces
│   ├── Entities/                     # Business entities
│   │   ├── ProductAd.cs              # Main ad listing entity
│   │   ├── Category.cs                # Product categories
│   │   ├── Message.cs                 # Chat messages
│   │   └── ApplicationUser.cs         # Custom Identity user
│   ├── Interfaces/                   # Repository contracts
│   │   ├── IUnitOfWork.cs
│   │   ├── IAdRepository.cs
│   │   ├── IMessageRepository.cs
│   │   ├── IUserRepository.cs
│   │   └── IFileService.cs
│   ├── Enums/                        # Business enums
│   │   └── AdStatus.cs               # Active, Sold, Hidden, PendingReview, Rejected
│   └── Common/                       # Base classes
│
├── Application/                      # Business logic layer
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── AdDto.cs
│   │   ├── MessageDto.cs
│   │   └── ConversationDto.cs
│   ├── Interfaces/                   # Service contracts
│   │   ├── IAdService.cs
│   │   ├── IMessageService.cs
│   │   └── IAdminService.cs          # Admin service interface
│   └── Services/                     # Business logic implementation
│       ├── AdService.cs
│       ├── MessageService.cs
│       └── AdminService.cs           # Administrative operations
│
├── Infrastructure/                   # External concerns
│   ├── Data/
│   │   ├── ApplicationDbContext.cs   # EF Core DbContext
│   │   └── UnitOfWork.cs            # Unit of Work pattern
│   ├── Repositories/                 # Data access implementations
│   │   ├── AdRepository.cs
│   │   ├── MessageRepository.cs
│   │   ├── UserRepository.cs
│   │   └── GenericRepository.cs
│   └── Services/                     # Infrastructure services
│       ├── ImageService.cs
│       └── LocalFileService.cs
│
└── Web/                              # Presentation layer
    ├── Controllers/                  # MVC Controllers
    │   ├── AdsController.cs          # Ad CRUD with cache invalidation
    │   ├── AccountController.cs
    │   ├── DashboardController.cs
    │   ├── HomeController.cs         # Output cached homepage
    │   └── AdminController.cs        # Admin panel (role-based)
    ├── Hubs/                         # SignalR Hubs
    │   └── ChatHub.cs                # Real-time chat
    └── Views/
        ├── Admin/                    # Admin views
        │   ├── Index.cshtml          # Dashboard with stats
        │   ├── Users.cshtml          # User management
        │   ├── PendingAds.cshtml     # Ad moderation
        │   └── Ads.cshtml            # All ads management
        └── ...
```

---

## 🛠️ Technology Stack

| Category | Technology |
|----------|------------|
| **Framework** | ASP.NET Core 8.0 (MVC) |
| **Database** | SQL Server + Entity Framework Core 8.0 |
| **Authentication** | ASP.NET Identity |
| **Authorization** | Role-based (Admin, User) |
| **Real-time** | SignalR |
| **Caching** | Output Caching (Memory) |
| **ORM** | Entity Framework Core |
| **Frontend** | Razor Views + Bootstrap 5 |
| **Image Storage** | Local file system |

---

## 🔑 Key Features

### 1. User Authentication & Authorization
- **ASP.NET Identity** for secure user management
- Role-based authorization (Admin, User)
- Custom user properties: `IsBanned`, `BanReason`, `BanExpiresAt`

### 2. Ad Management (CRUD)
- Create, read, update, delete product listings
- Multi-image upload support
- Category-based filtering
- Search functionality
- Price and status tracking
- **Ad Status Workflow**: Active → Sold, Hidden, PendingReview, Rejected

### 3. Real-time Messaging (SignalR)
- Live chat between buyers and sellers
- Conversation-based messaging
- Group-based message broadcasting
- Online presence tracking

### 4. 🛡️ Administrative Layer (Site Safety)
A complete admin panel for site moderation:

- **Dashboard**: Statistics (total users, active/banned, ads by status)
- **User Management**: View all users, ban/unban with reason and expiration
- **Ad Moderation**: Approve, reject, hide, show, delete ads
- **Content Review**: Pending ads queue for review before publishing

### 5. ⚡ Output Caching (High Performance)
Enterprise-level caching for instant page loads:

- **Homepage Caching**: 60-second cache for `/` route
- **Cache Tags**: Named cache tags for targeted invalidation
- **Automatic Invalidation**: Cache clears on ad create/update/delete
- **Scalability**: Supports thousands of concurrent users

---

## 📊 Database Schema

### Entities Relationship

```
ApplicationUser (Identity)
    │
    ├── ProductAd (1:N)
    │       │
    │       ├── Category (N:1)
    │       │
    │       └── AdImage (1:N)
    │
    └── Message (1:N)
```

### Key Tables
- **ProductAds**: Main listing table with seller, category, price, status
- **Categories**: Pre-seeded categories (Electronics, Vehicles, Furniture, etc.)
- **AdImages**: Multiple images per ad
- **Messages**: Conversation messages between users
- **AspNetUsers/ Roles/ Claims**: Identity tables with ban fields

---

## 🎯 Design Patterns Used

### 1. Repository Pattern
```
csharp
public interface IAdRepository
{
    Task<IEnumerable<ProductAd>> GetRecentAdsAsync(int count);
    Task<ProductAd?> GetAdByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<ProductAd>> GetFilteredAdsAsync(string? searchQuery, Guid? categoryId);
    Task<IEnumerable<ProductAd>> GetPendingAdsAsync();
    Task<IEnumerable<ProductAd>> GetAllAdsAsync();
}
```

### 2. Unit of Work
```
csharp
public interface IUnitOfWork : IDisposable
{
    IAdRepository Ads { get; }
    ICategoryRepository Categories { get; }
    IMessageRepository Messages { get; }
    IUserRepository Users { get; }
    Task<int> CompleteAsync();
}
```

### 3. Dependency Injection
```
csharp
// Program.cs
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdService, AdService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddOutputCache();
```

### 4. Admin Service Pattern
```
csharp
public interface IAdminService
{
    // User Management
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<bool> BanUserAsync(string userId, string reason, DateTime? banExpiresAt);
    Task<bool> UnbanUserAsync(string userId);
    
    // Ad Management
    Task<IEnumerable<AdDto>> GetPendingAdsAsync();
    Task<bool> ApproveAdAsync(Guid adId);
    Task<bool> RejectAdAsync(Guid adId, string reason);
    
    // Statistics
    Task<AdminDashboardDto> GetDashboardStatsAsync();
}
```

### 5. Output Caching Pattern
```
csharp
// HomeController - Cached endpoint
[OutputCache(Duration = 60, Tags = new[] { "home" })]
public async Task<IActionResult> Index()
{
    var ads = await _adService.GetLatestAdsAsync();
    return View(ads);
}

// AdsController - Cache invalidation
await _outputCacheStore.EvictByTagAsync("home", default);
```

### 6. Real-time Communication (SignalR)
```
csharp
[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(string conversationId, string receiverId, string message);
    public async Task JoinConversation(string conversationId);
}
```

---

## 🛡️ Administrative Layer Implementation

### Domain Extensions
```
csharp
// AdStatus Enum - Extended for moderation
public enum AdStatus
{
    Active = 1,
    Sold = 2,
    Hidden = 3,
    Expired = 4,
    PendingReview = 5,    // New: Awaiting admin approval
    Rejected = 6          // New: Rejected by admin
}

// ApplicationUser - Extended for bans
public class ApplicationUser : IdentityUser
{
    public bool IsBanned { get; set; }
    public string? BanReason { get; set; }
    public DateTime? BanExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Admin Controller
```
csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IOutputCacheStore _outputCacheStore;
    
    public async Task<IActionResult> Index();        // Dashboard
    public async Task<IActionResult> Users();        // User list
    public async Task<IActionResult> PendingAds();   // Ad review
    public async Task<IActionResult> ApproveAd(Guid id);
    public async Task<IActionResult> RejectAd(Guid id, string reason);
    public async Task<IActionResult> BanUser(string userId, string reason, DateTime? banExpiresAt);
}
```

---

## ⚡ Output Caching Implementation

### Program.cs Configuration
```
csharp
// Add memory cache and output caching
builder.Services.AddMemoryCache();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.With(c => c.HttpContext.Request.Path.StartsWithSegments("/Home")));
});

// Use middleware
app.UseOutputCache();
```

### Cache Invalidation Strategy
| Action | Cache Invalidated |
|--------|-------------------|
| Create Ad | ✅ Yes |
| Approve Ad | ✅ Yes |
| Reject Ad | ✅ Yes |
| Hide Ad | ✅ Yes |
| Show Ad | ✅ Yes |
| Delete Ad | ✅ Yes |
| Homepage Visit | Cached for 60s |

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Configuration

Update `appsettings.json`:
```
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ConnectBuySellToday;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Database Setup

```
powershell
# Package Manager Console
Update-Database

# Or CLI
dotnet ef database update
```

### Running the Application

```
bash
cd Web/ConnectBuySellToday.Web
dotnet run
```

Navigate to `https://localhost:7000`

---

## 📱 Key Endpoints

| Feature | Endpoint | Method | Auth |
|---------|----------|--------|------|
| Home (Cached) | `/` | GET | Anonymous |
| All Ads | `/Ads` | GET | Anonymous |
| Ad Details | `/Ads/Details/{id}` | GET | Anonymous |
| Create Ad | `/Ads/Create` | GET/POST | User |
| Dashboard | `/Dashboard` | GET | User |
| Chat | `/Dashboard/Chat` | GET | User |
| **Admin Dashboard** | `/Admin` | GET | **Admin** |
| **Manage Users** | `/Admin/Users` | GET/POST | **Admin** |
| **Pending Ads** | `/Admin/PendingAds` | GET | **Admin** |
| **Approve Ad** | `/Admin/ApproveAd/{id}` | POST | **Admin** |
| Login | `/Account/Login` | GET/POST | Anonymous |
| Register | `/Account/Register` | GET/POST | Anonymous |

---

## 🔐 Security Features

- Password requirements (digit, lowercase, uppercase, non-alphanumeric)
- **Role-based access control** (Admin, User)
- Anti-forgery tokens on forms
- SQL injection prevention via EF Core
- XSS prevention via Razor encoding
- **User banning system** with expiration support
- **Ad moderation** with approve/reject workflow

---

## 🎨 UI/UX Features

- Bootstrap 5 responsive design
- Real-time chat notifications
- Image gallery for ads
- Category filtering
- Search functionality
- User dashboard with tabbed interface
- **Admin panel** with statistics cards
- **Moderation tools** with approve/reject modals

---

## 📈 Performance Optimizations

| Feature | Implementation | Benefit |
|---------|---------------|---------|
| Output Caching | Memory cache on `/Home` | Instant page loads |
| Cache Tags | Named cache "home" | Targeted invalidation |
| Lazy Loading | EF Core navigation | Efficient DB queries |
| Async/Await | All I/O operations | Non-blocking threads |

---

## 📝 License

This project is for educational and demonstration purposes.

---

## 👤 Author

Built with ❤️ using **ASP.NET Core** and **Clean Architecture** principles.

---

## 🙏 Acknowledgments

- ASP.NET Core Documentation
- Entity Framework Core Documentation
- SignalR Documentation
- Microsoft Output Caching
- Clean Architecture patterns
