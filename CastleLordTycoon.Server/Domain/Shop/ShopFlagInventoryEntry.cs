using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Shop;

public sealed class ShopFlagInventoryEntry
{
    [Name("flag_id")]
    public string FlagId { get; init; } = string.Empty;

    [Name("flag_name_ko")]
    public string FlagNameKo { get; init; } = string.Empty;

    [Name("size_tiles")]
    public string SizeTiles { get; init; } = string.Empty;

    [Name("cost_gold")]
    public int CostGold { get; init; }

    [Name("rank_required")]
    public string RankRequired { get; init; } = string.Empty;

    [Name("resale_rate_pct")]
    public int ResaleRatePct { get; init; }

    [Name("max_owned")]
    public int MaxOwned { get; init; }

    [Name("icon_asset")]
    public string IconAsset { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
