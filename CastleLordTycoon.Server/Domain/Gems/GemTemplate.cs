using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Gems;

public sealed class GemTemplate
{
    [Name("gem_id")]
    public string GemId { get; init; } = string.Empty;

    [Name("gem_name_ko")]
    public string GemNameKo { get; init; } = string.Empty;

    [Name("color")]
    public string Color { get; init; } = string.Empty;

    [Name("stat_type")]
    public string StatType { get; init; } = string.Empty;

    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("value_percent")]
    public int ValuePercent { get; init; }

    [Name("description_ko")]
    public string DescriptionKo { get; init; } = string.Empty;
}
