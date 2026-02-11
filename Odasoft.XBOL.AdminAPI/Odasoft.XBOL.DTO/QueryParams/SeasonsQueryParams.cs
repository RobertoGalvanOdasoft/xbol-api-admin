using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.QueryParams
{
    public class SeasonsQueryParams : BaseQueryParams
    {
        /// <summary>Minimum bound: returns seasons starting on or after this date.</summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>Maximum bound: returns seasons ending on or before this date.</summary>
        public DateTimeOffset? EndDate { get; set; }

        public SeasonStatus? Status { get; set; }

        /// <summary>Computed filter: CurrentSeason (started and not yet ended) or PastSeason (already ended).</summary>
        public SeasonPeriod? Period { get; set; }
    }
}
