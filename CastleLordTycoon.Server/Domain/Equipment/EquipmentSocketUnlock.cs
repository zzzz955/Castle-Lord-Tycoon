using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Equipment;

public sealed class EquipmentSocketUnlock
{
    [Name("slot_index")]
    public int SlotIndex { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("unlock_time_minutes")]
    public int UnlockTimeMinutes { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
