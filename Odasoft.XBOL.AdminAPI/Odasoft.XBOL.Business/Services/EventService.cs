using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Business.Services
{
    public class EventService(EventRepository eventRepository, XBOLDbContext dbContext)
    {

        public async Task<PagedResponse<EventListItemDTO>> GetEventListAsync(
            string? venues,
            string? categories,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            string? search,
            string? sortBy,
            bool? descending,
            int? page,
            int? pageSize,
            long? seasonId = null,
            EventStatus? status = null,
            bool? upcoming = null)
        {

            var events = await eventRepository.GetEventListAsync(
                venues,
                categories,
                startDate,
                endDate,
                search,
                sortBy,
                descending ?? false,
                page ?? 1,
                pageSize ?? 10,
                seasonId,
                status,
                upcoming);

            return events;
        }

        public async Task<PagedResponse<EventListItemDTO>> GetEventsOnSaleAsync(
            string? venues,
            string? categories,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            string? search,
            string? sortBy,
            bool? descending,
            int? page,
            int? pageSize)
        {
            var events = await eventRepository.GetEventsOnSaleAsync(
                venues,
                categories,
                startDate,
                endDate,
                search,
                sortBy,
                descending ?? false,
                page ?? 1,
                pageSize ?? 10);
            return events;
        }

        public async Task<EventInfoDTO?> GetEventByIdAsync(long eventId)
        {
            var result = await eventRepository.GetEventByIdAsync(eventId);

            // TODO: Handle null result (e.g., throw exception or return a default value)
            return result;
        }

        public async Task<IList<ListItem>> GetEventCatalogAsync()
        {
            return await eventRepository
                            .Get()
                            .AsNoTracking()
                            .Select(x => new ListItem
                            {
                                Id = x.Id,
                                Name = x.Name
                            }).ToListAsync();
        }

        public async Task<EventResult?> CreateEventAsync(CreateEventRequest request)
        {
            var categories = await dbContext.EventCategories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync();

            Models.Event newEvent = new()
            {
                VenueMapId = request.VenueMapId,
                Name = request.Name,
                Subtitle = request.Subtitle,
                ShortDescription = request.ShortDescription,
                LongDescription = request.LongDescription,
                Categories = categories,
                Status = EventStatus.Draft,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty, // TODO: Replace with actual user ID from context
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedBy = Guid.Empty
            };
            try
            {
                await eventRepository.InsertAsync(newEvent);
                await eventRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Event: {ex.Message}");
                return null;
            }
            return new EventResult
            {
                Id = newEvent.Id,
                VenueMapId = newEvent.VenueMapId,
                Name = newEvent.Name,
                Subtitle = newEvent.Subtitle,
                ShortDescription = newEvent.ShortDescription,
                LongDescription = newEvent.LongDescription,
                Categories = [.. newEvent.Categories.Select(c => new EventCategoryResult
                {
                    Id = c.Id,
                    Name = c.Name,
                    DisplayName = c.DisplayName,
                    IsActive = c.IsActive,
                })],
                Status = newEvent.Status
            };
        }

        public async Task<bool> UpdateEventAsync(long eventId, UpdateEventRequest request)
        {
            var existingEvent = await dbContext.Events
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (existingEvent == null)
            {
                Console.WriteLine($"Event with ID {eventId} not found.");
                return false;
            }

            existingEvent.VenueMapId = request.VenueMapId;
            existingEvent.Name = request.Name;
            existingEvent.Subtitle = request.Subtitle;
            existingEvent.ShortDescription = request.ShortDescription;
            existingEvent.LongDescription = request.LongDescription;
            existingEvent.UpdatedAt = DateTimeOffset.UtcNow;
            existingEvent.UpdatedBy = Guid.Empty; // TODO: Replace with actual user ID from context

            var newCategories = await dbContext.EventCategories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync();

            existingEvent.Categories.Clear();
            foreach (var cat in newCategories)
            {
                existingEvent.Categories.Add(cat);
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Event: {ex.Message}");
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteEventAsync(long eventId)
        {
            Models.Event? existingEvent = await eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null)
            {
                Console.WriteLine($"Event with ID {eventId} not found.");
                return false;
            }

            if (existingEvent.Schedules.Count > 0)
            {
                Console.WriteLine($"Cannot delete Event with ID {eventId} because it has associated schedules.");
                return false;
            }

            existingEvent.DeletedAt = DateTimeOffset.UtcNow;
            existingEvent.UpdatedAt = DateTimeOffset.UtcNow;
            existingEvent.UpdatedBy = Guid.Empty; // TODO: Replace with actual user ID from context

            try
            {
                await eventRepository.UpdateAsync(existingEvent);
                await eventRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Event: {ex.Message}");
                return false;
            }
            return true;
        }
    }
}
