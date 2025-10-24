using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Shop;

public sealed class ShopRefreshScheduleEntry
{
    [Name("refresh_id")]
    public string RefreshId { get; init; } = string.Empty;

    [Name("refresh_hour_utc9")]
    public int RefreshHourUtc9 { get; init; }

    [Name("applies_to")]
    public string AppliesTo { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
