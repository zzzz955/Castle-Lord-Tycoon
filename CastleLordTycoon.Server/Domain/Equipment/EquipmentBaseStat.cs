using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Equipment;

public sealed class EquipmentBaseStat
{
    [Name("equipment_slot_id")]
    public string EquipmentSlotId { get; init; } = string.Empty;

    [Name("equipment_slot_ko")]
    public string EquipmentSlotKo { get; init; } = string.Empty;

    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("level_min")]
    public int LevelMin { get; init; }

    [Name("level_max")]
    public int LevelMax { get; init; }

    [Name("primary_stat_type")]
    public string PrimaryStatType { get; init; } = string.Empty;

    [Name("primary_stat_base")]
    public double PrimaryStatBase { get; init; }

    [Name("primary_stat_per_level")]
    public double PrimaryStatPerLevel { get; init; }
}
