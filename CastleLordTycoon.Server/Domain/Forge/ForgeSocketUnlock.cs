using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Forge;

public sealed class ForgeSocketUnlock
{
    [Name("slot_index")]
    public int SlotIndex { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("required_grade")]
    public string RequiredGrade { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
