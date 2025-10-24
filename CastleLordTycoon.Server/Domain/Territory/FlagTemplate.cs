using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Territory;

public sealed class FlagTemplate
{
    [Name("flag_id")]
    public string FlagId { get; init; } = string.Empty;

    [Name("flag_name_ko")]
    public string FlagNameKo { get; init; } = string.Empty;

    [Name("size_tiles")]
    public string SizeTiles { get; init; } = string.Empty;

    [Name("size_cells")]
    public int SizeCells { get; init; }

    [Name("cost_gold")]
    public int CostGold { get; init; }

    [Name("resale_rate_pct")]
    public int ResaleRatePct { get; init; }

    [Name("rank_required")]
    public string RankRequired { get; init; } = string.Empty;

    [Name("max_owned")]
    public int MaxOwned { get; init; }

    [Name("description_ko")]
    public string DescriptionKo { get; init; } = string.Empty;
}
