using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.DTO.Binders;
using Odasoft.XBOL.DTO.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateSuiteAgreementRequest
    {
        public required long SuiteId { get; set; }

        public required string OwnerName { get; set; } = "";

        [EmailAddress]
        public required string OwnerEmail { get; set; } = "";

        [Phone]
        public required string OwnerPhone { get; set; } = "";

        [ModelBinder(BinderType = typeof(InvariantDateTimeOffsetModelBinder))]
        public required DateTimeOffset StartDate { get; set; }

        [ModelBinder(BinderType = typeof(InvariantDateTimeOffsetModelBinder))]
        [DateGreaterThan("StartDate", ErrorMessage = "EndDate must be greater than StartDate")]
        public required DateTimeOffset EndDate { get; set; }

        public required IFormFile AgreementFile { get; set; }
    }
}
