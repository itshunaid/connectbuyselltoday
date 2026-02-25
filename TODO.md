# TODO - Favorite Feature Implementation

## Domain Layer
- [x] Create Favorite entity in Domain/Entities/Favorite.cs
- [x] Create IFavoriteRepository interface in Domain/Interfaces/IFavoriteRepository.cs

## Infrastructure Layer
- [x] Update ApplicationDbContext.cs - add DbSet<Favorite> and configure relationships
- [x] Update IAdRepository.cs - add GetFavoriteAdsByUserIdAsync method
- [x] Implement GetFavoriteAdsByUserIdAsync in AdRepository.cs
- [x] Create FavoriteRepository.cs in Infrastructure/Repositories/
- [x] Update IUnitOfWork.cs - add Favorite repository property
- [x] Update UnitOfWork.cs - implement Favorite repository

## Application Layer
- [x] Create FavoriteService.cs with ToggleFavoriteAsync method

## Database
- [x] Create migration for Favorite entity
