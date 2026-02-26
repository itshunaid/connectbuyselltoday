# Task: Add Location Value Object and Geographic Filtering

## Plan
1. [x] Confirm plan with user
2. [x] Create Location Value Object
3. [x] Update ProductAd Entity
4. [x] Update IAdRepository Interface
5. [x] Update AdRepository with Haversine formula
6. [x] Update ApplicationDbContext
7. [x] Update AdDto
8. [x] Update IAdService Interface
9. [x] Update AdService

## Implementation Steps Completed

### Step 1: Create Location Value Object ✅
- Created `Domain/ConnectBuySellToday.Domain/Common/Location.cs`

### Step 2: Update ProductAd Entity ✅
- Added Location property to ProductAd

### Step 3: Update IAdRepository Interface ✅
- Updated GetFilteredAdsAsync signature with userLatitude, userLongitude, radiusInKm

### Step 4: Update AdRepository ✅
- Implemented Haversine formula in LINQ query for SQL translation

### Step 5: Update ApplicationDbContext ✅
- Configured Location as owned type for EF Core

### Step 6: Update AdDto ✅
- Added Latitude, Longitude, CityName properties

### Step 7: Update IAdService Interface ✅
- Updated SearchAdsAsync signature with location parameters

### Step 8: Update AdService ✅
- Passed location parameters to repository
- Updated CreateAdAsync and UpdateAdAsync to handle Location

## Followup Steps
- Run migrations to add the new Location column to database
- Test the geographic filtering functionality
