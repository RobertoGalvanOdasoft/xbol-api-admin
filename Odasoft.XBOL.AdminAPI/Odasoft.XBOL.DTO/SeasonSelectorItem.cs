namespace XBOL.Admin.Core.DTO
{
    public class SeasonSelectorItem
    {
        public required long SeasonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public required bool IsCurrent { get; set; }
        public long EventId { get; set; }
    }
}
