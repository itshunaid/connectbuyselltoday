# ConnectBuySellToday

A full-stack ASP.NET Core web application for buying and selling products, featuring real-time messaging, user authentication, and ad management.

## 🏗️ Architecture Overview

This project follows **Clean Architecture (Onion Architecture)** principles with clear separation of concerns across four distinct layers:

```
┌─────────────────────────────────────────────────────────────┐
│                      Web Layer                               │
│  (ASP.NET Core MVC + Razor Pages + SignalR)                │
├─────────────────────────────────────────────────────────────┤
│                   Application Layer                           │
│        (Business Logic, DTOs, Service Interfaces)           │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                            │
│     (Entities, Enums, Repository Interfaces, Specifications)│
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                       │
│   (EF Core, Repositories, File Services, Data Context)      │
└─────────────────────────────────────────────────────────────┘
```

---

## 📁 Project Structure

```
ConnectBuySellToday/
├── Domain/                           # Core business entities & interfaces
│   ├── Entities/                     # Business entities
│   │   ├── ProductAd.cs             # Main ad listing entity
│   │   ├── Category.cs              # Product categories
│   │   ├── Message.cs               # Chat messages
│   │   └── ApplicationUser.cs        # Custom Identity user
│   ├── Interfaces/                   # Repository contracts
│   │   ├── IUnitOfWork.cs
│   │   ├── IAdRepository.cs
│   │   ├── IMessageRepository.cs
│   │   └── IFileService.cs
│   ├── Enums/                        # Business enums
│   └── Common/                       # Base classes
│
├── Application/                      # Business logic layer
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── AdDto.cs
│   │   ├── MessageDto.cs
│   │   └── ConversationDto.cs
│   ├── Interfaces/                   # Service contracts
│   │   ├── IAdService.cs
│   │   └── IMessageService.cs
│   └── Services/                     # Business logic implementation
│       ├── AdService.cs
│       └── MessageService.cs
│
├── Infrastructure/                   # External concerns
│   ├── Data/
│   │   ├── ApplicationDbContext.cs   # EF Core DbContext
│   │   └── UnitOfWork.cs
│   ├── Repositories/                 # Data access implementations
│   │   ├── AdRepository.cs
│   │   ├── MessageRepository.cs
│   │   └── GenericRepository.cs
│   └── Services/                     # Infrastructure services
│       ├── ImageService.cs
│       └── LocalFileService.cs
│
└── Web/                              # Presentation layer
    ├── Controllers/                  # MVC Controllers
    │   ├── AdsController.cs
    │   ├── AccountController.cs
    │   ├── DashboardController.cs
    │   └── HomeController.cs
    ├── Hubs/                         # SignalR Hubs
    │   └── ChatHub.cs                # Real-time chat
    └── Views/                        # Razor Views
```

---

## 🛠️ Technology Stack

| Category | Technology |
|----------|------------|
| **Framework** | ASP.NET Core 8.0 (MVC) |
| **Database** | SQL Server + Entity Framework Core |
| **Authentication** | ASP.NET Identity |
| **Real-time** | SignalR |
| **ORM** | Entity Framework Core 8.0 |
| **Frontend** | Razor Views + Bootstrap 5 |
| **Image Storage** | Local file system |

---

## 🔑 Key Features

### 1. User Authentication & Authorization
- **ASP.NET Identity** for secure user management
- Role-based authorization
- Custom user properties via `ApplicationUser`

### 2. Ad Management (CRUD)
- Create, read, update, delete product listings
- Multi-image upload support
- Category-based filtering
- Search functionality
- Price and status tracking

### 3. Real-time Messaging (SignalR)
- Live chat between buyers and sellers
- Conversation-based messaging
- Group-based message broadcasting
- Online presence tracking

### 4. Clean Architecture Implementation

#### Domain Layer
- **Entities**: Pure business objects with no external dependencies
- **Interfaces**: Abstractions for repositories and services
- **Specifications**: Query specifications pattern
- **Enums**: Business status types (Active, Sold, etc.)

#### Application Layer
- **DTOs**: Flat data transfer objects for API/UI
- **Service Interfaces**: Contracts for business logic
- **Services**: Implementation of business operations

#### Infrastructure Layer
- **EF Core Configuration**: Fluent API configurations
- **Repositories**: Data access with Unit of Work pattern
- **File Services**: Image upload/download abstraction

#### Web Layer
- **MVC Controllers**: Request handling
- **SignalR Hubs**: Real-time communication
- **Razor Views**: Server-side rendering

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
- **ProductAds**: Main listing table with seller, category, price
- **Categories**: Pre-seeded categories (Electronics, Vehicles, Furniture, etc.)
- **AdImages**: Multiple images per ad
- **Messages**: Conversation messages between users
- **AspNetUsers/ Roles/ Claims**: Identity tables

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
    Task<int> CompleteAsync();
}
```

### 3. Dependency Injection
All services and repositories are registered in `Program.cs`:
```
csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdService, AdService>();
builder.Services.AddScoped<IMessageService, MessageService>();
```

### 4. Real-time Communication (SignalR)
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

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Configuration

Update `appsettings.json` with your connection string:
```
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ConnectBuySellToday;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Database Setup

1. Update-Database in Package Manager Console:
```
powershell
Update-Database
```

Or use CLI:
```
bash
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

| Feature | Endpoint | Method |
|---------|----------|--------|
| Home | `/` | GET |
| All Ads | `/Ads` | GET |
| Ad Details | `/Ads/Details/{id}` | GET |
| Create Ad | `/Ads/Create` | GET/POST |
| My Ads | `/Dashboard/MyAds` | GET |
| Chat | `/Dashboard/Chat/{conversationId}` | GET |
| Login | `/Account/Login` | GET/POST |
| Register | `/Account/Register` | GET/POST |

---

## 📂 Image Upload Architecture

```
wwwroot/
└── uploads/
    └── ads/
        └── {guid}.jpg
```

- Images stored locally in `wwwroot/uploads/ads/`
- Unique GUID-based filenames
- Multiple images per ad supported
- Main image designation

---

## 🔐 Security Features

- Password requirements (digit, lowercase, uppercase, non-alphanumeric)
- Role-based access control
- Anti-forgery tokens on forms
- SQL injection prevention via EF Core parameterized queries
- XSS prevention via Razor encoding
- Secure cookie configuration

---

## 🎨 UI/UX Features

- Bootstrap 5 responsive design
- Real-time chat notifications
- Image gallery for ads
- Category filtering
- Search functionality
- User dashboard with tabbed interface

---

## 📈 Future Enhancements

- [ ] Payment integration (Stripe/PayPal)
- [ ] Email notifications
- [ ] Mobile API (REST/GraphQL)
- [ ] Cloud storage (Azure Blob/AWS S3)
- [ ] Advanced search with filters
- [ ] Rating/Review system
- [ ] Admin dashboard

---

## 📝 License

This project is for educational and demonstration purposes.

---

## 👤 Author

Built with ❤️ using ASP.NET Core and Clean Architecture principles.

---

## 🙏 Acknowledgments

- ASP.NET Documentation
- Entity Framework Core Documentation
- SignalR Documentation
- Clean Architecture patterns from various open-source projects
