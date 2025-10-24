using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroTemplate
{
    [Name("hero_id")]
    public string HeroId { get; init; } = string.Empty;

    [Name("hero_name_ko")]
    public string HeroNameKo { get; init; } = string.Empty;

    [Name("base_star")]
    public int BaseStar { get; init; }

    [Name("rarity")]
    public string Rarity { get; init; } = string.Empty;

    [Name("attribute")]
    public string Attribute { get; init; } = string.Empty;

    [Name("growth_type")]
    public string GrowthType { get; init; } = string.Empty;

    [Name("base_growth")]
    public int BaseGrowth { get; init; }

    [Name("unique_effect_ids")]
    public string UniqueEffectIdsRaw { get; init; } = string.Empty;

    [Name("role")]
    public string Role { get; init; } = string.Empty;

    [Name("recruit_tier_ids")]
    public string RecruitTierIdsRaw { get; init; } = string.Empty;

    public IReadOnlyList<string> UniqueEffectIds => Split(UniqueEffectIdsRaw);

    public IReadOnlyList<string> RecruitTierIds => Split(RecruitTierIdsRaw);

    private static IReadOnlyList<string> Split(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<string>();
        }

        return value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
