using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Settlements;

public sealed class SettlementTemplate
{
    [Name("settlement_id")]
    public string SettlementId { get; init; } = string.Empty;

    [Name("settlement_name_ko")]
    public string SettlementNameKo { get; init; } = string.Empty;

    [Name("type")]
    public string Type { get; init; } = string.Empty;

    [Name("tier")]
    public int Tier { get; init; }

    [Name("recommended_power")]
    public int RecommendedPower { get; init; }

    [Name("enemy_level_min")]
    public int EnemyLevelMin { get; init; }

    [Name("enemy_level_max")]
    public int EnemyLevelMax { get; init; }

    [Name("shop_level_min")]
    public int ShopLevelMin { get; init; }

    [Name("shop_level_max")]
    public int ShopLevelMax { get; init; }

    [Name("available_services")]
    public string AvailableServicesRaw { get; init; } = string.Empty;

    [Name("unlock_condition_ko")]
    public string UnlockConditionKo { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;

    public IReadOnlyList<string> AvailableServices =>
        string.IsNullOrWhiteSpace(AvailableServicesRaw)
            ? Array.Empty<string>()
            : AvailableServicesRaw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
