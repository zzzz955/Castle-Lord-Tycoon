using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroRebirthRule
{
    [Name("rebirth_stage")]
    public int RebirthStage { get; init; }

    [Name("required_level")]
    public int RequiredLevel { get; init; }

    [Name("required_material_id")]
    public string RequiredMaterialId { get; init; } = string.Empty;

    [Name("required_material_amount")]
    public int RequiredMaterialAmount { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("stat_bonus_pct")]
    public int StatBonusPct { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
