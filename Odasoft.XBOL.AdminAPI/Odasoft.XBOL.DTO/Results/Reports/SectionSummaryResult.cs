namespace Odasoft.XBOL.DTO.Results.Reports;

/// <summary>
/// Summary of seat availability and statuses for a specific section.
/// </summary>
public class SectionSummaryResult
{
    public int Count { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = [];
    public Dictionary<string, int> ByAvailability { get; set; } = [];
    public Dictionary<string, int> ByCategoryLabel { get; set; } = [];
    public Dictionary<string, int> ByCategoryKey { get; set; } = [];
    public Dictionary<string, int> ByChannel { get; set; } = [];
}
