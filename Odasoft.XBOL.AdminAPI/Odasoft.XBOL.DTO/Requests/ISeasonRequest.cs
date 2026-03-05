namespace Odasoft.XBOL.DTO.Requests
{
    public interface ISeasonRequest
    {
        string Name { get; set; }
        string Code { get; set; }
        string? Description { get; set; }
        string? BannerImageUrl { get; set; }
        string? PosterImageUrl { get; set; }
        string? LandingUrl { get; set; }
        DateTimeOffset? StartDate { get; set; }
        DateTimeOffset? EndDate { get; set; }
        DateTimeOffset? PublishedDate { get; set; }
        DateTimeOffset? PreSaleDate { get; set; }
        DateTimeOffset? OnSaleDate { get; set; }
        DateTimeOffset? OffSaleDate { get; set; }
    }
}
