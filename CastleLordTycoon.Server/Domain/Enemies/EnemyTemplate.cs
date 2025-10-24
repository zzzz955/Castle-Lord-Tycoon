using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Enemies;

public sealed class EnemyTemplate
{
    [Name("enemy_id")]
    public string EnemyId { get; init; } = string.Empty;

    [Name("enemy_name_ko")]
    public string EnemyNameKo { get; init; } = string.Empty;

    [Name("visual_id")]
    public string VisualId { get; init; } = string.Empty;

    [Name("type")]
    public string Type { get; init; } = string.Empty;

    [Name("attribute")]
    public string Attribute { get; init; } = string.Empty;

    [Name("base_level")]
    public int BaseLevel { get; init; }

    [Name("base_growth")]
    public int BaseGrowth { get; init; }

    [Name("unique_effect_ids")]
    public string UniqueEffectIdsRaw { get; init; } = string.Empty;

    [Name("base_exp")]
    public int BaseExp { get; init; }

    [Name("drop_table_id")]
    public string DropTableId { get; init; } = string.Empty;

    [Name("tags")]
    public string TagsRaw { get; init; } = string.Empty;

    public IReadOnlyList<string> UniqueEffectIds =>
        SplitSemicolonList(UniqueEffectIdsRaw);

    public IReadOnlyList<string> Tags =>
        SplitSemicolonList(TagsRaw);

    private static IReadOnlyList<string> SplitSemicolonList(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return Array.Empty<string>();
        }

        return source
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }
}
