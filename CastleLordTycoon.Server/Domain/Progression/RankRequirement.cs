using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Progression;

public sealed class RankRequirement
{
    [Name("rank_id")]
    public string RankId { get; init; } = string.Empty;

    [Name("rank_name_ko")]
    public string RankNameKo { get; init; } = string.Empty;

    [Name("order_index")]
    public int OrderIndex { get; init; }

    [Name("required_territory")]
    public int RequiredTerritory { get; init; }

    [Name("exp_bonus_pct")]
    public int ExpBonusPct { get; init; }

    [Name("gold_bonus_pct")]
    public int GoldBonusPct { get; init; }

    [Name("rare_item_bonus_pct")]
    public int RareItemBonusPct { get; init; }

    [Name("exploration_speed_pct")]
    public int ExplorationSpeedPct { get; init; }

    [Name("flag_unlock_size")]
    public int FlagUnlockSize { get; init; }

    [Name("description_ko")]
    public string DescriptionKo { get; init; } = string.Empty;
}
