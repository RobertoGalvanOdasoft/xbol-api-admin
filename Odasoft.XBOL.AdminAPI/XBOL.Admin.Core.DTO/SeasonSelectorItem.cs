namespace XBOL.Admin.Core.DTO
{
    public class SeasonSelectorItem
    {
        public long SeasonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
        public long EventId { get; set; }
    }
}
