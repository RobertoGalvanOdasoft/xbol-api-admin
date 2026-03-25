using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Extensions;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Responses;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class VenueImageService
    {
        private readonly VenueImageRepository _venueImageRepository;

        public VenueImageService(VenueImageRepository venueImageRepository)
        {
            _venueImageRepository = venueImageRepository;
        }

        public async Task<List<VenueImageResponse>> GetVenueImagesAsync(long venueId)
        {
            return await _venueImageRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vi => vi.VenueId == venueId)
                            .Select(vi => new VenueImageResponse
                            {
                                Id = vi.Id,
                                VenueId = vi.VenueId,
                                Title = vi.FileName,
                                ImageBase64 = Convert.ToBase64String(vi.Content),
                                ContentType = vi.ContentType
                            }).ToListAsync();
        }

        public async Task<List<VenueImageResponse>> GetVenueImagesByImageTypeAsync(long venueId, ImageType imageType)
        {
            return await _venueImageRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vi => vi.VenueId == venueId && vi.ImageType == imageType)
                            .Select(vi => new VenueImageResponse
                            {
                                Id = vi.Id,
                                VenueId = vi.VenueId,
                                Title = vi.FileName,
                                ImageBase64 = Convert.ToBase64String(vi.Content),
                                ContentType = vi.ContentType
                            }).ToListAsync();
        }

        public async Task<long> CreateVenueImagesAsync(long venueId, ImageType imageType, int order, IFormFile imageFile)
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
                    var venueImage = new VenueImage
                    {
                        VenueId = venueId,
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

                    await _venueImageRepository.InsertAsync(venueImage);
                    await _venueImageRepository.CommitAsync();

                    return venueImage.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to create the images for the venue. {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateVenueImageByIdAsync(long venueImageId, ImageType imageType, int order, IFormFile imageFile)
        {
            try
            {
                var imageContent = FileExtensions.ConvertIFormFileToByteArray(imageFile);

                if (imageContent == null)
                {
                    Console.WriteLine("Empty image file.");
                    return false;
                }

                VenueImage? venueImage = await _venueImageRepository.GetByIdAsync(venueImageId);

                if (venueImage == null)
                {
                    Console.WriteLine("Image not found.");
                    return false;
                }

                venueImage.FileName = imageFile.FileName;
                venueImage.ImageType = imageType;
                venueImage.ContentType = imageFile.ContentType;
                venueImage.Content = imageContent;
                venueImage.Order = order;
                venueImage.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
                venueImage.UpdatedBy = Guid.Empty;

                await _venueImageRepository.UpdateAsync(venueImage);
                await _venueImageRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to update the venue image with Id '{venueImageId}'. {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteVenueImageByIdAsync(long venueImageId)
        {
            try
            {
                VenueImage? venueImage = await _venueImageRepository.GetByIdAsync(venueImageId);

                if (venueImage == null)
                {
                    Console.WriteLine($"Venue image with Id '{venueImageId}' was not found.");
                    return false;
                }

                await _venueImageRepository.HardDeleteAsync(venueImage);
                await _venueImageRepository.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to delete the venue image with Id '{venueImageId}'. {ex.Message}");
                return false;
            }
        }
    }
}
