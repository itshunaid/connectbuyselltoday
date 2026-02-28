# Report Ad Feature Implementation

## Domain Layer
- [ ] Create ReportReason enum
- [ ] Create ReportStatus enum
- [ ] Create ReportAd entity

## Application Layer
- [ ] Create ReportAdDto
- [ ] Create IReportService interface
- [ ] Implement ReportService

## Infrastructure Layer
- [ ] Add DbSet<ReportAd> to ApplicationDbContext
- [ ] Add IReportRepository interface
- [ ] Implement ReportRepository

## Web Layer
- [ ] Add Report endpoint to AdsController
- [ ] Add Report modal to Details.cshtml
- [ ] Add Reports management page to AdminController
- [ ] Add Reports view for Admin

## Database
- [ ] Create migration for ReportAd entity
