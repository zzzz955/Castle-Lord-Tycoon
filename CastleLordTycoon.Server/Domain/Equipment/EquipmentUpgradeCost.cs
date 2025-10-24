using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Equipment;

public sealed class EquipmentUpgradeCost
{
    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("upgrade_level_min")]
    public int UpgradeLevelMin { get; init; }

    [Name("upgrade_level_max")]
    public int UpgradeLevelMax { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
