# TODO - Fix Category Dropdown Validation Error

## Plan
- [x] Seed categories with Guids in ApplicationDbContext.cs
- [x] Update AdsController.cs to pass categories to the view
- [x] Update Create.cshtml to use dynamic category data

## Status: Completed

## Changes Made:
1. **ApplicationDbContext.cs**: Added seed data with predefined Guids matching the integer values (1-7) to maintain backward compatibility
2. **AdsController.cs**: 
   - Added IUnitOfWork dependency injection
   - Updated Create GET and POST methods to fetch categories from database
3. **Create.cshtml**: Replaced hardcoded integer options with dynamic category list from ViewBag

## Next Steps:
- Run `dotnet ef migrations add SeedCategories` to create migration for seed data
- Run `dotnet ef database update` to apply the migration
- Test the application
