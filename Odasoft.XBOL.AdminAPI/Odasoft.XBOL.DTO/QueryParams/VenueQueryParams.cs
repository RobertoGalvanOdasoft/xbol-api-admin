using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.QueryParams
{
    public class VenueQueryParams : BaseQueryParams
    {
        public List<string> Cities { get; set; } = [];
        public List<VenueCategory> Categories { get; set; } = [];
    }
}
