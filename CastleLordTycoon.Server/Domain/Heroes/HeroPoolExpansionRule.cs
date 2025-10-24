using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroPoolExpansionRule
{
    [Name("slot_cap")]
    public int SlotCap { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
