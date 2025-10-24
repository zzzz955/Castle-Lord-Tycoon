using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroUniqueEffect
{
    [Name("effect_id")]
    public string EffectId { get; init; } = string.Empty;

    [Name("effect_name_ko")]
    public string EffectNameKo { get; init; } = string.Empty;

    [Name("trigger_type")]
    public string TriggerType { get; init; } = string.Empty;

    [Name("effect_values")]
    public string EffectValues { get; init; } = string.Empty;

    [Name("description_ko")]
    public string DescriptionKo { get; init; } = string.Empty;
}
