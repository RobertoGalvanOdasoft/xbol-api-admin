using Odasoft.XBOL.DTO.Results.Reports;

namespace Odasoft.XBOL.Business.Services;

public class ReportService(ITicketingClient ticketingClient)
{
    public async Task<Dictionary<string, SectionSummaryResult>> GetSectionSummaryAsync(string key)
    {
        var report = await ticketingClient.GetSectionSummaryAsync(key);
        return report.ToDictionary(kvp => kvp.Key, kvp => MapSummary<SectionSummaryResult>(
            kvp.Value.Count, kvp.Value.ByStatus, kvp.Value.ByAvailability,
            kvp.Value.ByCategoryLabel, kvp.Value.ByCategoryKey, kvp.Value.ByChannel));
    }

    public async Task<Dictionary<string, ZoneSummaryResult>> GetZoneSummaryAsync(string key)
    {
        var report = await ticketingClient.GetZoneSummaryAsync(key);
        return report.ToDictionary(kvp => kvp.Key, kvp => MapSummary<ZoneSummaryResult>(
            kvp.Value.Count, kvp.Value.ByStatus, kvp.Value.ByAvailability,
            kvp.Value.ByCategoryLabel, kvp.Value.ByCategoryKey, kvp.Value.ByChannel));
    }

    public async Task<Dictionary<string, AvailabilitySummaryResult>> GetAvailabilitySummaryAsync(string key)
    {
        var report = await ticketingClient.GetAvailabilitySummaryAsync(key);
        return report.ToDictionary(kvp => kvp.Key, kvp => MapSummary<AvailabilitySummaryResult>(
            kvp.Value.Count, kvp.Value.ByStatus, kvp.Value.ByAvailability,
            kvp.Value.ByCategoryLabel, kvp.Value.ByCategoryKey, kvp.Value.ByChannel));
    }

    public async Task<Dictionary<string, AvailabilityReasonSummaryResult>> GetAvailabilityReasonSummaryAsync(string key)
    {
        var report = await ticketingClient.GetAvailabilityReasonSummaryAsync(key);
        return report.ToDictionary(kvp => kvp.Key, kvp => MapSummary<AvailabilityReasonSummaryResult>(
            kvp.Value.Count, kvp.Value.ByStatus, kvp.Value.ByAvailability,
            kvp.Value.ByCategoryLabel, kvp.Value.ByCategoryKey, kvp.Value.ByChannel));
    }

    private static TResult MapSummary<TResult>(
        int? count,
        IDictionary<string, int>? byStatus,
        IDictionary<string, int>? byAvailability,
        IDictionary<string, int>? byCategoryLabel,
        IDictionary<string, int>? byCategoryKey,
        IDictionary<string, int>? byChannel) where TResult : SectionSummaryResult, new()
    {
        return new TResult
        {
            Count = count ?? 0,
            ByStatus = ToDictionary(byStatus),
            ByAvailability = ToDictionary(byAvailability),
            ByCategoryLabel = ToDictionary(byCategoryLabel),
            ByCategoryKey = ToDictionary(byCategoryKey),
            ByChannel = ToDictionary(byChannel),
        };
    }

    private static Dictionary<string, int> ToDictionary(IDictionary<string, int>? source) =>
        source is null ? [] : new Dictionary<string, int>(source);
}
