using System.ComponentModel.DataAnnotations;
using Odasoft.XBOL.DTO.Helpers;

namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateSeasonRequest
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; } = null!;

        public string? Description { get; set; }
        public string? BannerImageUrl { get; set; }
        public string? PosterImageUrl { get; set; }
        public string? LandingUrl { get; set; }

        [Required]
        [Display(Name = "StartDate")]
        public DateTimeOffset? StartDate { get; set; }

        [Required]
        [DateGreaterThan("StartDate")]
        [Display(Name = "EndDate")]
        public DateTimeOffset? EndDate { get; set; }

        public DateTimeOffset? PublishedDate { get; set; }

        [DateGreaterThan("PublishedDate")]
        [Display(Name = "PreSaleDate")]
        public DateTimeOffset? PreSaleDate { get; set; }

        [DateGreaterThan("PreSaleDate")]
        [Display(Name = "OnSaleDate")]
        public DateTimeOffset? OnSaleDate { get; set; }

        [DateGreaterThan("OnSaleDate")]
        [Display(Name = "OffSaleDate")]
        public DateTimeOffset? OffSaleDate { get; set; }
    }
}
