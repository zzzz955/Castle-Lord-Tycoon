using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Equipment;

public sealed class EquipmentGradeRule
{
    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("grade_name_ko")]
    public string GradeNameKo { get; init; } = string.Empty;

    [Name("substat_count")]
    public int SubstatCount { get; init; }

    [Name("substat_value_pct")]
    public int SubstatValuePct { get; init; }

    [Name("description_ko")]
    public string DescriptionKo { get; init; } = string.Empty;
}
