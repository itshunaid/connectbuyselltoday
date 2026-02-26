# TODO - Add Featured Ads Functionality

## Tasks
- [ ] 1. Add IsFeatured and FeaturedExpiryDate properties to ProductAd entity
- [ ] 2. Add IsFeatured and FeaturedExpiryDate properties to AdDto
- [ ] 3. Update AdRepository.GetRecentAdsAsync to prioritize featured ads
- [ ] 4. Update AdService methods to map IsFeatured and FeaturedExpiryDate
- [ ] 5. Add Featured badge overlay in Index.cshtml
- [ ] 6. Run EF Core migration to update database

## Implementation Order
1. ProductAd.cs - Add entity properties
2. AdDto.cs - Add DTO properties
3. AdRepository.cs - Update GetRecentAdsAsync query
4. AdService.cs - Update all methods that map to AdDto
5. Index.cshtml - Add Featured badge overlay
