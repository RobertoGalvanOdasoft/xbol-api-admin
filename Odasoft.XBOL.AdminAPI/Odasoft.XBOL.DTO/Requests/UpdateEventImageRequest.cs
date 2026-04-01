using Microsoft.AspNetCore.Http;
using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public class UpdateEventImageRequest : IEventImageRequest
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public ImageType ImageType { get; set; }
        public int Order { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
}
