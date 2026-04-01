using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class EventScheduleService(EventScheduleRepository repository)
    {

        public async Task<long?> GetEventIdByExternalEventKeyAsync(string eventKey)
        {
            EventSchedule? schedule = await repository.Get(x => x.ExternalEventKey == eventKey).FirstOrDefaultAsync();

            return schedule?.EventId;
        }

        public async Task<EventScheduleResult?> CreateEventScheduleAsync(CreateEventScheduleRequest request)
        {
            var newSchedule = new EventSchedule
            {
                EventId = request.EventId,
                PreSaleStartDate = request.PreSaleDate.ToUniversalTime(),
                PreSaleEndDate = request.PreSaleEndDate.ToUniversalTime(),
                OnSaleDate = request.OnSaleDate.ToUniversalTime(),
                OffSaleDate = request.OffSaleDate.ToUniversalTime(),
                PublishedDate = request.PublishedDate.ToUniversalTime(),
                GateOpenDate = request.GateOpenDate.ToUniversalTime(),
                StartDateTime = request.StartDateTime.ToUniversalTime(),
                EndDateTime = request.EndDateTime.ToUniversalTime(),
                Status = ScheduleStatus.Draft,
                ExternalEventKey = string.Empty,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedBy = Guid.Empty
            };

            try
            {
                await repository.InsertAsync(newSchedule);
                await repository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error creating Schedule: {ex.Message}");
                return null;
            }

            return new EventScheduleResult();
        }

        public async Task<bool> UpdateScheduleAsync(long id, UpdateEventScheduleRequest request)
        {
            EventSchedule? existingSchedule = await repository.GetByIdAsync(id);

            if (existingSchedule == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"Schedule with ID {id} not found.");
                return false;
            }

            existingSchedule.PreSaleStartDate = request.PreSaleDate;
            existingSchedule.PreSaleEndDate = request.PreSaleEndDate;
            existingSchedule.OnSaleDate = request.OnSaleDate;
            existingSchedule.OffSaleDate = request.OffSaleDate;
            existingSchedule.PublishedDate = request.PublishedDate;
            existingSchedule.GateOpenDate = request.GateOpenDate;
            existingSchedule.StartDateTime = request.StartDateTime;
            existingSchedule.EndDateTime = request.EndDateTime;

            existingSchedule.UpdatedAt = DateTimeOffset.UtcNow;
            existingSchedule.UpdatedBy = Guid.Empty;

            // TODO: Add updated At and By columns to schedule or update event instead

            try
            {
                await repository.UpdateAsync(existingSchedule);
                await repository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error updating Schedule: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteScheduleAsync(long id)
        {
            EventSchedule? existingSchedule = await repository.GetByIdAsync(id);

            if (existingSchedule == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"Schedule with ID {id} not found.");
                return false;
            }

            // TODO: Add columns for deletion
            existingSchedule.DeletedAt = DateTimeOffset.UtcNow;
            existingSchedule.UpdatedAt = DateTimeOffset.UtcNow;
            existingSchedule.UpdatedBy = Guid.Empty;

            try
            {
                await repository.UpdateAsync(existingSchedule);
                await repository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error deleting Schedule: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
