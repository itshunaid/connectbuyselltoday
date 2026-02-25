# TODO - Admin Moderation & Performance Optimization

## Task 1: Admin Moderation (Approve/Reject Ads)
- [ ] 1.1 Update AdService.CreateAdAsync to set status to PendingReview
- [ ] 1.2 Update AdRepository.GetRecentAdsAsync to filter by Active status only
- [ ] 1.3 Verify AdRepository.GetFilteredAdsAsync also filters by Active status

## Task 2: Performance Optimization - Caching Categories
- [ ] 2.1 Create ICategoryService interface
- [ ] 2.2 Create CategoryService with IMemoryCache implementation
- [ ] 2.3 Register CategoryService in Program.cs
- [ ] 2.4 Update HomeController to include categories in ViewBag
- [ ] 2.5 Update Layout to display categories navigation

## Implementation Notes:
- Admin moderation flow: User creates ad → Status = PendingReview → Admin approves → Status = Active
- Categories will be cached for 30 minutes to improve performance
