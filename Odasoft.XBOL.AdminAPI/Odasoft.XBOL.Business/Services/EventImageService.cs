using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Extensions;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Responses;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class EventImageService(EventImageRepository _eventImageRepository)
    {
        public async Task<List<EventImageResponse>> GetEventImagesAsync(long eventId)
        {
            return await _eventImageRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vi => vi.EventId == eventId)
                            .Select(vi => new EventImageResponse
                            {
                                Id = vi.Id,
                                EventId = vi.EventId,
                                Title = vi.FileName,
                                ImageBase64 = Convert.ToBase64String(vi.Content),
                                ContentType = vi.ContentType
                            }).ToListAsync();
        }

        public async Task<List<EventImageResponse>> GetEventImagesByImageTypeAsync(long eventId, ImageType imageType)
        {
            return await _eventImageRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vi => vi.EventId == eventId && vi.ImageType == imageType)
                            .Select(vi => new EventImageResponse
                            {
                                Id = vi.Id,
                                EventId = vi.EventId,
                                Title = vi.FileName,
                                ImageBase64 = Convert.ToBase64String(vi.Content),
                                ContentType = vi.ContentType
                            }).ToListAsync();
        }

        public async Task<long> CreateEventImagesAsync(long eventId, ImageType imageType, int order, IFormFile imageFile)
        {
            try
            {
                var imageContent = FileExtensions.ConvertIFormFileToByteArray(imageFile);

                if (imageContent == null)
                {
                    return 0;
                }
                else
                {
                    var eventImage = new EventImage
                    {
                        EventId = eventId,
                        FileName = imageFile.FileName,
                        ImageType = imageType,
                        ContentType = imageFile.ContentType,
                        Content = imageContent,
                        Order = order,
                        CreatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                        CreatedBy = Guid.Empty,
                        UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                        UpdatedBy = Guid.Empty
                    };

                    await _eventImageRepository.InsertAsync(eventImage);
                    await _eventImageRepository.CommitAsync();

                    return eventImage.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to create the images for the event. {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateEventImageByIdAsync(long eventImageId, ImageType imageType, int order, IFormFile imageFile)
        {
            try
            {
                var imageContent = FileExtensions.ConvertIFormFileToByteArray(imageFile);

                if (imageContent == null)
                {
                    Console.WriteLine("Empty image file.");
                    return false;
                }

                EventImage? eventImage = await _eventImageRepository.GetByIdAsync(eventImageId);

                if (eventImage == null)
                {
                    Console.WriteLine("Image not found.");
                    return false;
                }

                eventImage.FileName = imageFile.FileName;
                eventImage.ImageType = imageType;
                eventImage.ContentType = imageFile.ContentType;
                eventImage.Content = imageContent;
                eventImage.Order = order;
                eventImage.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
                eventImage.UpdatedBy = Guid.Empty;

                await _eventImageRepository.UpdateAsync(eventImage);
                await _eventImageRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to update the event image with Id '{eventImageId}'. {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEventImageByIdAsync(long eventImageId)
        {
            try
            {
                EventImage? eventImage = await _eventImageRepository.GetByIdAsync(eventImageId);

                if (eventImage == null)
                {
                    Console.WriteLine($"Event image with Id '{eventImageId}' was not found.");
                    return false;
                }

                await _eventImageRepository.HardDeleteAsync(eventImage);
                await _eventImageRepository.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to delete the event image with Id '{eventImageId}'. {ex.Message}");
                return false;
            }
        }
    }
}
