using Odasoft.XBOL.DTO.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateSuiteAgreementRequest
    {
        public required long SuiteId { get; set; }

        public required string OwnerName { get; set; } = "";

        [EmailAddress]
        public required string Email { get; set; } = "";

        public long? PhoneRegionCodeId { get; set; }

        [Phone]
        public required string PhoneNumber { get; set; } = "";

        public required DateTimeOffset StartDate { get; set; }

        [DateGreaterThan("StartDate", ErrorMessage = "EndDate must be greater than StartDate")]
        public required DateTimeOffset EndDate { get; set; }
    }
}
