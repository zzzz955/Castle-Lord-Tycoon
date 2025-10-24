using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Equipment;

public sealed class EquipmentSubstatRange
{
    [Name("stat_type")]
    public string StatType { get; init; } = string.Empty;

    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("min_value_pct")]
    public int MinValuePct { get; init; }

    [Name("max_value_pct")]
    public int MaxValuePct { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
