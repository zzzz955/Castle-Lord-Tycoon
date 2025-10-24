using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroTemplate
{
    [Name("hero_id")]
    public string HeroId { get; init; } = string.Empty;

    [Name("hero_name_ko")]
    public string HeroNameKo { get; init; } = string.Empty;

    [Name("star_rating")]
    public int StarRating { get; init; }

    [Name("growth_type_ko")]
    public string GrowthTypeKo { get; init; } = string.Empty;

    [Name("attribute")]
    public string Attribute { get; init; } = string.Empty;

    [Name("base_growth")]
    public int BaseGrowth { get; init; }

    [Name("base_attack")]
    public int BaseAttack { get; init; }

    [Name("base_defense")]
    public int BaseDefense { get; init; }

    [Name("base_hp")]
    public int BaseHp { get; init; }

    [Name("crit_rate_bp")]
    public int CritRateBp { get; init; }

    [Name("crit_damage_bp")]
    public int CritDamageBp { get; init; }

    [Name("defense_ignore_bp")]
    public int DefenseIgnoreBp { get; init; }

    [Name("damage_reduction_bp")]
    public int DamageReductionBp { get; init; }

    [Name("unique_effect_ids")]
    public string UniqueEffectIdsRaw { get; init; } = string.Empty;

    [Name("release_fragment_item")]
    public string ReleaseFragmentItem { get; init; } = string.Empty;

    [Name("localization_key")]
    public string LocalizationKey { get; init; } = string.Empty;

    [Name("historical_figure")]
    public bool HistoricalFigure { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;

    public IReadOnlyList<string> UniqueEffectIds => Split(UniqueEffectIdsRaw);

    private static IReadOnlyList<string> Split(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<string>();
        }

        return value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
