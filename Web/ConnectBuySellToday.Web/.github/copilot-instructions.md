# ConnectBuySellToday - Copilot Coding Standards

You are an expert .NET architect. When generating code for this OLX-clone, strictly follow these rules:

## 1. Clean Architecture Layers
- **Domain:** No dependencies. Contains Entities, Enums, and Interfaces. 
- **Application:** Depends ONLY on Domain. Contains DTOs, Services, and Business Logic. Use `IUnitOfWork` for data access.
- **Infrastructure:** Implements Domain interfaces. Contains `DbContext`, Repositories, SignalR Hubs, and File Storage.
- **Web (MVC):** Depends on Application. Controllers must only use Services, never Repositories or DbContext directly.

## 2. Patterns & Practices
- **Persistence:** Use the Repository Pattern + Unit of Work. Use `Task<T>` and `async/await` for all database and I/O operations.
- **Data Mapping:** Always map Entities to DTOs before sending data to the Web layer.
- **UI:** Use Bootstrap 5. Primary color: `#002f34`, Accent: `#ffce32`.
- **Validation:** Use Data Annotations in DTOs and validate `ModelState` in Controllers.

## 3. Real-Time & Messaging
- Use **SignalR** for all chat functionality. Ensure `Context.UserIdentifier` is used to map connections to Identity Users.