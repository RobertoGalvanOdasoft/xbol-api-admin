using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Business.Extensions;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class VenueService
    {
        private readonly VenueRepository _venueRepository;

        public VenueService(VenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        // TODO: Implement query params
        public async Task<PagedResponse<VenueResponse>> GetVenueListAsync(VenueQueryParams queryParams)
        {
            var query = _venueRepository
                            .Get()
                            .AsNoTracking()
                            .Where(v => v.IsDeleted == false)
                            .Select(v => new VenueResponse
                            {
                                Id = v.Id,
                                Name = v.Name,
                                Country = v.Country,
                                State = v.State,
                                City = v.City,
                                Neighborhood = v.Neighborhood,
                                StreetAddress = v.StreetAddress,
                                ExtNum = v.ExtNum,
                                IntNum = v.IntNum,
                                ZipCode = v.ZipCode,
                                Latitude = v.Latitude,
                                Longitude = v.Longitude,
                                LandingUrl = v.LandingUrl,
                                ContactName = v.ContactName,
                                ContactEmail = v.ContactEmail,
                                PhoneRegionCodeId = v.PhoneRegionCodeId,
                                DialCode = v.PhoneRegionCode == null ? "" : v.PhoneRegionCode.DialCode,
                                ContactPhoneNumber = v.ContactPhoneNumber,
                                Category = v.Category,
                                Status = v.Status
                            });

            SetCitiesFilter(ref query, queryParams.Cities);
            SetCategoryFilter(ref query, queryParams.Categories);
            SetSearchTermFilter(ref query, queryParams.SearchTerm);
            SetOrder(ref query, queryParams.SortBy, queryParams.Descending);

            return await query.ToPagedResponseAsync(queryParams.Page, queryParams.PageSize);
        }

        public async Task<VenueResponse?> GetVenueByIdAsync(long venueId)
        {
            return await _venueRepository
                            .Get()
                            .AsNoTracking()
                            .Where(v => v.Id == venueId
                                && v.IsDeleted == false)
                            .Select(v => new VenueResponse
                            {
                                Id = v.Id,
                                Name = v.Name,
                                Country = v.Country,
                                State = v.State,
                                City = v.City,
                                Neighborhood = v.Neighborhood,
                                StreetAddress = v.StreetAddress,
                                ExtNum = v.ExtNum,
                                IntNum = v.IntNum,
                                ZipCode = v.ZipCode,
                                Latitude = v.Latitude,
                                Longitude = v.Longitude,
                                LandingUrl = v.LandingUrl,
                                ContactName = v.ContactName,
                                ContactEmail = v.ContactEmail,
                                PhoneRegionCodeId = v.PhoneRegionCodeId,
                                DialCode = v.PhoneRegionCode == null ? "" : v.PhoneRegionCode.DialCode,
                                ContactPhoneNumber = v.ContactPhoneNumber,
                                Category = v.Category,
                                Status = v.Status
                            }).FirstOrDefaultAsync();
        }

        public async Task<List<ListItem>> GetVenueCatalogAsync()
        {
            return await _venueRepository.Get()
                            .AsNoTracking()
                            .Where(v => v.IsDeleted == false
                                && v.Status == VenueStatus.Active)
                            .Select(v => new ListItem
                            {
                                Id = v.Id,
                                Name = v.Name
                            }).ToListAsync();
        }

        public async Task<List<string>> GetVenueCitiesCatalogAsync()
        {
            return await _venueRepository.Get()
                            .AsNoTracking()
                            .Where(v => v.IsDeleted == false)
                            .Select(v => v.City)
                            .Distinct()
                            .ToListAsync();
        }

        public async Task<bool> DeleteVenueAsync(long venueId)
        {
            // TODO: Pending validations

            try
            {
                Venue? venue = await _venueRepository.GetByIdAsync(venueId);

                if (venue == null || venue.IsDeleted)
                {
                    Console.WriteLine($"Venue with Id '{venueId}' was not found.");
                    return false;
                }

                venue.IsDeleted = true;
                venue.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
                venue.UpdatedBy = Guid.Empty;

                await _venueRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete venue with Id '{venueId}'. {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateVenueAsync(long venueId, VenueRequest request)
        {
            try
            {
                Venue? venue = await _venueRepository.GetByIdAsync(venueId);

                if (venue == null || venue.IsDeleted)
                {
                    Console.WriteLine($"Venue with Id '{venueId}' was not found.");
                    return false;
                }

                venue.Name = request.Name;
                venue.Category = request.Category;
                venue.Status = request.Status;
                venue.Country = request.Country;
                venue.State = request.State;
                venue.City = request.City;
                venue.Neighborhood = request.Neighborhood;
                venue.StreetAddress = request.StreetAddress;
                venue.ExtNum = request.ExtNum;
                venue.IntNum = request.IntNum;
                venue.ZipCode = request.ZipCode;
                venue.Latitude = request.Latitude;
                venue.Longitude = request.Longitude;
                venue.LandingUrl = request.LandingUrl;
                venue.ContactName = request.ContactName;
                venue.ContactEmail = request.ContactEmail;
                venue.PhoneRegionCodeId = request.PhoneRegionCodeId;
                venue.ContactPhoneNumber = request.ContactPhoneNumber;
                venue.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
                venue.UpdatedBy = Guid.Empty;

                await _venueRepository.UpdateAsync(venue);
                await _venueRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to update a Venue. {ex.Message}");
                return false;
            }
        }

        public async Task<long> CreateVenueAsync(VenueRequest request)
        {
            try
            {
                Venue venue = new Venue()
                {
                    Name = request.Name,
                    Category = request.Category,
                    Status = request.Status,
                    Country = request.Country,
                    State = request.State,
                    City = request.City,
                    Neighborhood = request.Neighborhood,
                    StreetAddress = request.StreetAddress,
                    ExtNum = request.ExtNum,
                    IntNum = request.IntNum,
                    ZipCode = request.ZipCode,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    LandingUrl = request.LandingUrl,
                    ContactName = request.ContactName,
                    ContactEmail = request.ContactEmail,
                    PhoneRegionCodeId = request.PhoneRegionCodeId,
                    ContactPhoneNumber = request.ContactPhoneNumber,
                    CreatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                    UpdatedBy = Guid.Empty
                };

                await _venueRepository.InsertAsync(venue);
                await _venueRepository.CommitAsync();

                return venue.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to create a Venue. {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateVenueStatusAsync(long venueId, VenueStatus venueStatus)
        {
            try
            {
                Venue? venue = await _venueRepository.GetByIdAsync(venueId);

                if (venue == null || venue.IsDeleted)
                {
                    Console.WriteLine($"Venue with Id '{venueId}' was not found.");
                    return false;
                }

                venue.Status = venueStatus;

                await _venueRepository.UpdateAsync(venue);
                await _venueRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to update the status of the Venue. {ex.Message}");
                return false;
            }
        }

        private void SetCitiesFilter(ref IQueryable<VenueResponse> query, List<string> cities)
        {
            if (cities.Any())
            {
                cities = cities.Select(x => x.ToLower().Trim()).ToList();
                query = query.Where(x => cities.Contains(x.City.ToLower().Trim()));
            }
        }

        private void SetCategoryFilter(ref IQueryable<VenueResponse> query, List<VenueCategory> categories)
        {
            if (categories.Any())
            {
                query = query.Where(x => categories.Contains(x.Category));
            }
        }

        private void SetSearchTermFilter(ref IQueryable<VenueResponse> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return;
            }

            var lowerSearchTerm = searchTerm.ToLower();

            query = query.Where(x => x.Name.ToLower().Contains(lowerSearchTerm)
                || x.Country.ToLower().Contains(lowerSearchTerm)
                || x.State.ToLower().Contains(lowerSearchTerm)
                || x.City.ToLower().Contains(lowerSearchTerm)
                || x.Neighborhood.ToLower().Contains(lowerSearchTerm)
                || x.StreetAddress.ToLower().Contains(lowerSearchTerm)
                || x.ExtNum.ToLower().Contains(lowerSearchTerm)
                || x.IntNum.ToLower().Contains(lowerSearchTerm)
                || x.ZipCode.ToLower().Contains(lowerSearchTerm));
        }

        private void SetOrder(ref IQueryable<VenueResponse> query, string sortBy, bool descensing)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return;
            }

            query = sortBy.ToLower() switch
            {
                _ => query
            };
        }
    }
}
